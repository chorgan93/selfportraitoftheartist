using UnityEngine;
using System.Collections;

public class EnemyFadesS : MonoBehaviour {

	private const float Y_SPAWN_MULT = 0.3f; // to help w/ perspective

	public float spawnRateMax = 0.1f;
	public float spawnRateMin = 0.4f;
	private float spawnRateCountdown;

	public float spawnRadiusMin = 0.1f;
	public float spawnRadiusMax = 1f;

	public GameObject spawnPrefab;
	private EnemyS myEnemy;
	private SpriteRenderer enemyRenderer;

	Vector3 spawnPos;

	// Use this for initialization
	void Start () {

		myEnemy = GetComponent<EnemyS>();
		enemyRenderer = myEnemy.myRenderer;

		spawnRateCountdown = FindSpawnValue(spawnRateMin, spawnRateMax);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!myEnemy.isDead){
		spawnRateCountdown -= Time.deltaTime;
		if (spawnRateCountdown <= 0){
			spawnPos = transform.position;
				Vector3 randomMult = Random.insideUnitSphere;
				randomMult.y *= Y_SPAWN_MULT;
			spawnPos += FindSpawnValue(spawnRadiusMin,spawnRadiusMax)*randomMult;
			spawnPos.z = transform.position.z + 0.5f;
			GameObject newSpawn = Instantiate(spawnPrefab, spawnPos, Quaternion.identity) as GameObject;
			newSpawn.transform.localScale = enemyRenderer.transform.localScale.x*myEnemy.transform.localScale;
			newSpawn.GetComponent<SpriteRenderer>().sprite = enemyRenderer.sprite;
			spawnRateCountdown = FindSpawnValue(spawnRateMin, spawnRateMax);
		}
		}
	
	}

	float FindSpawnValue(float min, float max){
		return (min + (max-min)*(1f-(myEnemy.currentHealth-1f)/myEnemy.maxHealth));
	}
}
