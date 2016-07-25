using UnityEngine;
using System.Collections;

public class CameraPOIS : MonoBehaviour {

	private PlayerController playerReference;
	private EnemyDetectS enemyReference;

	public float playerWeight = 1f;
	public float enemyWeight = 0.5f;

	public float moveEasing = 0.1f;

	private Vector3 currentPosition;

	// Use this for initialization
	void Start () {

		playerReference = GetComponentInParent<PlayerController>();
		transform.parent = null;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!enemyReference){
			enemyReference = playerReference.myDetect;
		}else{
			
			
			Vector3 newPos = Vector3.zero;
			if (enemyReference.closestEnemy != null && !playerReference.myStats.PlayerIsDead()){

				currentPosition = (playerReference.transform.position*playerWeight + enemyReference.enemyCenterpoint*enemyWeight)/
					(playerWeight+enemyWeight);

				newPos.x = (1-moveEasing)*transform.position.x + moveEasing*currentPosition.x;
				newPos.y = (1-moveEasing)*transform.position.y + moveEasing*currentPosition.y;

			}else{
				newPos = currentPosition = playerReference.transform.position;
			}


			transform.position = newPos;

		}

	}
}
