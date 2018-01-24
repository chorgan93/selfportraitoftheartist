using UnityEngine;
using System.Collections;

public class CameraPOIS : MonoBehaviour {

	private PlayerController playerReference;
	private ControlManagerS controlRef;
	private EnemyDetectS enemyReference;

	public float playerWeight = 1f;
	private float sprintingWeightMult = 1.5f;
	private float currentSprintMult = 1f;
	public float enemyWeight = 0.5f;

	public float moveEasing = 0.1f;

	[HideInInspector]
	public Vector3 poiOffset = Vector3.zero;

	private Vector3 lookVector;
	private float lookAmt = 1f;
	private float lookWeight = 1f;

	private Vector3 currentPosition;
	private bool CAMERA_LOCK = false;

	private Vector3 newPos;

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

	#if UNITY_EDITOR_OSX || UNITY_EDITOR || UNITY_EDITOR_64
	void Update(){
		CheckLock();
	}
	#endif
	
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

			/*lookVector.x = controlRef.RightHorizontal();
			lookVector.y = controlRef.RightVertical();
			lookVector.z = 0;**/

			currentSprintMult = 1f;

			newPos = Vector3.zero;
			if (enemyReference.closestEnemy != null && !playerReference.myStats.PlayerIsDead()){

				if (playerReference.myLockOn.lockedOn){
					currentPosition = (playerReference.transform.position*playerWeight 
					                   + playerReference.myLockOn.myEnemy.transform.position*enemyWeight)/
						(playerWeight+enemyWeight);
				}/*else if (playerReference.isSprinting){
					currentPosition = (playerReference.SprintProjection()*playerWeight 
						+ enemyReference.enemyCenterpoint*enemyWeight)/
						(playerWeight+enemyWeight);
				}**/
				else{
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
				/*if (playerReference.isSprinting){
					currentPosition = playerReference.SprintProjection();
					newPos.x = (1-moveEasing)*(transform.position.x) + moveEasing*currentPosition.x;
					newPos.y = (1-moveEasing)*(transform.position.y) + moveEasing*currentPosition.y;
				}else{**/
					currentPosition = newPos = playerReference.transform.position;
				//}
			}

			if (!playerReference.myLockOn.lockedOn){
				newPos += lookVector*lookAmt;
			}
			newPos += poiOffset;

			if (!CAMERA_LOCK){
				transform.position = newPos;
			}

		}

	}

	void CheckLock(){

		if (Input.GetKeyDown(KeyCode.Alpha8)){
			CAMERA_LOCK = !CAMERA_LOCK;
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

	public void SetOffset(Vector3 newOff){
		poiOffset = newOff;
	}

	public void ResetOffset(){
		poiOffset = Vector3.zero;
	}
}
