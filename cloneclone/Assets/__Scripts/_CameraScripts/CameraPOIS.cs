using UnityEngine;
using System.Collections;

public class CameraPOIS : MonoBehaviour {

	private PlayerController playerReference;
	private ControlManagerS controlRef;
	private EnemyDetectS enemyReference;

	public float playerWeight = 1f;
	public float enemyWeight = 0.5f;

	public float moveEasing = 0.1f;

	private Vector3 lookVector;
	private float lookAmt = 1f;
	private float lookWeight = 1f;

	private Vector3 currentPosition;

	// Use this for initialization
	void Start () {

		playerReference = GetComponentInParent<PlayerController>();
		controlRef = playerReference.myControl;
		transform.parent = null;

		lookVector = Vector3.zero;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!enemyReference){
			enemyReference = playerReference.myDetect;
		}else{
			
			if (!controlRef){
				controlRef = playerReference.myControl;
			}
			
			lookVector.x = controlRef.RightHorizontal();
			lookVector.y = controlRef.RightVertical();
			lookVector.z = 0;

			
			Vector3 newPos = Vector3.zero;
			if (enemyReference.closestEnemy != null && !playerReference.myStats.PlayerIsDead()){

				if (playerReference.myLockOn.lockedOn){
					currentPosition = (playerReference.transform.position*playerWeight 
					                   + playerReference.myLockOn.myEnemy.transform.position*enemyWeight)/
						(playerWeight+enemyWeight);
				}else{
					currentPosition = (playerReference.transform.position*playerWeight 
					                   + enemyReference.enemyCenterpoint*enemyWeight)/
					(playerWeight+enemyWeight);
				}

				newPos.x = (1-moveEasing)*(transform.position.x-lookVector.x) + moveEasing*currentPosition.x;
				newPos.y = (1-moveEasing)*(transform.position.y-lookVector.y) + moveEasing*currentPosition.y;

			}else{
				newPos = currentPosition = playerReference.transform.position;
			}

			newPos += lookVector*lookAmt;

			transform.position = newPos;

		}

	}
}
