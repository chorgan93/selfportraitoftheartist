using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventoryS : MonoBehaviour {

	public LoadoutMasterScriptableObject masterLoadoutList;

	private bool initialized = false;

	private List<int> _collectedItems;
	private List<int> _collectedKeyItems;
	private List<int> _collectedItemCount;
	public List<int> collectedItems { get { return _collectedItems; } }
	public List<int> collectedKeyItems { get { return _collectedKeyItems; } }
	public List<int> collectedItemCount { get { return _collectedItemCount; } }
	
	private List<int> _earnedUpgrades;
	public List<int> earnedUpgrades { get { return _earnedUpgrades; } }
	public List<int> _earnedVirtues;
	public List<int> earnedVirtues { get { return _earnedVirtues; } }
	public List<int> _earnedTech;
	public List<int> earnedTech { get { return _earnedTech; } }

	private List<int> _clearedWalls;
	public List<int> clearedWalls { get { return _clearedWalls; } }
	private List<int> _openedDoors;
	public List<int> openedDoors { get { return _openedDoors; } }

	public List<PlayerWeaponS> unlockedWeapons;
	public List<BuddyS> unlockedBuddies;
	private static List<PlayerWeaponS> equippedWeapons;
	private static List<PlayerWeaponS> subWeapons;

	private List<int> healNums;
	private List<int> staminaNums;
	private List<int> chargeNums;

	private List<int> checkpointsReachedScenes;
	private List<int> checkpointsReachedSpawns;
	
	private static List<GameObject> equippedBuddies;

	private InventoryManagerS _iManager;
	public InventoryManagerS iManager { get { return _iManager; } }
	private PlayerDestructionS _dManager;
	public PlayerDestructionS dManager { get { return _dManager; } }

	public static PlayerInventoryS I;

	public static InventorySave inventoryData;

	void Awake () {

		if (I == null){
			Initialize();
		}else{
			if (I != this){
				Destroy(gameObject);
			}
		}

	}

	public void AddCheckpoint(int sceneIndex, int spawnN){
		if (!checkpointsReachedScenes.Contains(sceneIndex)){
			checkpointsReachedScenes.Add(sceneIndex);
			checkpointsReachedSpawns.Add(spawnN);
		}
	}

	public int ReturnCheckpointIndex(int scene){
		int checkpointNum = 0;
		if (checkpointsReachedScenes.Contains(scene)){
			checkpointNum = checkpointsReachedScenes.IndexOf(scene);
		}
		return checkpointNum;
	}
	public int ReturnCheckpointAtIndex(int index){
		int checkpointNum = -1;
		if (checkpointsReachedScenes.Count > index){
			checkpointNum = checkpointsReachedScenes[index];
		}
		return checkpointNum;
	}
	public int ReturnCheckpointSpawnAtIndex(int index){
		int checkpointNum = -1;
		if (checkpointsReachedScenes.Count > index){
			checkpointNum = checkpointsReachedSpawns[index];
		}
		return checkpointNum;
	}
	public int ReturnCheckpointSpawnAtScene(int sceneIndex){
		int checkpointNum = -1;
		if (ReturnCheckpointIndex(sceneIndex) > -1){
			checkpointNum = ReturnCheckpointSpawnAtIndex(ReturnCheckpointIndex(sceneIndex));
		}
		return checkpointNum;
	}
	public int CheckpointsReached(){
		return checkpointsReachedScenes.Count;
	}

	public bool HasReachedScene(int checkpointSceneIndex){
		bool hasReached = false;
		if (checkpointsReachedScenes != null){
			for (int i = 0; i < checkpointsReachedScenes.Count; i++){
				if (checkpointsReachedScenes[i] == checkpointSceneIndex){
					hasReached = true;
				}
			}
		}
		return hasReached;
	}

	public void AddKeyItem(int i){
		// for key items that should NOT be in inventory (memos)
		if (!_collectedKeyItems.Contains(i)){
			_collectedKeyItems.Add(i);
		}
	}

	public void AddToInventory(int i, bool isKey = false){
		if (!_collectedItems.Contains(i)){
			_collectedItems.Add(i);
			_collectedItemCount.Add(1);
			_iManager.AddNextAvailable(i);
			if (isKey){
				_collectedKeyItems.Add(i);
			}
		}else{
			_collectedItemCount[_collectedItems.IndexOf(i)]++;
			_iManager.RefreshUI();
		}
	}

	public void AddHeal(int i){
		//if (!healNums.Contains(i)){
			healNums.Add(i);
		//}
	}
	public void AddCharge(int i){
		if (!chargeNums.Contains(i)){
			chargeNums.Add(i);
		}
	}
	public void AddStamina(int i){
		if (!staminaNums.Contains(i)){
			staminaNums.Add(i);
		}
	}

	public int GetItemCount(int i){
		int count = 0;
		if (_collectedItems != null){
			
			if (_collectedItems.Contains(i)){
				count = _collectedItemCount[_collectedItems.IndexOf(i)];
			}
		}
		return count;
	}

	public void RemoveFromInventory(int i, bool rechargeable = false){
		if (_collectedItems.Contains(i)){
			_collectedItemCount[_collectedItems.IndexOf(i)]--;
			if (_collectedItemCount[_collectedItems.IndexOf(i)] <= 0 && !rechargeable){
				_collectedItemCount.RemoveAt(_collectedItems.IndexOf(i));
				_collectedItems.Remove(i);
			}
		}
	}

	public bool CheckForItem(int i){
		return (_collectedItems.Contains(i));
	}

	public void AddToUpgrades(int i){
		_earnedUpgrades.Add(i);

		// account for upgrades that add virtues

		// Add "Perceptive" virtue
		if (i == 7){
			AddEarnedVirtue(5);
		}
		// Add "Adaptive" virtue
		if (i == 8){
			AddEarnedVirtue(4);
		}
	}

	public void AddEarnedVirtue(int i){
		if (!_earnedVirtues.Contains(i)){
			_earnedVirtues.Add(i);
		}
	}

	public void AddEarnedTech(int i){
		if (!_earnedTech.Contains(i)){
			_earnedTech.Add(i);
			PlayerController.equippedUpgrades.Add(i);
		}
	}

	public void AddClearedWall(int i){
		_clearedWalls.Add(i);
	}

	void Initialize(){
		I = this;
		DontDestroyOnLoad(gameObject);

		_iManager = GetComponent<InventoryManagerS>();
		_dManager = GetComponent<PlayerDestructionS>();
		
		unlockedBuddies = new List<BuddyS>();

		if (inventoryData != null){

			LoadInventoryData();

		}else{
			
			PlayerController pRef = GameObject.Find("Player").GetComponent<PlayerController>();


			_earnedUpgrades = new List<int>();
			_earnedVirtues = new List<int>();
			_earnedVirtues.Add(0);
			PlayerController.equippedVirtues = new List<int>();
			PlayerController.equippedVirtues.Add(0);
			_collectedItems = new List<int>();
			healNums = new List<int>();
			staminaNums = new List<int>();
			chargeNums = new List<int>();
			_collectedKeyItems = new List<int>();
			_collectedItemCount = new List<int>();
			_openedDoors = new List<int>();
			_clearedWalls = new List<int>();
			equippedWeapons = pRef.equippedWeapons;
			equippedBuddies = pRef.equippedBuddies;
			subWeapons = pRef.subWeapons;

			checkpointsReachedScenes = new List<int>();
			checkpointsReachedSpawns = new List<int>();

			unlockedWeapons = new List<PlayerWeaponS>();
			for (int i = 0; i < pRef.equippedWeapons.Count; i++){
				if (i == 0){
					unlockedWeapons.Add(pRef.equippedWeapons[i]);
				}else{
					if (!unlockedWeapons.Contains(pRef.equippedWeapons[i])){
						unlockedWeapons.Add(pRef.equippedWeapons[i]);
					}
				}
			}

			unlockedBuddies = new List<BuddyS>();
			for (int i = 0; i < pRef.equippedBuddies.Count; i++){
				if (i == 0){
					unlockedBuddies.Add(pRef.equippedBuddies[i].GetComponent<BuddyS>());
				}else{
					if (!unlockedBuddies.Contains(pRef.equippedBuddies[i].GetComponent<BuddyS>())){
						unlockedBuddies.Add(pRef.equippedBuddies[i].GetComponent<BuddyS>());
					}
				}
			}
			
			_earnedTech = new List<int>(){0,1,2,3,7,8,9};
			PlayerController.equippedUpgrades = new List<int>{0,1,2,3,4,7};


		}

		initialized = true;
	}

	public void AddOpenDoor(int i){
		if (!_openedDoors.Contains(i)){
			_openedDoors.Add(i);
		}
	}

	public void RefreshRechargeables(){
		if (CheckForItem(0)){
			_collectedItemCount[_collectedItems.IndexOf(0)]=healNums.Count;
		}
		if (CheckForItem(1)){
			_collectedItemCount[_collectedItems.IndexOf(1)]=staminaNums.Count;
		}
		if (CheckForItem(2)){
			_collectedItemCount[_collectedItems.IndexOf(2)]=chargeNums.Count;
		}
		_iManager.RefreshUI();
	}

	public void SaveLoadout(List<PlayerWeaponS> wepList, List<PlayerWeaponS> subList, List<int> equipBuds){
		equippedWeapons = wepList;
		subWeapons = subList;
		equippedBuddies = new List<GameObject>();
		for (int i = 0; i < equipBuds.Count; i++){
			equippedBuddies.Add(masterLoadoutList.masterBuddyList[equipBuds[i]].gameObject);
		}
	}
	public List<PlayerWeaponS> EquippedWeapons(){
		return equippedWeapons;
	}
	public List<PlayerWeaponS> SubWeapons(){
		return subWeapons;
	}
	public List<GameObject> EquippedBuddies(){
		return equippedBuddies;
	}

	public bool CheckHeal(int n){
		return healNums.Contains(n);
	}
	public bool CheckCharge(int n){
		return chargeNums.Contains(n);
	}
	public bool CheckStim(int n){
		return staminaNums.Contains(n);
	}

	public void NewGame(){
		_dManager.ClearAllSaved();
		_collectedItems.Clear();
		_collectedItemCount.Clear();
		_collectedKeyItems.Clear();
		_clearedWalls.Clear();
		_openedDoors.Clear();
		healNums.Clear();
		if (unlockedWeapons.Count > 1){
			unlockedWeapons.RemoveRange(1, unlockedWeapons.Count-1);
		}
		if (unlockedBuddies.Count > 1){
			unlockedBuddies.RemoveRange(1, unlockedBuddies.Count-1);
		}
		SetUpStartTech();
		PlayerStatsS.healOnStart = true;
		PlayerStatsS._currentDarkness = 0f;
		PlayerCollectionS.currencyCollected = 0;
		PlayerController._currentParadigm = 0;
		SpawnPosManager.whereToSpawn = 0;
		GameOverS.revivePosition = 0;
		List<int> buddyList = new List<int>();
		buddyList.Add(unlockedBuddies[0].buddyNum);
		equippedWeapons = new List<PlayerWeaponS>{unlockedWeapons[0]};
		subWeapons = new List<PlayerWeaponS>{unlockedWeapons[0]};
		SaveLoadout(equippedWeapons, subWeapons, buddyList);
		GameMenuS.ResetOptions();
		OverwriteInventoryData();
	}

	void SetUpStartTech(){

		PlayerInventoryS.I._earnedUpgrades.Clear();
		PlayerInventoryS.I._earnedVirtues.Clear();
		PlayerInventoryS.I._earnedVirtues.Add(0);
		PlayerController.equippedVirtues = new List<int>();
		PlayerController.equippedVirtues.Add(0);
		PlayerController.equippedUpgrades = new List<int>{0,1,2,3,4,7};
		_earnedTech =  new List<int>{0,1,2,3,4,7};
	}

	void LoadInventoryData(){
		_earnedUpgrades = inventoryData.earnedUpgrades;
		_collectedItems = inventoryData.collectedItems;
		healNums = inventoryData.healNums;
		staminaNums = inventoryData.staminaNums;
		chargeNums = inventoryData.chargeNums;
		_collectedKeyItems = inventoryData.collectedKeyItems;
		_collectedItemCount = inventoryData.collectedItemCount;
		_openedDoors = inventoryData.openedDoors;
		_clearedWalls = inventoryData.clearedWalls;
		_earnedVirtues = inventoryData.earnedVirtues;
		_earnedTech = inventoryData.earnedTech;

		_dManager.LoadCombatsCleared(inventoryData.combatClearedAtLeastOnce);

		checkpointsReachedScenes = inventoryData.checkpointsReachedScenes;
		checkpointsReachedSpawns = inventoryData.checkpointsReachedSpawns;


		unlockedWeapons = new List<PlayerWeaponS>();
		equippedWeapons = new List<PlayerWeaponS>();
		subWeapons = new List<PlayerWeaponS>();
		unlockedBuddies = new List<BuddyS>();
		equippedBuddies = new List<GameObject>();

		for (int i = 0; i < inventoryData.unlockedWeapons.Count; i++){
			unlockedWeapons.Add(masterLoadoutList.masterWeaponList[inventoryData.unlockedWeapons[i]]);
		}
		for (int i = 0; i < inventoryData.equippedWeapons.Count; i++){
			equippedWeapons.Add(masterLoadoutList.masterWeaponList[inventoryData.equippedWeapons[i]]);
		}
		for (int i = 0; i < inventoryData.subWeapons.Count; i++){
			subWeapons.Add(masterLoadoutList.masterWeaponList[inventoryData.subWeapons[i]]);
		}
		for (int i = 0; i < inventoryData.unlockedBuddies.Count; i++){
			unlockedBuddies.Add(masterLoadoutList.masterBuddyList[inventoryData.unlockedBuddies[i]]);
		}
		for (int i = 0; i < inventoryData.equippedBuddies.Count; i++){
			equippedBuddies.Add(masterLoadoutList.masterBuddyList[inventoryData.equippedBuddies[i]].gameObject);
		}

		PlayerController._currentParadigm = inventoryData.currentParadigm;
		
		PlayerController.equippedVirtues = inventoryData.equippedVirtues;
		PlayerController.equippedUpgrades = inventoryData.equippedTech;

		LevelUpHandlerS lHandler = GetComponent<LevelUpHandlerS>();
		List<LevelUpS> nLU = new List<LevelUpS>();
		List<LevelUpS> aLU = new List<LevelUpS>();
		List<LockedLevelUpS> lLU = new List<LockedLevelUpS>();
		for (int i = 0; i < inventoryData.nextLevelUpgrades.Count; i++){
			nLU.Add(masterLoadoutList.levelUpList[inventoryData.nextLevelUpgrades[i]]);
		}
		for (int i = 0; i < inventoryData.availableUpgrades.Count; i++){
			aLU.Add(masterLoadoutList.levelUpList[inventoryData.availableUpgrades[i]]);
		}
		for (int i = 0; i < inventoryData.lockedUpgrades.Count; i++){
			lLU.Add(masterLoadoutList.lockedUpList[inventoryData.lockedUpgrades[i]]);
		}
		lHandler.LoadLists(nLU, aLU, lLU);

		GameOverS.revivePosition = inventoryData.currentSpawnPoint;
		RefreshRechargeables();

		DifficultyS.SetDifficultiesFromInt(inventoryData.sinLevel, inventoryData.punishLevel);

		_iManager.LoadInventory(inventoryData.equippedInventory);
		_iManager.RefreshUI();
	}
	 
	public void OverwriteInventoryData(){

		inventoryData = new InventorySave();

		if (initialized){
		inventoryData.earnedUpgrades = _earnedUpgrades;
		inventoryData.collectedItems = _collectedItems;
		inventoryData.healNums = healNums;
		inventoryData.staminaNums = staminaNums;
		inventoryData.chargeNums = chargeNums;
		inventoryData.collectedKeyItems = _collectedKeyItems;
		inventoryData.collectedItemCount = _collectedItemCount;
		inventoryData.openedDoors = _openedDoors;
		inventoryData.clearedWalls = _clearedWalls;
			inventoryData.earnedVirtues = _earnedVirtues;
			inventoryData.earnedTech = _earnedTech;

			inventoryData.checkpointsReachedScenes = checkpointsReachedScenes;
			inventoryData.checkpointsReachedSpawns = checkpointsReachedSpawns;
			inventoryData.currentSpawnPoint = GameOverS.revivePosition;

			if (_dManager.combatClearedAtLeastOnce != null){
				inventoryData.combatClearedAtLeastOnce = _dManager.combatClearedAtLeastOnce;
			}


			for (int i = 0; i < unlockedWeapons.Count; i++){
				inventoryData.unlockedWeapons.Add(unlockedWeapons[i].weaponNum);
			}

			for (int i = 0; i < unlockedBuddies.Count; i++){
				inventoryData.unlockedBuddies.Add(unlockedBuddies[i].buddyNum);
			}
			for (int i = 0; i < equippedWeapons.Count; i++){
				inventoryData.equippedWeapons.Add(equippedWeapons[i].weaponNum);
			}
			for (int i = 0; i < subWeapons.Count; i++){
				inventoryData.subWeapons.Add(subWeapons[i].weaponNum);
			}
			
			inventoryData.equippedVirtues = PlayerController.equippedVirtues;
			inventoryData.equippedTech = PlayerController.equippedUpgrades;

			for (int i = 0; i < equippedBuddies.Count; i++){
				inventoryData.equippedBuddies.Add(equippedBuddies[i].GetComponent<BuddyS>().buddyNum);
			}

			inventoryData.equippedInventory = _iManager.equippedInventory;

			LevelUpHandlerS lHandler = GetComponent<LevelUpHandlerS>();
			inventoryData.availableUpgrades = new List<int>();
			for (int i = 0; i < lHandler.availableLevelUps.Count; i++){
				inventoryData.availableUpgrades.Add(lHandler.availableLevelUps[i].upgradeID);
			}
			inventoryData.nextLevelUpgrades = new List<int>();
			for (int i = 0; i < lHandler.nextLevelUps.Count; i++){
				inventoryData.nextLevelUpgrades.Add(lHandler.nextLevelUps[i].upgradeID);
			}
			inventoryData.lockedUpgrades = new List<int>();
			for (int i = 0; i < lHandler.lockedLevelUps.Count; i++){
				inventoryData.lockedUpgrades.Add(lHandler.lockedLevelUps[i].lockedUpgradeID);
			}

			inventoryData.currentParadigm = PlayerController._currentParadigm;

			inventoryData.sinLevel = DifficultyS.GetSinInt();
			inventoryData.punishLevel = DifficultyS.GetPunishInt();



		}
	}
}

