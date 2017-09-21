using UnityEngine;
using System.Collections;

public class EnemySpawnEnemyBehavior : EnemyBehaviorS {

	public bool stopEnemy = false;
	[Header("Behavior Duration")]
	public float behaviorDuration = -1f;
	private float behaviorCountdown;

	[Header("Spawn Variables")]
	public float spawnDelay = 0.8f;
	public float timeBetweenSpawns = 0.3f;
	public EnemySpawnerS[] spawnReferences;
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
			for (int i = 0; i < spawnReferences.Length; i++){
				if (!spawnedEnemy){
				if (spawnReferences[i].enemySpawned){
					if (spawnReferences[i].currentSpawnedEnemy.isDead){
							spawnReferences[i].RespawnEnemies(false);
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

	bool CanSpawn(){
		int numAvail = 0;
		for (int i = 0; i < spawnReferences.Length; i++){
			if (spawnReferences[i].enemySpawned){
				if (spawnReferences[i].currentSpawnedEnemy.isDead){
					numAvail++;
				}
			}else{
				numAvail++;
			}
		}
		return (numAvail > 0);
	}

	public void ResetSpawn(){
		for (int i = 0; i < spawnReferences.Length; i++){
			spawnReferences[i].Unspawn();
		}
	}

	public void KillAll(){
		for (int i = 0; i < spawnReferences.Length; i++){
			spawnReferences[i].KillWithoutXP();
		}
	}

	public bool EnemiesAreActive(){
		bool activeEnemies = false;
		for (int i = 0; i < spawnReferences.Length; i++){
			if (spawnReferences[i].currentSpawnedEnemy != null){
				if (spawnReferences[i].currentSpawnedEnemy.gameObject.activeSelf && !spawnReferences[i].currentSpawnedEnemy.isDead){
					activeEnemies = true; 
				}
			}
		}
		return activeEnemies;
	}
}
