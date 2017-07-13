using UnityEngine;
using System.Collections;

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

	private Vector3 attackDirection;

	private float attackTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){
			
			BehaviorUpdate();

			if (!foundTrackingTarget){
				trackingCountdown -= Time.deltaTime*currentDifficultyMult;
				if (trackingCountdown <= 0){
					SetAttackDirection();
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
				projectileRef.Fire(attackDirection, myEnemyReference);
				launchedAttack = true;
			}


		}
	
	}

	private void InitializeAction(){

		if (AttackInRange()){

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

			if (trackingTime <= 0){
				foundTrackingTarget = true;
			}else{
				trackingCountdown = trackingTime;
			}
			
	
			if (attackDragAmt > 0){
				myEnemyReference.myRigidbody.drag = attackDragAmt;
			}
	
			if (setVelocityToZeroOnStart){
				myEnemyReference.myRigidbody.velocity = Vector3.zero;
			}
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
	}

	private bool AttackInRange(){

		bool canContinue = true;

		if (rangeCheck != null){
			if (!rangeCheck.PlayerInRange()){
				canContinue = false;
			}
		}

		return canContinue;

	}

	private void SetAttackDirection(){

		attackDirection = (myEnemyReference.GetTargetReference().transform.position - transform.position).normalized;
		attackDirection.z = 0;

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
