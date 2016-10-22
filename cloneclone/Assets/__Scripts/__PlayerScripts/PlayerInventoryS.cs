using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventoryS : MonoBehaviour {

	private bool initialized = false;

	private List<int> _collectedItems;
	private List<int> _collectedItemCount;
	public List<int> collectedItems { get { return _collectedItems; } }
	public List<int> collectedItemCount { get { return _collectedItemCount; } }
	
	private List<int> _earnedUpgrades;
	public List<int> earnedUpgrades { get { return _earnedUpgrades; } }

	private List<int> _clearedWalls;
	public List<int> clearedWalls { get { return _clearedWalls; } }

	public List<PlayerWeaponS> unlockedWeapons;
	public List<BuddyS> unlockedBuddies;
	private static List<PlayerWeaponS> equippedWeapons;
	private static List<PlayerWeaponS> subWeapons;
	
	private static List<GameObject> equippedBuddies;

	private InventoryManagerS _iManager;

	public static PlayerInventoryS I;

	void Awake () {

		if (I == null){
			Initialize();
		}else{
			if (I != this){
				Destroy(gameObject);
			}
		}

	}

	public void AddToInventory(int i){
		if (!_collectedItems.Contains(i)){
			_collectedItems.Add(i);
			_collectedItemCount.Add(1);
			_iManager.AddNextAvailable(i);
		}else{
			_collectedItemCount[_collectedItems.IndexOf(i)]++;
		}
	}

	public int GetItemCount(int i){
		int count = 0;
		if (_collectedItems.Contains(i)){
			count = _collectedItemCount[_collectedItems.IndexOf(i)];
		}
		return count;
	}

	public void RemoveFromInventory(int i){
		if (_collectedItems.Contains(i)){
			_collectedItemCount[_collectedItems.IndexOf(i)]--;
			if (_collectedItemCount[_collectedItems.IndexOf(i)] <= 0){
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

		_earnedUpgrades = new List<int>();
		_collectedItems = new List<int>();
		_collectedItemCount = new List<int>();
		_clearedWalls = new List<int>();
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
}
