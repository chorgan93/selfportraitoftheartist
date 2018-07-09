using UnityEngine;
using System.Collections;

public class MatchCinemaPositionS : MonoBehaviour {

	public Transform targetPos;
	public EnemySpawnerS targetEnemy;
	public Vector3 offsetPos = Vector3.zero;

	public bool onlyMatchOnCall = false;
    public bool matchEnemyFace = false;

	void Awake(){
		if (!onlyMatchOnCall){
		if (targetEnemy){
			transform.position = targetEnemy.currentSpawnedEnemy.transform.position+offsetPos;
                if (matchEnemyFace){
                    Vector3 matchScale = transform.localScale;
                    if (targetEnemy.currentSpawnedEnemy.transform.localScale.x < 0){
                        matchScale.x *= -1f;
                        transform.localScale = matchScale;
                    }
                }
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
