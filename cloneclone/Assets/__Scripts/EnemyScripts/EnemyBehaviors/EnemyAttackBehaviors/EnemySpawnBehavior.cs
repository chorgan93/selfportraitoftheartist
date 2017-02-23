using UnityEngine;
using System.Collections;

public class EnemySpawnBehavior : EnemyBehaviorS {

	public bool stopEnemy = false;
	[Header("Behavior Duration")]
	public float behaviorDuration = -1f;
	private float behaviorCountdown;

	[Header("Spawn Variables")]
	public float spawnDelay = 0.8f;
	public float timeBetweenSpawns = 0.3f;
	public Transform[] spawnReferences;
	public GameObject spawnObject;
	private GameObject currentSpawnObject;
	public bool parentSpawn = false;

	private float currentSpawnDelay;
	private int currentSpawnStep;

	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (BehaviorActing()){

			BehaviorUpdate();

			DoSpawns();

			behaviorCountdown -= Time.deltaTime;
			if (behaviorCountdown <= 0){
				EndAction();
			}
		}
		
	}
	
	private void InitializeAction(){

		if (stopEnemy){
			myEnemyReference.myRigidbody.velocity = Vector3.zero;
		}

		currentSpawnDelay = spawnDelay;
		currentSpawnStep = 0;
		behaviorCountdown = behaviorDuration;
		
	}

	private void DoSpawns(){

		if (currentSpawnStep < spawnReferences.Length){
			currentSpawnDelay -= Time.deltaTime;
			if (currentSpawnDelay <= 0){
				currentSpawnDelay = timeBetweenSpawns;
				currentSpawnObject = 
					(GameObject)Instantiate(spawnObject, spawnReferences[currentSpawnStep].position, Quaternion.identity);
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
	
	public override void EndAction (bool doNextAction = true)
	{
		base.EndAction (doNextAction);
	}
}
