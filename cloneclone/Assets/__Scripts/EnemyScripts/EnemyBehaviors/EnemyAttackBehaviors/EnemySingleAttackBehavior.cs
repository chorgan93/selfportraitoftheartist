using UnityEngine;
using System.Collections;

public class EnemySingleAttackBehavior : EnemyBehaviorS {

	public PlayerDetectS rangeCheck;
	public EnemyDodgeBehaviorS dodgeCheck;
	private bool doDodge = false;

	[Header("Behavior Duration")]
	public float trackingTime = 0f;
	private bool foundTarget = false;
	public float attackDuration = 3f;
	public float attackWarmup = 1f;
	private bool launchedAttack = false;

	[Header ("Behavior Physics")]
	public GameObject attackPrefab;
	public bool spawnOnTarget = false;
	public float attackDragAmt = -1;
	public bool setVelocityToZeroOnStart = false;
	public bool lockDirectionOnTargeting = false;
	private bool lockFacing = false;
	public int numToSpawn = 1;


	private Vector3 attackDirection;

	private float attackTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){

			BehaviorUpdate();
			
			if (attackTimeCountdown <= 0 || myEnemyReference.behaviorBroken){
				EndAction();
			}

			attackTimeCountdown -= Time.deltaTime;
			if (!foundTarget && attackTimeCountdown <= (attackDuration - trackingTime)/currentDifficultyMult){
				SetAttackDirection();
				foundTarget = true;
				if (lockDirectionOnTargeting){
					lockFacing = true;
					myEnemyReference.SetFaceForAttack(attackDirection);
				}
			}

			if (!launchedAttack && attackTimeCountdown <= (attackDuration-attackWarmup)/currentDifficultyMult){
				Vector3 spawnPos = transform.position;
				GameObject attackObj;
				EnemyProjectileS projectileRef;
				if (spawnOnTarget){
					spawnPos = myEnemyReference.GetTargetReference().position;
				}
				for (int i =0; i < numToSpawn; i++){
				attackObj = Instantiate(attackPrefab, spawnPos, attackPrefab.transform.rotation)
						as GameObject;
					projectileRef = attackObj.GetComponent<EnemyProjectileS>();
				projectileRef.Fire(attackDirection, myEnemyReference, currentDifficultyMult);
				}
				launchedAttack = true;

				myEnemyReference.SetBreakState(9999f,0f);
			}
		}
	
	}

	private void InitializeAction(){

		//Debug.Log(AttackInRange());
		lockFacing = false;
		if (numToSpawn <= 0){
			numToSpawn = 1;
		}
		if (AttackInRange() || myEnemyReference.OverrideSpacingRequirement){

			launchedAttack = false;
			attackTimeCountdown = attackDuration/currentDifficultyMult;
			SetAttackDirection();
			
			myEnemyReference.myAnimator.SetTrigger(animationKey);
			//Debug.Log("Attempting to animate single attack!");
			if (signalObj != null){
				Vector3 signalPos =  transform.position;
				signalPos.z = transform.position.z+1f;
				GameObject signal = Instantiate(signalObj, signalPos, Quaternion.identity)
					as GameObject;
				signal.transform.parent = myEnemyReference.transform;
			}
			myEnemyReference.AttackFlashEffect();
			
	
			if (attackDragAmt > 0){
				myEnemyReference.myRigidbody.drag = attackDragAmt*EnemyS.FIX_DRAG_MULT;
			}
	
			if (setVelocityToZeroOnStart){
				myEnemyReference.myRigidbody.velocity = Vector3.zero;
			}
		}
		/*else if (doDodge){
			EndAction(false);
			dodgeCheck.SetEnemy(myEnemyReference);
			dodgeCheck.StartAction();
		}**/
		else{
			//myEnemyReference.myAnimator.SetTrigger("Idle");
			EndAction();
		}

	}

	private bool AttackInRange(){

		bool canContinue = true;
		doDodge = false;

		/*if (dodgeCheck != null){
			if (myEnemyReference.GetPlayerReference().InAttack()){
				canContinue = false;
				doDodge = true;
			}
		}**/

		if (rangeCheck != null){
			rangeCheck.FindTarget();
			if (!rangeCheck.currentTarget){
				canContinue = false;
			}
		}

		return canContinue;

	}

	private void SetAttackDirection(){

		myEnemyReference.RefreshTarget();
		if (trackingTime >= 0 && myEnemyReference.GetTargetReference() != null){
			attackDirection = (myEnemyReference.GetTargetReference().transform.position - transform.position).normalized;
			attackDirection.z = 0;
			myEnemyReference.SetTargetReference(attackDirection);
		}else{
			attackDirection = myEnemyReference.currentTarget;
			foundTarget = true;
			if (lockDirectionOnTargeting){
				lockFacing = true;
				myEnemyReference.SetFaceForAttack(attackDirection);
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
		doDodge = false;
		if (dodgeCheck != null){
			if (myEnemyReference.GetPlayerReference().InAttack()){
				doDodge = true;
			}
		}
		if (doDodge){

			base.EndAction(false);
			dodgeCheck.SetEnemy(myEnemyReference);
			dodgeCheck.StartAction();
		}
		else{
			base.EndAction (doNextAction);
		}
	}
}
