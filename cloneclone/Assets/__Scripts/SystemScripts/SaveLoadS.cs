using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

public class SaveLoadS : MonoBehaviour {

	public static List<GameDataS> savedGames = new List<GameDataS>();
    public static int currentSaveSlot = 0;
	
	//it's static so we can call it from anywhere
	public static void Save() {
        
        if (currentSaveSlot < SaveLoadS.savedGames.Count){
            SaveLoadS.savedGames[currentSaveSlot] = GameDataS.current;
		}else{
            SaveLoadS.savedGames.Add(GameDataS.current);
		}
		if (Application.platform != RuntimePlatform.WebGLPlayer){
		BinaryFormatter bf = new BinaryFormatter();
		//Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
		bf.Serialize(file, SaveLoadS.savedGames);
		file.Close();
		}
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
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            if (savedGames == null || savedGames.Count <= 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
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
    
	}

	public static bool SaveFileExists(){
		if (Application.platform == RuntimePlatform.WebGLPlayer){
			return false;
		}
		else if(File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
			return true;
		}else{
			Debug.Log("Save does not exist");
			return false;
		}
	}

    public static int NumSavesOnDisk(){
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            return 0;
        }
        else if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            if (savedGames == null || savedGames.Count <= 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
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

    public static int LastUsedSave(){
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            return 0;
        }
        else if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            if (savedGames == null || savedGames.Count <= 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
                savedGames = (List<GameDataS>)bf.Deserialize(file);
                file.Close();
            }
            int whichFile = 0;
            for (int i = 0; i < savedGames.Count; i++){
                if (savedGames[i].lastLoaded > 0){
                    whichFile = i;
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
