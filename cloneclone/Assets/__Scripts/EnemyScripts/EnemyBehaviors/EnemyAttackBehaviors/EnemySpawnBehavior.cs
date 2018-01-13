using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnBehavior : EnemyBehaviorS {

	public bool stopEnemy = false;
	[Header("Behavior Duration")]
	public float behaviorDuration = -1f;
	private float behaviorCountdown;

	[Header("Spawn Variables")]
	public float spawnDelay = 0.8f;
	public float timeBetweenSpawns = 0.3f;
	public Transform[] spawnReferences;
	private List<Vector3> spawnPositions;
	public GameObject spawnObject;
	private GameObject currentSpawnObject;
	public bool parentSpawn = false;
	public bool singleTargetSpawns = false;
	private EnemyShooterS targetShooter;
	private EnemyShooterS shooterRef;

	private float currentSpawnDelay;
	private int currentSpawnStep;

	
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

		if (stopEnemy){
			myEnemyReference.myRigidbody.velocity = Vector3.zero;
		}
		myEnemyReference.AttackFlashEffect();

		if (spawnPositions == null){
			spawnPositions = new List<Vector3>();
			for (int i = 0; i < spawnReferences.Length; i++){
				spawnPositions.Add(spawnReferences[i].position-myEnemyReference.transform.position);
			}
		}

		currentSpawnDelay = spawnDelay;
		currentSpawnStep = 0;
		behaviorCountdown = behaviorDuration;
		
	}

	private void DoSpawns(){

		if (currentSpawnStep < spawnReferences.Length){
			currentSpawnDelay -= Time.deltaTime*currentDifficultyMult;
			if (currentSpawnDelay <= 0){
				currentSpawnDelay = timeBetweenSpawns;
				currentSpawnObject = 
					(GameObject)Instantiate(spawnObject, transform.position+spawnPositions[currentSpawnStep], Quaternion.identity);

				if (currentSpawnObject.GetComponent<EnemyShooterS>()){
					if (singleTargetSpawns){
						if (currentSpawnStep == 0){
							shooterRef = currentSpawnObject.GetComponent<EnemyShooterS>();
							targetShooter = shooterRef;
							shooterRef.SetTargetRef(null);
						}
						else{
							if (!targetShooter){
								targetShooter = shooterRef;
							}
							shooterRef = currentSpawnObject.GetComponent<EnemyShooterS>();
							shooterRef.SetTargetRef(targetShooter);
						}
					}else{
						shooterRef = currentSpawnObject.GetComponent<EnemyShooterS>();
					shooterRef.SetEnemy(myEnemyReference);
					}
				}
				if (parentSpawn){
					currentSpawnObject.transform.parent = spawnReferences[currentSpawnStep].parent;
				}
				currentSpawnStep++;
			}
		}

	}
	
	public override void StartAction (bool setAnimTrigger = true)
	{
		base.StartAction ();
		InitializeAction();

	}

	public override void SetSecondBehaviorStart(float difficultyMult, EnemyS enemyRef){
		base.SetSecondBehaviorStart(difficultyMult, enemyRef);
		InitializeAction();
	}
	
	public override void EndAction (bool doNextAction = true)
	{
		base.EndAction (doNextAction);
	}
}
