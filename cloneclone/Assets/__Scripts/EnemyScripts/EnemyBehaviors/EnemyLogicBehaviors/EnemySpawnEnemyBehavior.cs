using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnEnemyBehavior : EnemyBehaviorS {

	public bool stopEnemy = false;
	[Header("Behavior Duration")]
	public float behaviorDuration = -1f;
	private float behaviorCountdown;

	[Header("Spawn Variables")]
	public float spawnDelay = 0.8f;
	public float timeBetweenSpawns = 0.3f;
	public EnemySpawnerS[] spawnReferences;
	public string searchSpawnReferenceTag = "";
	private bool foundSpawnReferences;
	private List<EnemySpawnerS> externalSpawners;
	public bool parentSpawn = false;
	public int numToSpawnPer = 3;
	private int currentSpawns;

	private float currentSpawnDelay;

	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (BehaviorActing()){

			BehaviorUpdate();

			DoSpawns();

			behaviorCountdown -= Time.deltaTime*currentDifficultyMult;
			if (behaviorCountdown <= 0){
				EndAction();
			}
		}
		
	}
	
	private void InitializeAction(){

		if (!foundSpawnReferences && searchSpawnReferenceTag != ""){
			FindExternalSpawners();
		}

		if (CanSpawn()){
			myEnemyReference.myAnimator.SetTrigger(animationKey);
			if (signalObj != null){
				Vector3 signalPos =  transform.position;
				signalPos.z = transform.position.z+1f;
				GameObject signal = Instantiate(signalObj, signalPos, Quaternion.identity)
					as GameObject;
				signal.transform.parent = myEnemyReference.transform;
			}
			myEnemyReference.AttackFlashEffect();
		if (stopEnemy){
			myEnemyReference.myRigidbody.velocity = Vector3.zero;
		}
		myEnemyReference.AttackFlashEffect();

		currentSpawns = 0;
		currentSpawnDelay = spawnDelay;
		behaviorCountdown = behaviorDuration;
		}else{
			EndAction();
		}
		
	}

	void FindExternalSpawners(){
		GameObject[] externalSpawns = GameObject.FindGameObjectsWithTag(searchSpawnReferenceTag);
		externalSpawners = new List<EnemySpawnerS>();
		for (int i = 0; i < externalSpawns.Length; i++){
			externalSpawners.Add(externalSpawns[i].GetComponent<EnemySpawnerS>());
			externalSpawners[i].gameObject.SetActive(false);
			externalSpawners[i].allowSpawn = true;
		}
		foundSpawnReferences = true;
	}

	private void DoSpawns(){

		if (currentSpawns < numToSpawnPer){
			currentSpawnDelay -= Time.deltaTime*currentDifficultyMult;
			if (currentSpawnDelay <= 0){
				currentSpawnDelay = timeBetweenSpawns;
				SpawnAnEnemy();
				currentSpawns++;
			}
		}

	}

	public override void StartAction (bool setAnimTrigger = true)
	{
		base.StartAction (false);
		InitializeAction();

	}
	
	public override void EndAction (bool doNextAction = true)
	{
		base.EndAction (doNextAction);
	}

	void SpawnAnEnemy(){
		if (CanSpawn()){
			bool spawnedEnemy = false;
			if (foundSpawnReferences){
				for (int i = 0; i < externalSpawners.Count; i++){
					if (!spawnedEnemy){
						if (externalSpawners[i].enemySpawned){
							if (externalSpawners[i].currentSpawnedEnemy.isDead || !externalSpawners[i].currentSpawnedEnemy.gameObject.activeSelf){
								externalSpawners[i].RespawnEnemies(false);
								spawnedEnemy = true;
							}
						}else{
							externalSpawners[i].myManager = myEnemyReference.mySpawner.myManager;
							externalSpawners[i].gameObject.SetActive(true);
							spawnedEnemy = true;
						}
					}
				}
			}else{
			for (int i = 0; i < spawnReferences.Length; i++){
				if (!spawnedEnemy){
				if (spawnReferences[i].enemySpawned){
						if (spawnReferences[i].currentSpawnedEnemy.isDead || !spawnReferences[i].currentSpawnedEnemy.gameObject.activeSelf){
							spawnReferences[i].RespawnEnemies(false);
							spawnedEnemy = true;
					}
				}else{
					spawnReferences[i].myManager = myEnemyReference.mySpawner.myManager;
					spawnReferences[i].gameObject.SetActive(true);
						spawnedEnemy = true;
				}
				}
			}
			}
		}
	}

	bool CanSpawn(){
		int numAvail = 0;
		if (foundSpawnReferences){
			for (int i = 0; i < externalSpawners.Count; i++){
				if (externalSpawners[i].enemySpawned){
					if (externalSpawners[i].currentSpawnedEnemy.isDead || !externalSpawners[i].currentSpawnedEnemy.gameObject.activeSelf){
						numAvail++;
					}
				}else{
					numAvail++;
				}
			}
		}else{
		for (int i = 0; i < spawnReferences.Length; i++){
			if (spawnReferences[i].enemySpawned){
				if (spawnReferences[i].currentSpawnedEnemy.isDead || !spawnReferences[i].currentSpawnedEnemy.gameObject.activeSelf){
					numAvail++;
				}
			}else{
				numAvail++;
			}
		}
		}
		return (numAvail > 0);
	}

	public void ResetSpawn(){

		if (foundSpawnReferences){
			for (int i = 0; i < externalSpawners.Count; i++){
				externalSpawners[i].Unspawn();
			}
		}else{
		for (int i = 0; i < spawnReferences.Length; i++){
			spawnReferences[i].Unspawn();
		}
		}
	}

	public void KillAll(){

		if (foundSpawnReferences){
			for (int i = 0; i < externalSpawners.Count; i++){
				externalSpawners[i].KillWithoutXP();
			}
		}else{
			for (int i = 0; i < spawnReferences.Length; i++){
			spawnReferences[i].KillWithoutXP();
		}
		}
	}

	public void FeatherAll(Color newCol){
		if (foundSpawnReferences){
			for (int i = 0; i < externalSpawners.Count; i++){
				externalSpawners[i].ChangeFeatherColor(newCol);
			}
		}else{
		for (int i = 0; i < spawnReferences.Length; i++){
			spawnReferences[i].ChangeFeatherColor(newCol);
		}
		}

	}

	public bool EnemiesAreActive(){
		bool activeEnemies = false;
		if (foundSpawnReferences){
			for (int i = 0; i < externalSpawners.Count; i++){
				if (externalSpawners[i].currentSpawnedEnemy != null){
					if (externalSpawners[i].currentSpawnedEnemy.gameObject.activeSelf && !externalSpawners[i].currentSpawnedEnemy.isDead){
						activeEnemies = true; 
					}
				}
			}
		}else{
		for (int i = 0; i < spawnReferences.Length; i++){
			if (spawnReferences[i].currentSpawnedEnemy != null){
				if (spawnReferences[i].currentSpawnedEnemy.gameObject.activeSelf && !spawnReferences[i].currentSpawnedEnemy.isDead){
					activeEnemies = true; 
				}
			}
		}
		}
		return activeEnemies;
	}
}
