using UnityEngine;
using System.Collections;

public class EnemyLaserBuddyS : EnemyBuddyS {

	[Header ("Animation Properties")]
	public string shootAnimationTrigger;

	[Header ("Timing Properties")]
	public float actionTime;
	private float actionCount;
	public float trackingTime;
	public float shootTime;
	private Vector3 trackedTarget;
	private bool setTarget = false;
	private bool shotLaser = false;
	private bool isShooting = false;

	[Header ("AttackPrefabs")]
	public GameObject laserPrefab;
	public GameObject muzzleFlarePrefab;
	public SpriteRenderer targetingVisual;
	private Color targetingCol;
	private float maxTargetCol;
	private float targetingT;
	public float visualRange;
	private Vector3 targetingObjPos;
	public float muzzleRange = 1f;
	public float accuracyMult = 5f;

	private Vector3 currentTargetDir;

	// Use this for initialization
	void Start () {
		Initialize();
		//visualRange = targetingVisual.transform.localScale.x/2f;
		targetingCol = targetingVisual.color;
		maxTargetCol = targetingCol.a;
		targetingCol.a = 0f;
		targetingVisual.color = targetingCol;
		targetingVisual.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!isShooting){
			FollowEnemy();
			FaceDirection();
		}
		HandleShooting();
	}

	void HandleShooting(){
		if (isShooting){
			actionCount += Time.deltaTime;
			if (actionCount >= actionTime/currentDifficultyMult){
				isShooting = false;
				EndAction();
			}


			if (actionCount >= trackingTime/currentDifficultyMult && !setTarget){
				trackedTarget = enemyRef.GetTargetReference().position;
				trackedTarget.z = transform.position.z;
				setTarget = true;
				FaceTarget();

			}

			/*if (actionCount < shootTime/currentDifficultyMult){
				SetTargetingVisual();
			}**/

			if (actionCount >= shootTime/currentDifficultyMult && !shotLaser){
				shotLaser = true;

				targetingVisual.gameObject.SetActive(false);

				Vector3 spawnMuzzlePos = transform.position;
				spawnMuzzlePos += (trackedTarget-transform.position).normalized*muzzleRange;
				spawnMuzzlePos.z = transform.position.z-1;
				Instantiate(muzzleFlarePrefab, spawnMuzzlePos, Quaternion.identity);

				GameObject newLaser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
				newLaser.transform.Rotate(RotateToTarget());
				newLaser.transform.position += newLaser.transform.localScale.x/2f*newLaser.transform.right;

				EnemyProjectileS laserProj = newLaser.GetComponent<EnemyProjectileS>();
				laserProj.Fire(Vector3.zero, enemyRef);
			}
		}
	}

	void FaceTarget(){
		Vector3 faceScale = transform.localScale;
		if (transform.position.x > trackedTarget.x){
			faceScale.x = sScale;
		}

		if (transform.position.x < trackedTarget.x){
			faceScale.x = -sScale;
		}
		transform.localScale = faceScale;
	}

	void SetTargetingVisual(){

		// set position
		if (!setTarget){
			currentTargetDir = enemyRef.GetTargetReference().position-transform.position;
		}else{
			currentTargetDir = trackedTarget-transform.position;
		}
		currentTargetDir = currentTargetDir.normalized;

		targetingObjPos = visualRange*currentTargetDir;
		targetingObjPos.z = transform.position.z+2f;
		targetingVisual.transform.localPosition = targetingObjPos;
		targetingVisual.transform.rotation = Quaternion.Euler(RotateToTarget(currentTargetDir));

		// set color
		targetingT = actionTime/(shootTime/currentDifficultyMult);
		targetingT = Mathf.Sin(targetingT * Mathf.PI * 0.5f);
		targetingCol.a = Mathf.Lerp(0f, maxTargetCol, targetingT);
		targetingVisual.color = targetingCol;
	}

	public override void TriggerAction(){

		if (!buddyDoingAction && enemyRef.GetTargetReference() != null){
			base.TriggerAction();

			actionCount = 0f;
			myAnimator.SetTrigger(shootAnimationTrigger);
			shotLaser = false;
			isShooting = true;
			setTarget = false;

			SetTargetingVisual();
			//targetingVisual.gameObject.SetActive(true);
		}

	}

	private Vector3 RotateToTarget(Vector3 setTarget){
		Vector3 rotateEuler = Vector3.zero;

		Vector3 targetDir = setTarget.normalized;

		float rotateZ = 0;

		if(targetDir.x == 0){
			if (targetDir.y > 0){
				rotateZ = 90;
			}
			else{
				rotateZ = -90;
			}
		}
		else{
			rotateZ = Mathf.Rad2Deg*Mathf.Atan((targetDir.y/targetDir.x));
		}	


		if (targetDir.x < 0){
			rotateZ += 180;
		}

		rotateZ += accuracyMult*Random.insideUnitCircle.x;


		rotateEuler.z = rotateZ;

		return rotateEuler;
	}

	private Vector3 RotateToTarget(){

		Vector3 rotateEuler = Vector3.zero;

		Vector3 targetDir = (trackedTarget-transform.position).normalized;

		float rotateZ = 0;

		if(targetDir.x == 0){
			if (targetDir.y > 0){
				rotateZ = 90;
			}
			else{
				rotateZ = -90;
			}
		}
		else{
			rotateZ = Mathf.Rad2Deg*Mathf.Atan((targetDir.y/targetDir.x));
		}	


		if (targetDir.x < 0){
			rotateZ += 180;
		}

		rotateZ += accuracyMult*Random.insideUnitCircle.x;


		rotateEuler.z = rotateZ;

		return rotateEuler;
	}
}
