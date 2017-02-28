using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectSpawnManagerS : MonoBehaviour {

	public GameObject playerDashEffectPrefab;
	private List<GameObject> playerDashes = new List<GameObject>();

	public GameObject enemyHealthPrefab;
	private List<GameObject> enemyHealthBars = new List<GameObject>();

	public static EffectSpawnManagerS E;

	// Use this for initialization
	void Awake () {

		E = this;
	
	}

	public GameObject SpawnPlayerFade(Vector3 spawnPos){

		GameObject spawnObj;

		if (playerDashes.Count > 0){
			spawnObj = playerDashes[0];
			playerDashes.Remove(spawnObj);
			spawnObj.transform.position = spawnPos;
			spawnObj.SetActive(true);
		}else{
			spawnObj = Instantiate(playerDashEffectPrefab, spawnPos, Quaternion.identity) as GameObject;
		}

		spawnObj.transform.parent = null;
		spawnObj.GetComponent<FadeSpriteObjectS>().SetManager(this, 1);

		return spawnObj;
	}

	public GameObject SpawnEnemyHealthBar(EnemyS enemyTarget){

		GameObject spawnObj;
		if (enemyHealthBars.Count > 0){
			spawnObj = enemyHealthBars[0];
			enemyHealthBars.Remove(spawnObj);
			spawnObj.transform.position = enemyTarget.transform.position + enemyTarget.healthBarOffset;
			//spawnObj.transform.parent = enemyTarget.transform;
			spawnObj.SetActive(true);
		}else{
			spawnObj = Instantiate(enemyHealthPrefab, enemyTarget.transform.position + enemyTarget.healthBarOffset, 
			                       enemyHealthPrefab.transform.rotation) as GameObject;
			//spawnObj.transform.parent = enemyTarget.transform;
		}

		spawnObj.GetComponent<EnemyHealthBarS>().SetEnemyHealthBar(this, 2, enemyTarget);
		
		return spawnObj;

	}

	public void Despawn(GameObject target, int spawnCode){

		target.SetActive(false);
		target.transform.parent = transform;

		if (spawnCode == 1){
			playerDashes.Add(target);
		}
		if (spawnCode == 2){
			enemyHealthBars.Add(target);
		}

	}
}
