using UnityEngine;
using System.Collections;

public class SpawnOnProjectileS : MonoBehaviour {

	public float spawnRate;
	private float spawnRateCountdown;
	public float firstSpawnDelay = 0f;
	public int maxSpawns = -1;
	private bool infiniteSpawn = true;

	public GameObject spawnObject;
	public float spawnObjectRadius = 2.5f;
	public float spawnRadiusAdd = 0f;
	public float spawnObjZ = 0f;

	private Vector3 spawnPos;

	public bool chargeSpawner = false;
	private PlayerController playerRef;

	private bool firstSpawned = false;

	// Use this for initialization
	void Start () {

		spawnRateCountdown = firstSpawnDelay;
		if (maxSpawns > 0){
			infiniteSpawn = false;
		}
		if (chargeSpawner){
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
				GameObject newSpawn = Instantiate(spawnObject, spawnPos, spawnObject.transform.rotation)
					as GameObject;

				if (chargeSpawner){
					newSpawn.GetComponent<ChargeAttackS>().SetPlayer(playerRef);
					if (!firstSpawned){
						newSpawn.GetComponent<ChargeAttackS>().SetFirstSpawned();
					}
				}
				firstSpawned = true;
			}
		}
	
	}
}
