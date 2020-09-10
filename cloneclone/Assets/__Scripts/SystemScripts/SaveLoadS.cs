using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

public class SaveLoadS : MonoBehaviour {

	public static List<GameDataS> savedGames = new List<GameDataS>();
    public static int currentSaveSlot = 0;
    public static bool challengeUnlocked = false;
    public static bool turboUnlocked = false;
	
	//it's static so we can call it from anywhere
	public static void Save() {

#if UNITY_SWITCH
        if (currentSaveSlot < SaveLoadS.savedGames.Count)
        {
            SaveLoadS.savedGames[currentSaveSlot] = GameDataS.current;
        }
        else
        {
            SaveLoadS.savedGames.Add(GameDataS.current);
        }
        NintendoSwitchSaveObjS.singleton.save();
#else
        if (currentSaveSlot < SaveLoadS.savedGames.Count){
            SaveLoadS.savedGames[currentSaveSlot] = GameDataS.current;
		}else{
            SaveLoadS.savedGames.Add(GameDataS.current);
		}
		if (Application.platform != RuntimePlatform.WebGLPlayer){
		BinaryFormatter bf = new BinaryFormatter();
		//Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
		FileStream file = System.IO.File.Create (Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
		bf.Serialize(file, SaveLoadS.savedGames);
		file.Close();
		}
#endif
	}  

	public static void OverwriteCurrentSave(){
		
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
		if (!PlayerInventoryS.DO_NOT_SAVE){
#endif
			
		if (GameDataS.current == null){
			GameDataS.current = new GameDataS();
		}
		GameDataS.current.OverwriteCurrent();
		Save ();
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
		}
#endif
	}

    public static void Load(int saveToLoad = 0)
    {
#if UNITY_SWITCH
        if (!NintendoSwitchSaveObjS.singleton.load())
        {
            return;
        }
        else {
            if (savedGames.Count < saveToLoad && savedGames.Count > 0)
            {
                Debug.Log("There was an error, default to 0");
                saveToLoad = savedGames.Count - 1;
            }

            // load selected file, and set rest to not last loaded
            for (int i = 0; i < savedGames.Count; i++)
                if (i != saveToLoad)
                {
                    savedGames[i].lastLoaded = 0;
                }
                else
                {
                    savedGames[i].lastLoaded = 1;
                }

            // finally, load game
            currentSaveSlot = saveToLoad;
            GameDataS.current = savedGames[currentSaveSlot];
            GameDataS.current.LoadCurrent();
        }
#else
        if (System.IO.File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            if (savedGames == null || savedGames.Count <= 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = System.IO.File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
                savedGames = (List<GameDataS>)bf.Deserialize(file);
                file.Close();
            }


            if (savedGames.Count < saveToLoad && savedGames.Count > 0)
            {
                Debug.Log("There was an error, default to 0");
                saveToLoad = savedGames.Count-1;
            }

            // load selected file, and set rest to not last loaded
            for (int i = 0; i < savedGames.Count; i++)
                if (i != saveToLoad)
                {
                    savedGames[i].lastLoaded = 0;
                }
                else
                {
                    savedGames[i].lastLoaded = 1;
                }

            // finally, load game
            currentSaveSlot = saveToLoad;
            GameDataS.current = savedGames[currentSaveSlot];
            GameDataS.current.LoadCurrent();
        }
#endif
    
	}

	public static bool SaveFileExists(){
#if UNITY_SWITCH && !UNITY_EDITOR
        return NintendoSwitchSaveObjS.singleton.load();
#endif
        if (Application.platform == RuntimePlatform.WebGLPlayer){
			return false;
		}
		else if(System.IO.File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
			return true;
		}else{
			Debug.Log("Save does not exist");
			return false;
		}
	}

    public static int SavedLanguage()
    {
        int lastUsedLanguage = -1;
#if UNITY_SWITCH
        if (!SaveFileExists())
        {
            return lastUsedLanguage;
        }
        else {
            int whichFile = 0;
            for (int i = 0; i < savedGames.Count; i++)
            {
                if (savedGames[i].lastLoaded > 0)
                {
                    if (savedGames[i].currentLanguage != null) {
                        lastUsedLanguage = savedGames[i].currentLanguage;
                    }
                }
            }
            return lastUsedLanguage;
        }
#endif
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            return lastUsedLanguage;
        }
        else if (System.IO.File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            if (savedGames == null || savedGames.Count <= 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = System.IO.File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
                savedGames = (List<GameDataS>)bf.Deserialize(file);
                file.Close();
            }
            for (int i = 0; i < savedGames.Count; i++)
            {
                if (savedGames[i].lastLoaded > 0)
                {
                    if (savedGames[i].currentLanguage != null)
                    {
                        lastUsedLanguage = savedGames[i].currentLanguage;
                    }
                }
            }
            return lastUsedLanguage;
        }
        else
        {
            Debug.Log("Save does not exist");
            return lastUsedLanguage;
        }
    }

    public static int NumSavesOnDisk(){
#if UNITY_SWITCH
        if (!SaveFileExists())
        {
            return 0;
        }
        else {
            return savedGames.Count;
        }
#endif
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            return 0;
        }
        else if (System.IO.File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            if (savedGames == null || savedGames.Count <= 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = System.IO.File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
                savedGames = (List<GameDataS>)bf.Deserialize(file);
                file.Close();
            }
            return savedGames.Count;
        }
        else
        {
            Debug.Log("Save does not exist");
            return 0;
        }
    }

    public static int LastUsedSave()
    {
#if UNITY_SWITCH
        if (!SaveFileExists())
        {
            return 0;
        }
        else {
            int whichFile = 0;
            for (int i = 0; i < savedGames.Count; i++)
            {
                if (savedGames[i].lastLoaded > 0)
                {
                    whichFile = i;
                }
                if (savedGames[i].playerInventory.unlockedChallenge)
                {
                    challengeUnlocked = true;
                }
                if (savedGames[i].playerInventory.unlockedTurbo)
                {
                    turboUnlocked = true;
                }
            }
            return whichFile;
        }
#endif
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            return 0;
        }
        else if (System.IO.File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            if (savedGames == null || savedGames.Count <= 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = System.IO.File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
                savedGames = (List<GameDataS>)bf.Deserialize(file);
                file.Close();
            }
            int whichFile = 0;
            for (int i = 0; i < savedGames.Count; i++){
                if (savedGames[i].lastLoaded > 0){
                    whichFile = i;
                }
                if (savedGames[i].playerInventory.unlockedChallenge){
                    challengeUnlocked = true;
                }
                if (savedGames[i].playerInventory.unlockedTurbo)
                {
                    turboUnlocked = true;
                }
            }
            return whichFile;
        }
        else
        {
            Debug.Log("Save does not exist");
            return 0;
        }
    }
}
