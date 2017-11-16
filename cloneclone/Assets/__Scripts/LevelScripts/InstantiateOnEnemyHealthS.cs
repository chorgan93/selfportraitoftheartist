using UnityEngine;
using System.Collections;

public class InstantiateOnEnemyHealthS : MonoBehaviour {

	public GameObject[] spawnObjs;
	public float spawnAtPercent = 0.5f;
	private bool _hasSpawned = false;

	public void CheckSpawn(float checkPercent){
		if (!_hasSpawned && checkPercent <= spawnAtPercent){
			GameObject newSpawn;
			for (int i = 0; i < spawnObjs.Length; i++){
				newSpawn = Instantiate(spawnObjs[i]) as GameObject;
			newSpawn.SetActive(true);
			}
			_hasSpawned = true;
		}
	}

	public void ResetSpawn(){
		_hasSpawned = false;
	}

}
