using UnityEngine;
using System.Collections;

public class EnemySingleAttackBehavior : EnemyBehaviorS {

	public PlayerDetectS rangeCheck;

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

			attackTimeCountdown -= Time.deltaTime;
			if (!foundTarget && attackTimeCountdown <= (attackDuration - trackingTime)){
				SetAttackDirection();
				foundTarget = true;
			}

			if (!launchedAttack && attackTimeCountdown <= (attackDuration-attackWarmup)){
				GameObject attackObj = Instantiate(attackPrefab, transform.position, Quaternion.identity)
					as GameObject;
				EnemyProjectileS projectileRef = attackObj.GetComponent<EnemyProjectileS>();
				projectileRef.Fire(attackDirection, myEnemyReference);
				launchedAttack = true;
			}

			if (attackTimeCountdown <= 0){
				EndAction();
			}
		}
	
	}

	private void InitializeAction(){

		if (AttackInRange()){

			launchedAttack = false;
			attackTimeCountdown = attackDuration;
			SetAttackDirection();
			
	
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

		attackDirection = (myEnemyReference.GetPlayerReference().transform.position - transform.position).normalized;
		attackDirection.z = 0;

	}

	public override void StartAction ()
	{
		base.StartAction ();

		InitializeAction();
	}

	public override void EndAction ()
	{
		base.EndAction ();
	}
}
