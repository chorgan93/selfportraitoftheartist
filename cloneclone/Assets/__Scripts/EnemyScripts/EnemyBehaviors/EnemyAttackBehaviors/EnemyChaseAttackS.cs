using UnityEngine;
using System.Collections;

public class EnemyChaseAttackS : EnemyBehaviorS {

	public GameObject attackPrefab;
	private GameObject currentAttackPrefab;
	private EnemyProjectileS currentAttackS;
	
	[Header("Behavior Duration")]
	public float chaseTimeFixed = -1f;
	public float chaseTimeMin;
	public float chaseTimeMax;
	public PlayerDetectS rangeCheck;
	
	[Header("Movement Variables")]
	public float chaseDragAmt = -1f;
	public float chaseSpeedFixed = -1f;
	public float chaseSpeedMin;
	public float chaseSpeedMax;
	public float recenterMin;
	public float recenterMax;
	private Vector3 recenterTarget;
	private float recenterCountdown;

	[Header("Double Action Properties")]
	public EnemySpawnBehavior secondarySpawnBehavior;
	
	private float currentchaseSpeed;
	
	private float chaseTimeCountdown;

	private bool didWallRedirect = false;
	private float preventRedirectTime = 0.3f;
	private float preventRedirectCountdown = 0f;
	private bool redirecting = false;
	private float redirectTime = 0.4f;
	private float redirectCountdown;
	private Vector3 redirectTarget;

	private bool initialFace;

	
	// Update is called once per frame
	void FixedUpdate () {

		BehaviorUpdate();
		
		if (BehaviorActing()){
			
			
			DoMovement();
			
			chaseTimeCountdown -= Time.deltaTime;
			
		
			
			if (chaseTimeCountdown <= 0){
				EndAction();
			}
		}
		
	}
	
	private void InitializeAction(){


		initialFace = facePlayer;
		if (AttackInRange()){
			preventRedirectCountdown = preventRedirectTime;
			didWallRedirect = false;
			redirecting = false;
			myEnemyReference.myAnimator.SetTrigger(animationKey);
			if (signalObj != null){
				Vector3 signalPos =  transform.position;
				signalPos.z = transform.position.z+1f;
				GameObject signal = Instantiate(signalObj, signalPos, Quaternion.identity)
					as GameObject;
				signal.transform.parent = myEnemyReference.transform;
			}
			myEnemyReference.AttackFlashEffect();
		
		if (chaseTimeFixed > 0){
			chaseTimeCountdown = chaseTimeFixed;
		}
		else{
			chaseTimeCountdown = Random.Range(chaseTimeMin, chaseTimeMax);
		}
		
		if (chaseSpeedFixed > 0){
			currentchaseSpeed = chaseSpeedFixed;
		}
		else{
			currentchaseSpeed = Random.Range(chaseSpeedMin, chaseSpeedMax);
		}

		if (recenterMin > 0){
			recenterTarget = myEnemyReference.GetTargetReference().transform.position;
			recenterCountdown = Random.Range(recenterMin,recenterMax);
		}

		currentAttackPrefab = Instantiate(attackPrefab, transform.position, Quaternion.identity)
			as GameObject;
		currentAttackS = currentAttackPrefab.GetComponent<EnemyProjectileS>();
		currentAttackS.AllowMultiHit();
		currentAttackS.Fire((myEnemyReference.GetTargetReference().transform.position
		                     -transform.position).normalized, myEnemyReference);
		currentAttackPrefab.transform.localPosition = Vector3.zero;
		
		if (chaseDragAmt > 0){
			myEnemyReference.myRigidbody.drag = chaseDragAmt;
		}

			if (secondarySpawnBehavior){
				secondarySpawnBehavior.SetSecondBehaviorStart(currentDifficultyMult, myEnemyReference);
			}
		}
		else{
			myEnemyReference.myAnimator.SetTrigger("Idle");
			EndAction();
		}

		
	}

	private bool AttackInRange(){
		
		bool canContinue = true;
		
		if (rangeCheck != null){
			rangeCheck.FindTarget();
			if (!rangeCheck.PlayerInRange()){
				canContinue = false;
			}
		}
		
		return canContinue;
		
	}
	
	private void DoMovement(){

		if (myEnemyReference.hitWall && !didWallRedirect && preventRedirectCountdown <= 0f){
			WallRedirect();
		}

		if (!myEnemyReference.hitStunned){
			preventRedirectCountdown -= Time.deltaTime;
			if (redirecting){
				redirectCountdown -= Time.deltaTime;
				if (redirectCountdown <= 0){
					redirecting = false;
					myEnemyReference.SetFaceStatus(initialFace);
					didWallRedirect = false;
				}
				myEnemyReference.myRigidbody.AddForce((redirectTarget
					-transform.position).normalized*currentchaseSpeed*currentDifficultyMult*Time.deltaTime);
			}else{
			if (recenterMin > 0){
				recenterCountdown -= Time.deltaTime;
				if (recenterCountdown <= 0){
					recenterTarget = myEnemyReference.GetTargetReference().transform.position;
					recenterCountdown = Random.Range(recenterMin,recenterMax);
					didWallRedirect = false;
				}
				myEnemyReference.myRigidbody.AddForce((recenterTarget
					-transform.position).normalized*currentchaseSpeed*currentDifficultyMult*Time.deltaTime);


			}else{
			myEnemyReference.myRigidbody.AddForce((myEnemyReference.GetTargetReference().transform.position
					-transform.position).normalized*currentchaseSpeed*currentDifficultyMult*Time.deltaTime);
			}
		}
		}
		
	}
	
	public override void StartAction (bool setAnimTrigger = true)
	{
		base.StartAction (false);
		InitializeAction();

	}
	
	public override void EndAction (bool doNextAction = true)
	{
		if (currentAttackPrefab){
			Destroy(currentAttackPrefab);
		}
		if (secondarySpawnBehavior){
			secondarySpawnBehavior.EndAction(false);
		}
		base.EndAction (doNextAction);
	}

	public override void CancelAction(){
		if (currentAttackPrefab){
			Destroy(currentAttackPrefab);
		}
		if (secondarySpawnBehavior){
			secondarySpawnBehavior.EndAction(false);
		}
		base.CancelAction ();
	}

	void WallRedirect(){
		redirectTarget = Vector3.zero;
		float targetDistance = (recenterTarget-transform.position).magnitude;
		redirectTarget = Quaternion.Euler(0,0,180f)*(recenterTarget-transform.position).normalized;
		redirectTarget*=targetDistance;
		redirectTarget.z = transform.position.z;
		didWallRedirect =  true;
		redirecting = true;
		redirectCountdown = redirectTime;
		myEnemyReference.SetFaceStatus(false);
	}
}
