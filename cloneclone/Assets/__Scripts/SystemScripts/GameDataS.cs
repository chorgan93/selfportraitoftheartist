using UnityEngine;
using System.Collections;
[System.Serializable]
public class GameDataS { 
	
	public static GameDataS current;
	public string currentReviveScene = "IntroCutscene";
	public int currentSpawnPos = 0;
	public int storyProgression = 0;
	public InventorySave playerInventory;
	public bool canUseMenu = false;
	public bool hasUsedMenu = false;

	public float currentDarkness;
	public int currentLa;
	
	public GameDataS () {

		currentReviveScene = "IntroCutscene";
		 currentSpawnPos = 0;
		storyProgression = 0;
	}

	public void OverwriteCurrent(){

		currentReviveScene = GameOverS.reviveScene;
		currentSpawnPos = GameOverS.revivePosition;
		storyProgression = StoryProgressionS.storyProgress;

		canUseMenu = InGameMenuManagerS.allowMenuUse;
		hasUsedMenu = InGameMenuManagerS.hasUsedMenu;

		currentDarkness = PlayerStatsS._currentDarkness;
		currentLa = PlayerCollectionS.currencyCollected;

		if (PlayerInventoryS.I != null){
			PlayerInventoryS.I.OverwriteInventoryData();
			playerInventory = PlayerInventoryS.inventoryData;
		}

	}

	public void LoadCurrent(){

		GameOverS.reviveScene = currentReviveScene;
		SpawnPosManager.whereToSpawn = GameOverS.revivePosition = currentSpawnPos;
		StoryProgressionS.storyProgress = storyProgression;
		PlayerInventoryS.inventoryData = playerInventory;
		InGameMenuManagerS.hasUsedMenu = hasUsedMenu;
		InGameMenuManagerS.allowMenuUse = canUseMenu;
		PlayerStatsS._currentDarkness = currentDarkness;
		PlayerCollectionS.currencyCollected = currentLa;

	}
	
}
