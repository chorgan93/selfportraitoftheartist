using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventoryS : MonoBehaviour {

	private bool initialized = false;

	private List<int> _collectedItems;
	public List<int> collectedItems { get { return _collectedItems; } }
	
	private List<int> _earnedUpgrades;
	public List<int> earnedUpgrades { get { return _earnedUpgrades; } }

	private List<int> _clearedWalls;
	public List<int> clearedWalls { get { return _clearedWalls; } }

	public List<PlayerWeaponS> unlockedWeapons;
	public List<BuddyS> unlockedBuddies;
	private static List<PlayerWeaponS> equippedWeapons;
	private static List<PlayerWeaponS> subWeapons;
	
	private static List<GameObject> equippedBuddies;

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
		_collectedItems.Add(i);
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

		_earnedUpgrades = new List<int>();
		_collectedItems = new List<int>();
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
