using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventoryS : MonoBehaviour {

	private bool initialized = false;

	private List<int> _collectedItems;
	public List<int> collectedItems { get { return _collectedItems; } }

	private List<int> _clearedWalls;
	public List<int> clearedWalls { get { return _clearedWalls; } }

	public static PlayerInventoryS I;

	void Awake () {

		if (!initialized){
			if (I == null){
				Initialize();
			}else{
				Destroy(gameObject);
			}
		}
	}

	public void AddToInventory(int i){
		_collectedItems.Add(i);
	}

	public void AddClearedWall(int i){
		_clearedWalls.Add(i);
	}

	void Initialize(){
		I = this;
		DontDestroyOnLoad(gameObject);

		_collectedItems = new List<int>();
		_clearedWalls = new List<int>();
	}
}
