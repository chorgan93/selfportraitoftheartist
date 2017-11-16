using UnityEngine;
using System.Collections;

public class InstantiateOnCombatRestartS : MonoBehaviour {

	public GameObject[] spawnTargets;

	public void SpawnOnRestart(){
		GameObject newSpawn;
		for (int i = 0 ; i < spawnTargets.Length; i++){
			newSpawn= Instantiate(spawnTargets[i]) as GameObject;
			newSpawn.SetActive(true);
		}
	}
}
