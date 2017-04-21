using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectSpawnManagerS : MonoBehaviour {

	public GameObject playerDashEffectPrefab;
	private List<GameObject> playerDashes = new List<GameObject>();

	public GameObject enemyHealthPrefab;
	private List<GameObject> enemyHealthBars = new List<GameObject>();

	public GameObject damageNumberPrefab;
	private List<GameObject> damageNumbers = new List<GameObject>();

	public GameObject projectileTrailPrefab;
	private List<GameObject> projectileTrails = new List<GameObject>();

	private FadeSpriteObjectS fadeRef;

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

	public GameObject SpawnProjectileFade(Vector3 spawnPos, Color newCol, Quaternion newRot, float dSpeed){

		GameObject spawnObj;

		if (projectileTrails.Count > 0){
			spawnObj = projectileTrails[0];
			projectileTrails.Remove(spawnObj);
			spawnObj.transform.position = spawnPos;
			spawnObj.transform.rotation = newRot;
			spawnObj.SetActive(true);
		}else{
			spawnObj = Instantiate(projectileTrailPrefab, spawnPos, newRot) as GameObject;
		}

		spawnObj.transform.parent = null;

		fadeRef = spawnObj.GetComponent<FadeSpriteObjectS>();
		fadeRef.startFadeAlpha = newCol.a;
		spawnObj.GetComponent<SpriteRenderer>().color = newCol;
		fadeRef.SetManager(this, 4);
		fadeRef.SetDrift(dSpeed);

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

	public GameObject SpawnDamangeNum(Vector3 spawnPos, bool isE, bool playerHit, float dmgAmt, Transform newParent = null){

		GameObject spawnObj = null;

		if (PlayerController.equippedUpgrades.Contains(1) && dmgAmt > 0){
		spawnPos.y += 0.8f;
		spawnPos.z = -8f;
		if (damageNumbers.Count > 0){
			spawnObj = damageNumbers[0];
			damageNumbers.Remove(spawnObj);
			spawnObj.transform.position = spawnPos;
			spawnObj.SetActive(true);
		}else{
			spawnObj = Instantiate(damageNumberPrefab, spawnPos, Quaternion.identity) as GameObject;
		}

		spawnObj.GetComponent<DamageNumberS>().Initialize(isE, dmgAmt, playerHit);
		spawnObj.transform.parent = null;
		}

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
		if (spawnCode == 3){
			damageNumbers.Add(target);
		}
		if (spawnCode == 4){
			projectileTrails.Add(target);
		}

	}
}
