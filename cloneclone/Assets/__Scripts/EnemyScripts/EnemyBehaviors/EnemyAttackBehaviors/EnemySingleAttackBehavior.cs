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
	public float attackDragAmt = -1;
	public bool setVelocityToZeroOnStart = false;


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
			}

			if (!launchedAttack && attackTimeCountdown <= (attackDuration-attackWarmup)/currentDifficultyMult){
				GameObject attackObj = Instantiate(attackPrefab, transform.position, attackPrefab.transform.rotation)
					as GameObject;
				EnemyProjectileS projectileRef = attackObj.GetComponent<EnemyProjectileS>();
				projectileRef.Fire(attackDirection, myEnemyReference, currentDifficultyMult);
				launchedAttack = true;

				myEnemyReference.SetBreakState(9999f,0f);
			}
		}
	
	}

	private void InitializeAction(){

		if (AttackInRange()){

			launchedAttack = false;
			attackTimeCountdown = attackDuration/currentDifficultyMult;
			SetAttackDirection();
			
			myEnemyReference.myAnimator.SetTrigger(animationKey);
			if (signalObj != null){
				Vector3 signalPos =  transform.position;
				signalPos.z = transform.position.z+1f;
				GameObject signal = Instantiate(signalObj, signalPos, Quaternion.identity)
					as GameObject;
				signal.transform.parent = myEnemyReference.transform;
			}
			
	
			if (attackDragAmt > 0){
				myEnemyReference.myRigidbody.drag = attackDragAmt;
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
			myEnemyReference.myAnimator.SetTrigger("Idle");
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
			if (!rangeCheck.currentTarget){
				canContinue = false;
			}
		}

		return canContinue;

	}

	private void SetAttackDirection(){

		if (trackingTime >= 0 && myEnemyReference.GetTargetReference() != null){
			attackDirection = (myEnemyReference.GetTargetReference().transform.position - transform.position).normalized;
			attackDirection.z = 0;
			myEnemyReference.SetTargetReference(attackDirection);
		}else{
			attackDirection = myEnemyReference.currentTarget;
			foundTarget = true;
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
