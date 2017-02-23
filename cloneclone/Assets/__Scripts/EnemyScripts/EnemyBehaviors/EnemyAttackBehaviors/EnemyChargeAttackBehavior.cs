using UnityEngine;
using System.Collections;

public class EnemyChargeAttackBehavior : EnemyBehaviorS {

	public PlayerDetectS rangeCheck;

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
			attackTimeCountdown = attackDuration;
			attackCollider.TurnOn(attackWarmup);
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
