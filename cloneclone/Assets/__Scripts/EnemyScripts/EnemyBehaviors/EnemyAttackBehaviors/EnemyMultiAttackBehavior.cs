using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMultiAttackBehavior : EnemyBehaviorS {

	public PlayerDetectS rangeCheck;
	public bool retarget = false;
	public bool interruptIfOutOfRange = false;

	[Header("Behavior Duration")]
	public float attackDuration = 3f;
	public float attackWarmup = 1f;
	private bool launchedAttack = false;
	public float trackingTime = 0f;
	private float trackingCountdown;
	private bool foundTrackingTarget = false;
	public float timeBetweenAttacks = -1;

	[Header ("Behavior Physics")]
	public GameObject[] attackPrefab;
	private int currentAttack = 0;
	public float attackDragAmt = -1;
	public bool setVelocityToZeroOnStart = false;
	public bool momsEye = false;
	private float momsEyeMult = 1f;
	public Transform[] attackSetTargets;
	private List<Vector3> setTargetPositions;

	private Vector3 attackDirection;

	private float attackTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){
			
			BehaviorUpdate();

			if (!foundTrackingTarget){
				trackingCountdown -= Time.deltaTime*currentDifficultyMult;
				if (trackingCountdown <= 0){
					SetAttackDirection(true);
					foundTrackingTarget = true;
				}
			}
			
			attackTimeCountdown -= Time.deltaTime*currentDifficultyMult;

			if (currentAttack < attackPrefab.Length-1){
				if (attackTimeCountdown <= attackDuration-attackWarmup-timeBetweenAttacks){
					currentAttack++;
					if (!interruptIfOutOfRange || (interruptIfOutOfRange && AttackInRange())){
						ResetAttack();
					}
					else{
						EndAction();
					}
				}
			}else{
				if (attackTimeCountdown <= 0){
					currentAttack ++;
					if (currentAttack > attackPrefab.Length-1){
						EndAction();
					}
					else{
						if (!interruptIfOutOfRange || (interruptIfOutOfRange && AttackInRange())){
							ResetAttack();
						}
						else{
							EndAction();
						}
					}
				}
			}

			if (!launchedAttack && attackTimeCountdown <= (attackDuration-attackWarmup)){
				GameObject attackObj = Instantiate(attackPrefab[currentAttack], transform.position, Quaternion.identity)
					as GameObject;
				EnemyProjectileS projectileRef = attackObj.GetComponent<EnemyProjectileS>();
				projectileRef.Fire(attackDirection*momsEyeMult, myEnemyReference);
				if (momsEye){
					momsEyeMult*=-1f;
				}
				launchedAttack = true;
			}


		}
	
	}

	private void InitializeAction(){

		if (AttackInRange()){

			if (attackSetTargets.Length > 0 && setTargetPositions == null){
				setTargetPositions = new List<Vector3>();
				Vector3 setTarget = Vector3.zero;
				for (int i = 0; i < attackSetTargets.Length; i++){
					if (attackSetTargets[i] != null){
					setTarget = attackSetTargets[i].localPosition.normalized;
					setTarget.z = 0f;
					}else{
						setTarget = Vector3.zero;
					}
					setTargetPositions.Add(setTarget);
				}
			}

			momsEyeMult = 1f;
			launchedAttack = false;
			currentAttack = 0;
			attackTimeCountdown = attackDuration;
			SetAttackDirection();

			if (soundObj){
				Instantiate(soundObj);
			}
			myEnemyReference.myAnimator.SetTrigger(animationKey);
			if (signalObj != null){
				Vector3 signalPos =  transform.position;
				signalPos.z = transform.position.z+1f;
				GameObject signal = Instantiate(signalObj, signalPos, Quaternion.identity)
					as GameObject;
				signal.transform.parent = myEnemyReference.transform;
			}
			myEnemyReference.AttackFlashEffect();

			if (trackingTime <= 0){
				foundTrackingTarget = true;
			}else{
				trackingCountdown = trackingTime;
				foundTrackingTarget = false;
			}
				
	
			if (attackDragAmt > 0){
				myEnemyReference.myRigidbody.drag = attackDragAmt*EnemyS.FIX_DRAG_MULT;
			}
	
			if (setVelocityToZeroOnStart){
				myEnemyReference.myRigidbody.velocity = Vector3.zero;
			}

                myEnemyReference.TriggerFosAttack();
		}
		else{
			EndAction();
		}

	}

	private void ResetAttack(){
		launchedAttack = false;
		attackTimeCountdown = attackDuration-attackWarmup;
		if (retarget){
			SetAttackDirection();
		}
		if (retarget || momsEye){
			if (myEnemyReference.myTracker && trackingTime > 0){
				if (myEnemyReference.transform.localScale.x < 0){
					Vector3 reverseDir = attackDirection*momsEyeMult;
					reverseDir.x*=-1f;
					myEnemyReference.myTracker.FireEffect(reverseDir, myEnemyReference.bloodColor, attackWarmup-trackingTime, Vector3.zero);
				}else{
					myEnemyReference.myTracker.FireEffect(attackDirection*momsEyeMult, myEnemyReference.bloodColor, attackWarmup-trackingTime, Vector3.zero);
				}
			}
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

	private void SetAttackDirection(bool doTracker = false){

		if (attackSetTargets.Length > 0){
			if (setTargetPositions[currentAttack] == Vector3.zero){
				attackDirection = (myEnemyReference.GetTargetReference().transform.position - transform.position).normalized;
			}else{
				attackDirection = (setTargetPositions[currentAttack]);
			}
		}else{
		attackDirection = (myEnemyReference.GetTargetReference().transform.position - transform.position).normalized;
		}
		attackDirection.z = 0;
		if (myEnemyReference.myTracker && doTracker && trackingTime > 0){
			if (myEnemyReference.transform.localScale.x < 0){
				Vector3 reverseDir = attackDirection*momsEyeMult;
				reverseDir.x*=-1f;
				myEnemyReference.myTracker.FireEffect(reverseDir, myEnemyReference.bloodColor, attackWarmup-trackingTime, Vector3.zero);
			}else{
				myEnemyReference.myTracker.FireEffect(attackDirection*momsEyeMult, myEnemyReference.bloodColor, attackWarmup-trackingTime, Vector3.zero);
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
		base.EndAction (doNextAction);
	}
}
