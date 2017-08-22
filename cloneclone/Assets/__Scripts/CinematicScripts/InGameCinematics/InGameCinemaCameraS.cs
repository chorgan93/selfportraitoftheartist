using UnityEngine;
using System.Collections;

public class InGameCinemaCameraS : MonoBehaviour {

	public float moveTime = -1;
	public int myCinemaStep = 0;
	public GameObject newPoi;
	public EnemySpawnerS enemyPoi;

	// Use this for initialization
	void Start () {
	
		if (newPoi != null){
		CameraFollowS.F.SetNewPOI(newPoi);
		}else if (enemyPoi != null){
			if (enemyPoi.currentSpawnedEnemy != null){
				CameraFollowS.F.SetNewPOI(enemyPoi.currentSpawnedEnemy.gameObject);
			}else{
				CameraFollowS.F.ResetPOI();
			}
		}
		else{
			CameraFollowS.F.ResetPOI();
		}

	}
}
