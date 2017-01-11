using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventoryS : MonoBehaviour {

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
		if (_collectedItems.Contains(i)){
			count = _collectedItemCount[_collectedItems.IndexOf(i)];
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
	}

	public void AddClearedWall(int i){
		_clearedWalls.Add(i);
	}

	void Initialize(){
		I = this;
		DontDestroyOnLoad(gameObject);

		_iManager = GetComponent<InventoryManagerS>();
		_dManager = GetComponent<PlayerDestructionS>();

		if (inventoryData != null){

			LoadInventoryData();

		}else{


			_earnedUpgrades = new List<int>();
			//_earnedVirtues = new List<int>();
			_collectedItems = new List<int>();
			healNums = new List<int>();
			staminaNums = new List<int>();
			chargeNums = new List<int>();
			_collectedKeyItems = new List<int>();
			_collectedItemCount = new List<int>();
			_openedDoors = new List<int>();
			_clearedWalls = new List<int>();

		}
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

	public void SaveLoadout(List<PlayerWeaponS> wepList, List<PlayerWeaponS> subList, List<GameObject> equipBuds){
		equippedWeapons = wepList;
		subWeapons = subList;
		equippedBuddies = equipBuds;
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
		PlayerInventoryS.I._earnedUpgrades.Clear();
		PlayerInventoryS.I._earnedVirtues.Clear();
		if (unlockedWeapons.Count > 1){
			unlockedWeapons.RemoveRange(1, unlockedWeapons.Count-1);
		}
		if (unlockedBuddies.Count > 1){
			unlockedBuddies.RemoveRange(1, unlockedWeapons.Count-1);
		}
		PlayerStatsS.healOnStart = true;
		PlayerController._currentParadigm = 0;
		PlayerController.currentBuddy = 0;
		SpawnPosManager.whereToSpawn = 0;
		GameOverS.revivePosition = 0;
		List<GameObject> buddyList = new List<GameObject>();
		buddyList.Add(unlockedBuddies[0].gameObject);
		SaveLoadout(unlockedWeapons, unlockedWeapons, buddyList);
		OverriteInventoryData();
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
	}
	 
	public void OverriteInventoryData(){
		if (inventoryData != null){
			inventoryData = new InventorySave();
		}

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
		}
	}
}

[System.Serializable]
public class InventorySave : MonoBehaviour {
	public List<int> earnedUpgrades;
	public List<int> collectedItems;
	public List<int> healNums;
	public List<int> staminaNums;
	public List<int> chargeNums;
	public List<int> collectedKeyItems;
	public List<int> collectedItemCount;
	public List<int> openedDoors;
	public List<int> clearedWalls;


	public InventorySave(){
		earnedUpgrades = new List<int>();
		collectedItems = new List<int>();
		healNums = new List<int>();
		staminaNums = new List<int>();
		chargeNums = new List<int>();
		collectedKeyItems = new List<int>();
		collectedItemCount = new List<int>();
		openedDoors = new List<int>();
		clearedWalls = new List<int>();
	}
}
