using UnityEngine;
using System.Collections;
[System.Serializable]
public class GameDataS { 
	
	public static GameDataS current;
	public string currentReviveScene = "IntroCutscene";
	public int currentSpawnPos = 0;
	public int storyProgression = 0;
	public InventorySave playerInventory;
	
	public GameDataS () {

		currentReviveScene = "IntroCutscene";
		 currentSpawnPos = 0;
		storyProgression = 0;
	}

	public void OverriteCurrent(){

		currentReviveScene = GameOverS.reviveScene;
		currentSpawnPos = GameOverS.revivePosition;
		storyProgression = StoryProgressionS.storyProgress;
		if (PlayerInventoryS.I != null){
			PlayerInventoryS.I.OverriteInventoryData();
			playerInventory = PlayerInventoryS.inventoryData;
		}

	}

	public void LoadCurrent(){

		GameOverS.reviveScene = currentReviveScene;
		GameOverS.revivePosition = currentSpawnPos;
		StoryProgressionS.storyProgress = storyProgression;
		PlayerInventoryS.inventoryData = playerInventory;

	}
	
}
