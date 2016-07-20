using UnityEngine;
using System.Collections;

public class EnemySpawnBehavior : EnemyBehaviorS {

	[Header("Behavior Duration")]
	public float behaviorDuration = -1f;
	private float behaviorCountdown;

	[Header("Spawn Variables")]
	public float spawnDelay = 0.8f;
	public float timeBetweenSpawns = 0.3f;
	public Transform[] spawnReferences;
	public GameObject spawnObject;

	private float currentSpawnDelay;
	private int currentSpawnStep;

	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (BehaviorActing()){

			DoSpawns();

			behaviorCountdown -= Time.deltaTime;
			if (behaviorCountdown <= 0){
				EndAction();
			}
		}
		
	}
	
	private void InitializeAction(){
		
		currentSpawnDelay = spawnDelay;
		currentSpawnStep = 0;
		behaviorCountdown = behaviorDuration;
		
	}

	private void DoSpawns(){

		if (currentSpawnStep < spawnReferences.Length){
			currentSpawnDelay -= Time.deltaTime;
			if (currentSpawnDelay <= 0){
				currentSpawnDelay = timeBetweenSpawns;
				Instantiate(spawnObject, spawnReferences[currentSpawnStep].position, Quaternion.identity);
				currentSpawnStep++;
			}
		}

	}
	
	public override void StartAction ()
	{
		base.StartAction ();
		
		InitializeAction();
	}
	
	public override void EndAction ()
	{
		base.EndAction ();
	}
}
