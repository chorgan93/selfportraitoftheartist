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

	//private float subtleLookAmt = 1f;

	public static CameraPOIS POI;

	void Awake(){
		if (POI != null){
			Destroy(gameObject);
		}else{
			POI = this; 
		}
	}

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

			// experiment - was pretty nauseating
			//lookVector.x = controlRef.RightHorizontal() + controlRef.Horizontal()*subtleLookAmt;
			//lookVector.y = controlRef.RightVertical() + controlRef.Vertical()*subtleLookAmt;

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

				if (!playerReference.myLockOn.lockedOn){
					newPos.x = (1-moveEasing)*(transform.position.x-lookVector.x) + moveEasing*currentPosition.x;
					newPos.y = (1-moveEasing)*(transform.position.y-lookVector.y) + moveEasing*currentPosition.y;
				}else{
					newPos.x = (1-moveEasing)*(transform.position.x) + moveEasing*currentPosition.x;
					newPos.y = (1-moveEasing)*(transform.position.y) + moveEasing*currentPosition.y;
				}

			}else{
				newPos = currentPosition = playerReference.transform.position;
			}

			if (!playerReference.myLockOn.lockedOn){
				newPos += lookVector*lookAmt;
			}

			transform.position = newPos;

		}

	}

	public void JumpToPoint(Vector3 newPoint){
		Vector3 newPos = newPoint;
		newPos.z = transform.position.z;
		transform.position = newPos;
		CameraFollowS.F.CutTo(transform.position);
	}
	public void JumpToMidpoint(Vector3 positionA, Vector3 positionB){
		Vector3 newPos = (positionA+positionB)/2f;
		newPos.z = transform.position.z;
		transform.position = newPos;
		CameraFollowS.F.CutTo(transform.position);
	}
}
