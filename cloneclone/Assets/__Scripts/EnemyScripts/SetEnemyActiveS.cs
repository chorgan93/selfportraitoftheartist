using UnityEngine;
using System.Collections;

public class SetEnemyActiveS : MonoBehaviour {

	public bool setEnemyActive = false;
	public EnemySpawnerS enemySpawnerToTarget;

	// Use this for initialization
	void Start () {

		if (enemySpawnerToTarget){
			enemySpawnerToTarget.currentSpawnedEnemy.SetActiveState(setEnemyActive);
		}
		Destroy(gameObject);
	
	}

}
