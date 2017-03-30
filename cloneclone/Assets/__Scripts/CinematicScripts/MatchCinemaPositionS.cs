using UnityEngine;
using System.Collections;

public class MatchCinemaPositionS : MonoBehaviour {

	public Transform targetPos;
	public EnemySpawnerS targetEnemy;

	void Start(){
		if (targetEnemy){
			transform.position = targetEnemy.currentSpawnedEnemy.transform.position;
		}else if (targetPos){
			transform.position = targetPos.transform.position;
		}
	}
}