[System.Serializable]
public class InventorySave {
	public List<int> earnedUpgrades;
	public List<int> earnedVirtues;
	public List<int> earnedTech;
	public List<int> collectedItems;
	public List<int> healNums;
	public List<int> staminaNums;
	public List<int> chargeNums;
	public List<int> collectedKeyItems;
	public List<int> collectedItemCount;
	public List<int> openedDoors;
	public List<int> clearedWalls;

	public List<int> combatClearedAtLeastOnce;
	
	public List<int> unlockedWeapons;
	public List<int> unlockedBuddies;
	public List<int> equippedWeapons;
	public List<int> subWeapons;

	public List<int> equippedVirtues;
	public List<int> equippedTech;
	
	public List<int> equippedBuddies;
	public int currentParadigm;

	public List<int> equippedInventory;

	public List<int> nextLevelUpgrades;
	public List<int> availableUpgrades;
	public List<int> lockedUpgrades;

	
	public List<int> checkpointsReachedSpawns;
	public List<int> checkpointsReachedScenes;
	public int currentSpawnPoint;

	public int sinLevel;
	public int punishLevel;


	public InventorySave(){
		earnedUpgrades = new List<int>();
		earnedVirtues = new List<int>();
		earnedTech = new List<int>();
		collectedItems = new List<int>();
		healNums = new List<int>();
		staminaNums = new List<int>();
		chargeNums = new List<int>();
		collectedKeyItems = new List<int>();
		collectedItemCount = new List<int>();
		openedDoors = new List<int>();
		clearedWalls = new List<int>();

		combatClearedAtLeastOnce = new List<int>();

		checkpointsReachedScenes = new List<int>();
		checkpointsReachedSpawns = new List<int>();

		unlockedWeapons = new List<int>();
		unlockedBuddies = new List<int>();
		equippedWeapons = new List<int>();
		subWeapons = new List<int>();
		
		equippedVirtues = new List<int>();
		equippedTech = new List<int>();
		
		equippedBuddies = new List<int>();

		equippedInventory = new List<int>();
		currentParadigm = 0;
		currentSpawnPoint = 0;

		availableUpgrades = new List<int>(){0,1,2,6};
		nextLevelUpgrades = new List<int>(){4,5,3};
		lockedUpgrades = new List<int>(){0,1};

		punishLevel = 1;
		sinLevel = 1;

	}
}
