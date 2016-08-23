using UnityEngine;
using System.Collections;

public class EnemySpawnerS : MonoBehaviour {

	private bool _enemySpawned = false;
	private EnemyS currentEnemyReference;

	public float enemySpawnDelay = 0f;
	public float chanceToSpawn = 1f;
	private bool didNotSpawnEnemy = false;

	public GameObject[] enemyPool;

	//private RoomClearCheck parentClear;
	private InfinitySpawnS parentClear;
	private bool sentClearMessage = false;

	// Use this for initialization
	void Start () {

		//parentClear = GetComponentInParent<RoomClearCheck>();
		parentClear = GetComponentInParent<InfinitySpawnS>();

		if (enemySpawnDelay <= 0){
			SpawnEnemy();
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!_enemySpawned){
			enemySpawnDelay -= Time.deltaTime;
			if (enemySpawnDelay <= 0){
				SpawnEnemy();
			}
		}

		if (EnemiesDefeated() && !sentClearMessage){
			sentClearMessage = true;
			parentClear.AddClear();
		}
	
	}

	private void SpawnEnemy(){

		float chanceEnemySpawns = Random.Range(0f, 1f);

		if (chanceEnemySpawns <= chanceToSpawn){
	
			int enemyToSpawn = Mathf.RoundToInt(Random.Range(0, enemyPool.Length));
	
			GameObject newEnemy = Instantiate(enemyPool[enemyToSpawn], transform.position, Quaternion.identity)
				as GameObject;
	
			currentEnemyReference = newEnemy.GetComponent<EnemyS>();
	
			newEnemy.transform.parent = transform;

		}else{
			didNotSpawnEnemy = true;
		}
		
		
		_enemySpawned = true;

	}

	public bool EnemiesDefeated(){

		if (didNotSpawnEnemy){
			return true;
		}
		else{
			if (currentEnemyReference != null){
				return (currentEnemyReference.isDead);
			}
			else{
				return false;
			}
		}

	}
}
