using UnityEngine;
using System.Collections;

public class MatchOnCombatActivate : MonoBehaviour {

	public Transform targetPos;
	public EnemySpawnerS targetEnemy;
	public Vector3 offsetPos = Vector3.zero;

	public void MatchTargetPos(){
		if (targetEnemy){
			transform.position = targetEnemy.currentSpawnedEnemy.transform.position+offsetPos;
		}else if (targetPos){
			transform.position = targetPos.transform.position+offsetPos;
		}else{
			transform.position = transform.position+offsetPos;
		}
	}
}
