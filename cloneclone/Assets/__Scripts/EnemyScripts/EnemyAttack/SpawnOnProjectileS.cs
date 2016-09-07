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
	public float spawnObjZ = 0f;

	private Vector3 spawnPos;

	// Use this for initialization
	void Start () {

		spawnRateCountdown = firstSpawnDelay;
		if (maxSpawns > 0){
			infiniteSpawn = false;
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
				spawnPos.z = spawnObjZ;
				Instantiate(spawnObject, spawnPos, Quaternion.identity);
			}
		}
	
	}
}
