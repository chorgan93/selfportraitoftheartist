using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class GameDataS { 
	
	public static GameDataS current;
	public string currentReviveScene = "IntroCutscene";
	public int currentSpawnPos = 0;
	public List<int> storyProgression;
	public InventorySave playerInventory;
	public bool canUseMenu = false;
	public bool hasUsedMenu = false;

	public float currentDarkness;
	public int currentLa;

    public int lastLoaded = -1;
	
	public GameDataS () {

		currentReviveScene = "IntroCutscene";
		 currentSpawnPos = 0;
		storyProgression = new List<int>();
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

    public void RemoveCurrent(){

        current = null;
        if (PlayerInventoryS.I)
        {
            PlayerInventoryS.I.NewGame();
        }
        else
        {
            PlayerInventoryS.inventoryData = null;
        }
        SpawnPosManager.whereToSpawn = GameOverS.revivePosition = 0;
        StoryProgressionS.storyProgress = new List<int>();

        InGameMenuManagerS.hasUsedMenu = false;
        InGameMenuManagerS.allowMenuUse = false;
        PlayerStatsS._currentDarkness = 0;
        PlayerCollectionS.currencyCollected = 0;
    }

	public void LoadCurrent(){

		GameOverS.reviveScene = currentReviveScene;
		SpawnPosManager.whereToSpawn = GameOverS.revivePosition = currentSpawnPos;
		StoryProgressionS.storyProgress = storyProgression;
        if (PlayerInventoryS.I)
        {
            PlayerInventoryS.I.LoadNewInventoryData(playerInventory);
        }else{
            PlayerInventoryS.inventoryData = playerInventory;
        }
		InGameMenuManagerS.hasUsedMenu = hasUsedMenu;
		InGameMenuManagerS.allowMenuUse = canUseMenu;
		PlayerStatsS._currentDarkness = currentDarkness;
		PlayerCollectionS.currencyCollected = currentLa;

        lastLoaded = 1;

	}
	
}
