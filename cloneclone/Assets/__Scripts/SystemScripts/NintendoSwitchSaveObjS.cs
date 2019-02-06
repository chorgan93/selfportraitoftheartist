using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_SWITCH
using System.IO;
using nn.fs;
using nn.account;
using System.Runtime.Serialization.Formatters.Binary;
#endif


public class NintendoSwitchSaveObjS : MonoBehaviour
{
#if UNITY_SWITCH
    private Uid userId; // user ID for the user account on the Nintendo Switch
    private FileHandle fileHandle = new nn.fs.FileHandle();
#endif
    private const string mountName = "saveData";
    private string saveDataPath = mountName + ":/savedGames.gd";

    // Save journaling memory is used for each time files are created, deleted, or written.
    // The journaling memory is freed after nn::fs::CommitSaveData is called.
    // For any single time you save data, check the file size against your journaling size.
    // Check against the total save data size only when you want to be sure all files don't exceed the limit.
    // The variable journalSaveDataSize is only a value that is checked against in this code. The actual journal size is set in the
    // Unity editor in PlayerSettings > Publishing Settings > User account save data    
    private const int journalSaveDataSize = 12976128;   // 16 KB. This value should be 32KB less than the journal size
                                                     // entered in PlayerSettings > Publishing Settings
    private const int loadBufferSize = 32768;  // 32 KB

    public static NintendoSwitchSaveObjS singleton;

    private void Awake()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        initialize();
        DontDestroyOnLoad(gameObject);
#else
        gameObject.SetActive(false);
#endif
    }

#if UNITY_SWITCH
    public void initialize()
    {
        nn.account.Account.Initialize();
        nn.account.UserHandle userHandle = new nn.account.UserHandle();
        nn.account.Account.OpenPreselectedUser(ref userHandle);
        nn.account.Account.GetUserId(ref userId, userHandle);

        // mount save data        
        nn.Result result = nn.fs.SaveData.Mount(mountName, userId);

        //print out error (debug only) and abort if the filesystem couldn't be mounted
        if (result.IsSuccess() == false)
        {
            Debug.Log("Critical Error: File System could not be mounted.");
            result.abortUnlessSuccess();
        }
        singleton = this;


    }

    void OnDestroy()
    {
        nn.fs.FileSystem.Unmount(mountName);
    }


    public void save()
    {
        string filePath = saveDataPath;

        byte[] dataByteArray;
        using (MemoryStream stream = new MemoryStream(journalSaveDataSize)) // the stream size must be less than or equal to the save journal size
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, SaveLoadS.savedGames);
            stream.Close();
            dataByteArray = stream.GetBuffer();
        }

#if !UNITY_EDITOR
        // This next line prevents the user from quitting the game while saving.
        // This is required for Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif

        // If you only ever save the entire file, it may be simpler just to delete the file and create a new one every time you save.
        // Most of the functions return an nn.Result which can be used for debugging purposes.
        nn.fs.File.Delete(filePath);
        nn.fs.File.Create(filePath, journalSaveDataSize); //this makes a file the size of your save journal. You may want to make a file smaller than this.
        nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Write);
        nn.fs.File.Write(fileHandle, 0, dataByteArray, dataByteArray.LongLength, nn.fs.WriteOption.Flush); // Writes and flushes the write at the same time
        nn.fs.File.Close(fileHandle);
        nn.fs.FileSystem.Commit(mountName); //you must commit the changes.

#if !UNITY_EDITOR
        // End preventing the user from quitting the game while saving.
        UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif

    }


    public bool load()
    {

        nn.fs.EntryType entryType = 0; //init to a dummy value (C# requirement)
        nn.fs.FileSystem.GetEntryType(ref entryType, saveDataPath);
        nn.Result result = nn.fs.File.Open(ref fileHandle, saveDataPath, nn.fs.OpenFileMode.Read); 
        if (result.IsSuccess() == false)
        {
            return false;   // Could not open file. This can be used to detect if this is the first time a user has launched your game.
                            // (However, be sure you are not getting this error due to your file being locked by another process, etc.)
        }
        //byte[] loadedData = new byte[loadBufferSize]; // old example style
        long fileSize = 0;
        nn.fs.File.GetSize(ref fileSize, fileHandle);
        byte[] loadedData = new byte[fileSize];
        nn.fs.File.Read(fileHandle, 0, loadedData, fileSize);
        nn.fs.File.Close(fileHandle);

        using (MemoryStream stream = new MemoryStream(loadedData))
        {
            BinaryFormatter bf = new BinaryFormatter();
            SaveLoadS.savedGames = (List<GameDataS>)bf.Deserialize(stream);
        }
        return true;
    }
#endif
}
