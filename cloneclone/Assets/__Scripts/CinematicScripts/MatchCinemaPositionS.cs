using UnityEngine;
using System.Collections;

public class MatchCinemaPositionS : MonoBehaviour {

	public Transform targetPos;
	public EnemySpawnerS targetEnemy;
	public Vector3 offsetPos = Vector3.zero;

	public bool onlyMatchOnCall = false;

	void Awake(){
		if (!onlyMatchOnCall){
		if (targetEnemy){
			transform.position = targetEnemy.currentSpawnedEnemy.transform.position+offsetPos;
		}else if (targetPos){
			transform.position = targetPos.transform.position+offsetPos;
		}
		}
	}

	public Vector3 GetTargetPos(){
		if (targetEnemy){
			return targetEnemy.currentSpawnedEnemy.transform.position+offsetPos;
		}else if (targetPos){
			return targetPos.transform.position+offsetPos;
		}else{
			return transform.position+offsetPos;
		}
	}
}
