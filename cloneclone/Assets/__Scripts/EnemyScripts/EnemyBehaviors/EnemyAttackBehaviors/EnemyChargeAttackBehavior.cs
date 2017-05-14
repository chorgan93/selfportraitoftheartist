using UnityEngine;
using System.Collections;

public class EnemyChargeAttackBehavior : EnemyBehaviorS {

	public PlayerDetectS rangeCheck;
	public EnemyDodgeBehaviorS dodgeCheck;
	private bool doDodge = false;

	[Header("Behavior Properties")]
	public float attackDuration = 3f;
	public float attackWarmup = 2f;
	public EnemyChargeAttackS attackCollider;


	private Vector3 attackDirection;

	private float attackTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {
		
		BehaviorUpdate();

		if (BehaviorActing()){

			attackTimeCountdown -= Time.deltaTime;


			if (attackTimeCountdown <= 0){
				EndAction();
			}
		}
	
	}

	private void InitializeAction(){

		if (AttackInRange()){
			myEnemyReference.myRigidbody.velocity = Vector3.zero;
			attackTimeCountdown = attackDuration;
			attackCollider.TurnOn(attackWarmup);
		}
		else if (doDodge){

			EndAction(false);
			dodgeCheck.SetEnemy(myEnemyReference);
			dodgeCheck.StartAction();
		}
		else{
			EndAction();
		}

	}

	private bool AttackInRange(){

		bool canContinue = true;
		doDodge = false;

		// check for dodge in beginning
		if (dodgeCheck != null){
			if (myEnemyReference.GetPlayerReference().InAttack()){
				canContinue = false;
				doDodge = true;
			}
		}

		if (rangeCheck != null){
			if (!rangeCheck.PlayerInRange()){
				canContinue = false;
			}
		}

		return canContinue;

	}

	public override void StartAction (bool setAnimTrigger = true)
	{
		base.StartAction ();

		InitializeAction();
	}

	public override void EndAction (bool doNextAction = true)
	{
		
		base.EndAction (doNextAction);
		
	}
}
