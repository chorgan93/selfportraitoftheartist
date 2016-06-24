using UnityEngine;
using System.Collections;

public class RoomClearCheck : MonoBehaviour {

	// place on Enemy Spawners parent to function correctly

	private EnemySpawnerS[] allSpawners;

	private int spawnersCleared = 0;
	private bool _cleared = false;
	public bool cleared { get { return _cleared; } }

	// Use this for initialization
	void Start () {

		allSpawners = GetComponentsInChildren<EnemySpawnerS>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!_cleared && spawnersCleared == allSpawners.Length){

			_cleared = true;

		}

	}

	public void AddClear(){

		spawnersCleared ++;

	}
}
