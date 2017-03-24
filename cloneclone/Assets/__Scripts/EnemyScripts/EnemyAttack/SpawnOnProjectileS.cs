using UnityEngine;
using System.Collections;

public class SpawnOnProjectileS : MonoBehaviour {

	public float spawnRate;
	private float spawnRateCountdown;
	public float firstSpawnDelay = 0f;
	public int maxSpawns = -1;
	private bool infiniteSpawn = true;

	public GameObject spawnObject;
	public GameObject[] spawnObjects;
	private int currentSpawn = 0;
	public float spawnObjectRadius = 2.5f;
	public float spawnRadiusAdd = 0f;
	public float spawnObjZ = 0f;

	private Vector3 spawnPos;

	public bool chargeSpawner = false;
	private PlayerController playerRef;
	public bool turnOffStun = false;

	public float spawnOnHitEnemyDelay = -1f;
	public bool spawnOnHitEnemies = false;
	private bool spawnedOnHitEnemies = false;
	public GameObject spawnObjectOnHitEnemies;

	private bool firstSpawned = false;

	// Use this for initialization
	void Start () {

		spawnRateCountdown = firstSpawnDelay;
		if (maxSpawns > 0){
			infiniteSpawn = false;
		}
		if (chargeSpawner || spawnOnHitEnemies){
			playerRef = GetComponent<ProjectileS>().myPlayer;
		}
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (infiniteSpawn || (!infiniteSpawn && maxSpawns > 0)){
			spawnRateCountdown -= Time.deltaTime;
			if (spawnRateCountdown <= 0){
				spawnRateCountdown = spawnRate;
				if (!infiniteSpawn){
					maxSpawns --;
				}

				spawnPos = transform.position;
				spawnPos += Random.insideUnitSphere*spawnObjectRadius;
				spawnObjectRadius+=spawnRadiusAdd;
				spawnPos.z = spawnObjZ;
				GameObject newSpawn;

				if (spawnObject != null){
					newSpawn = Instantiate(spawnObject, spawnPos, spawnObject.transform.rotation)
						as GameObject;
				}else{
					newSpawn = Instantiate(spawnObjects[currentSpawn], spawnPos, spawnObjects[currentSpawn].transform.rotation)
						as GameObject;
					currentSpawn++;
				}

				if (chargeSpawner){
					newSpawn.GetComponent<ChargeAttackS>().SetPlayer(playerRef);
					if (!firstSpawned){
						newSpawn.GetComponent<ChargeAttackS>().SetFirstSpawned();
					}
					if (turnOffStun){
						newSpawn.GetComponent<ChargeAttackS>().TurnOffStun();
					}
				}
				firstSpawned = true;
			}
		}

		if (spawnOnHitEnemies && !spawnedOnHitEnemies){
			spawnOnHitEnemyDelay -= Time.deltaTime;
			if (spawnOnHitEnemyDelay <= 0){
			if (playerRef){
				for (int i = 0; i < playerRef.enemiesHitByAttackRef.Count; i++){
					spawnPos = playerRef.enemiesHitByAttackRef[i].transform.position;
					spawnPos += Random.insideUnitSphere*spawnObjectRadius;
					spawnObjectRadius+=spawnRadiusAdd;
					spawnPos.z = spawnObjZ;
						GameObject newSpawn = Instantiate(spawnObjectOnHitEnemies, spawnPos, spawnObjectOnHitEnemies.transform.rotation)
						as GameObject;
					
					if (chargeSpawner){
						newSpawn.GetComponent<ChargeAttackS>().SetPlayer(playerRef);
						if (!firstSpawned){
							newSpawn.GetComponent<ChargeAttackS>().SetFirstSpawned();
						}
						if (turnOffStun){
							newSpawn.GetComponent<ChargeAttackS>().TurnOffStun();
						}
					}
				}
			}
				spawnedOnHitEnemies = true;
			}
		}
	
	}
}
