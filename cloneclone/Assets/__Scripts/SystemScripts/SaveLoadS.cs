using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

public class SaveLoadS : MonoBehaviour {

	public static List<GameDataS> savedGames = new List<GameDataS>();
	
	//it's static so we can call it from anywhere
	public static void Save() {
		// for now, only use one file
		if (SaveLoadS.savedGames.Count > 0){
			SaveLoadS.savedGames[0] = GameDataS.current;
		}else{
			SaveLoadS.savedGames.Add(GameDataS.current);
		}
		BinaryFormatter bf = new BinaryFormatter();
		//Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
		bf.Serialize(file, SaveLoadS.savedGames);
		file.Close();
	}  

	public static void OverriteCurrentSave(){
		if (GameDataS.current == null){
			GameDataS.current = new GameDataS();
		}
		GameDataS.current.OverriteCurrent();
		Save ();
	}
	
	public static void Load() {
		if(File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
			SaveLoadS.savedGames = (List<GameDataS>)bf.Deserialize(file);
			file.Close();
		}

		// load first file, for now
		if (savedGames.Count > 0){
			GameDataS.current = SaveLoadS.savedGames[0];
			GameDataS.current.LoadCurrent();
		}
	}
}
