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
	private List<int> laPickupNums;
	private List<int> chargeNums;
	private List<int> vpNums;

	private List<int> checkpointsReachedScenes;
	private List<int> checkpointsReachedSpawns;
	
	private static List<GameObject> equippedBuddies;

	private int _tvNum = 999;
	public int tvNum { get { return _tvNum; } }

	private InventoryManagerS _iManager;
	public InventoryManagerS iManager { get { return _iManager; } }
	private PlayerDestructionS _dManager;
	public PlayerDestructionS dManager { get { return _dManager; } }

	public static PlayerInventoryS I;

	public static InventorySave inventoryData;

	#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
	public static bool DO_NOT_SAVE = false;
	#endif

	void Awake () {

		if (I == null){
			Initialize();
		}else{
			if (I != this){
				Destroy(gameObject);
			}
		}

	}
	#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
	void Update(){
		if (Input.GetKeyDown(KeyCode.Comma)){
			DO_NOT_SAVE = !DO_NOT_SAVE;
			Debug.Log("DO_NOT_SAVE turned to " + DO_NOT_SAVE);
		}
	}
	#endif

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

	public void AddToInventory(int i, bool isKey = false, bool dontAddMultiple = false){
		if (!_collectedItems.Contains(i)){
			if (i != 1){
				_collectedItems.Add(i);
				_collectedItemCount.Add(1);
				_iManager.AddNextAvailable(i);
			}else{
				_collectedItems.Insert(1, i);
				_collectedItemCount.Insert(1, i);
				_iManager.AddAt(1, i);
			}
			if (isKey){
				_collectedKeyItems.Add(i);
			}
		}else{
			if (!dontAddMultiple){
			_collectedItemCount[_collectedItems.IndexOf(i)]++;
			_iManager.RefreshUI();
			}
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
	public void AddLaPickup(int i, int sinAmt){
		if (!laPickupNums.Contains(i)){
			laPickupNums.Add(i);
		}

		GameObject.Find("Player").GetComponent<PlayerController>().myStats.uiReference.cDisplay.AddCurrency(sinAmt);
	}
	public void AddVP(int i){
		if (!vpNums.Contains(i)){
			vpNums.Add(i);
		}
	}

	public int GetVPUpgradeCount(){
		if (vpNums != null){
			return vpNums.Count;
		}else{
			return 0;
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

	public int GetUpgradeCount(int i){
		int count = 0;
		if (_earnedUpgrades != null){
			if (_earnedUpgrades.Contains(i)){
				for (int j = 0; j < _earnedUpgrades.Count; j++){
					if (_earnedUpgrades[j] == i){
						count++;
					}
				}
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
		// Add "Driven" virtue
		if (i == 9){
			AddEarnedVirtue(6);
		}
		// Add "Agile" virtue
		if (i == 10){
			AddEarnedVirtue(9);
		}
		// Add "Loved" virtue
		if (i == 11){
			AddEarnedVirtue(14);
		}
		// Add "Empowered" virtue
		if (i == 12){
			AddEarnedVirtue(2);
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
			laPickupNums = new List<int>();
			chargeNums = new List<int>();
			vpNums = new List<int>();
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
			_collectedItemCount[_collectedItems.IndexOf(1)]=chargeNums.Count;
		}
		if (CheckForItem(2)){
			_collectedItemCount[_collectedItems.IndexOf(2)]=laPickupNums.Count;
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
		return laPickupNums.Contains(n);
	}
	public bool CheckVP(int v){
		return vpNums.Contains(v);
	}

	public void NewGame(){
		_dManager.ClearAllSaved();
		_collectedItems.Clear();
		_collectedItemCount.Clear();
		_collectedKeyItems.Clear();
		_clearedWalls.Clear();
		_openedDoors.Clear();

		checkpointsReachedScenes.Clear();
		checkpointsReachedSpawns.Clear();

		healNums.Clear();
		vpNums.Clear();
		laPickupNums.Clear();
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
		CameraShakeS.SetTurbo();
		List<int> buddyList = new List<int>();
		buddyList.Add(unlockedBuddies[0].buddyNum);
		equippedWeapons = new List<PlayerWeaponS>{unlockedWeapons[0]};
		subWeapons = new List<PlayerWeaponS>{unlockedWeapons[0]};
		LevelUpHandlerS lHandler = GetComponent<LevelUpHandlerS>();
		lHandler.ResetUpgrades();
		_tvNum = Mathf.RoundToInt(Random.Range(100, 999));
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
		vpNums = inventoryData.vpNums;
		laPickupNums = inventoryData.laPickupNums;
		chargeNums = inventoryData.chargeNums;
		_collectedKeyItems = inventoryData.collectedKeyItems;
		_collectedItemCount = inventoryData.collectedItemCount;
		_openedDoors = inventoryData.openedDoors;
		_clearedWalls = inventoryData.clearedWalls;
		_earnedVirtues = inventoryData.earnedVirtues;
		_earnedTech = inventoryData.earnedTech;

		_dManager.LoadCombatsCleared(inventoryData.combatClearedAtLeastOnce, inventoryData.combatRankingData);

		checkpointsReachedScenes = inventoryData.checkpointsReachedScenes;
		checkpointsReachedSpawns = inventoryData.checkpointsReachedSpawns;

		CameraShakeS.SetTurbo(inventoryData.turboSetting);

		TextInputUIS.playerName = inventoryData.playerName;

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

		if (inventoryData.tvNumber != null){
			if (inventoryData.tvNumber < 100 || inventoryData.tvNumber == 999){
				_tvNum = Mathf.FloorToInt(Random.Range(100, 999));
				inventoryData.tvNumber = _tvNum;
			}else{
				_tvNum = inventoryData.tvNumber;
			}
		}else{
			_tvNum = Mathf.FloorToInt(Random.Range(100, 999));
			inventoryData.tvNumber = _tvNum;
		}

		GameOverS.revivePosition = inventoryData.currentSpawnPoint;
		RefreshRechargeables();

		DifficultyS.SetDifficultiesFromInt(inventoryData.sinLevel, inventoryData.punishLevel);

		_iManager.LoadInventory(inventoryData.equippedInventory, inventoryData.currentSelection);
		_iManager.RefreshUI();
	}
	 
	public void OverwriteInventoryData(){

		inventoryData = new InventorySave();

		if (initialized){
			inventoryData.playerName = TextInputUIS.playerName;
		inventoryData.earnedUpgrades = _earnedUpgrades;
		inventoryData.collectedItems = _collectedItems;
		inventoryData.healNums = healNums;
		inventoryData.laPickupNums = laPickupNums;
		inventoryData.chargeNums = chargeNums;
			inventoryData.vpNums = vpNums;
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
				inventoryData.combatRankingData = _dManager.combatClearedRanks;
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
			inventoryData.currentSelection = _iManager.currentSelection;

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

			inventoryData.turboSetting = CameraShakeS.GetTurboInt();

			if (_tvNum < 100 || _tvNum == 999){
				_tvNum = Mathf.FloorToInt(Random.Range(100, 999));
			}
			inventoryData.tvNumber = _tvNum;

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
	public List<int> laPickupNums;
	public List<int> chargeNums;
	public List<int> vpNums;
	public List<int> collectedKeyItems;
	public List<int> collectedItemCount;
	public List<int> openedDoors;
	public List<int> clearedWalls;

	public List<int> combatClearedAtLeastOnce;
	public List<int> combatRankingData;
	
	public List<int> unlockedWeapons;
	public List<int> unlockedBuddies;
	public List<int> equippedWeapons;
	public List<int> subWeapons;

	public List<int> equippedVirtues;
	public List<int> equippedTech;
	
	public List<int> equippedBuddies;
	public int currentParadigm;

	public List<int> equippedInventory;
	public int currentSelection = 0;

	public List<int> nextLevelUpgrades;
	public List<int> availableUpgrades;
	public List<int> lockedUpgrades;

	
	public List<int> checkpointsReachedSpawns;
	public List<int> checkpointsReachedScenes;
	public int currentSpawnPoint;

	public int turboSetting = 0;
	public int tvNumber;

	public int sinLevel;
	public int punishLevel;

	public string playerName;


	public InventorySave(){
		playerName = "LUCAH";
		earnedUpgrades = new List<int>();
		earnedVirtues = new List<int>();
		earnedTech = new List<int>();
		collectedItems = new List<int>();
		healNums = new List<int>();
		laPickupNums = new List<int>();
		chargeNums = new List<int>();
		collectedKeyItems = new List<int>();
		collectedItemCount = new List<int>();
		openedDoors = new List<int>();
		clearedWalls = new List<int>();

		combatClearedAtLeastOnce = new List<int>();
		combatRankingData = new List<int>();

		checkpointsReachedScenes = new List<int>();
		checkpointsReachedSpawns = new List<int>();

		unlockedWeapons = new List<int>();
		unlockedBuddies = new List<int>();
		equippedWeapons = new List<int>();
		subWeapons = new List<int>();
		
		equippedVirtues = new List<int>();
		equippedTech = new List<int>();
		
		equippedBuddies = new List<int>();

		currentSelection = 0;
		equippedInventory = new List<int>();
		currentParadigm = 0;
		currentSpawnPoint = 0;

		availableUpgrades = new List<int>(){0,1,2,6};
		nextLevelUpgrades = new List<int>(){4,5,3};
		lockedUpgrades = new List<int>(){0,1};

		tvNumber = Mathf.FloorToInt(Random.Range(100, 999));

		turboSetting = 0;

		punishLevel = 1;
		sinLevel = 1;

	}
}
