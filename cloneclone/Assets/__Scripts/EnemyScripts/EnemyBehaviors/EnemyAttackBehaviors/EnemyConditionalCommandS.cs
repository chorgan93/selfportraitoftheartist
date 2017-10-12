using UnityEngine;
using System.Collections;

public class EnemyConditionalCommandS : EnemyBehaviorS {

	[Header("Conditional Command Properties")]
	public EnemyGiveSpawnCommandS conditionalCommand;
	public PlayerDetectS rangeCheck;
	public int minSpawnsToCommand = 1;
	public int numToGiveCommandsFixed = -1;
	public int numToGiveRandomMin = -1;
	public int numToGiveRandomMax = -1;
	private int numToGiveCommands;

	[Header("Behavior Duration")]
	public float commandDuration = 3f;
	public float giveCommandDelay = 0.5f;
	private bool gaveCommand = false;
	private float commandCountdown;

	[Header ("Behavior Physics")]
	public bool setVelocityToZeroOnStart = false;


	private Vector3 attackDirection;

	private float attackTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){

			BehaviorUpdate();
			
			if (commandCountdown <= 0 || myEnemyReference.behaviorBroken){
				EndAction();
			}

			commandCountdown -= Time.deltaTime;


			if (!gaveCommand && commandCountdown <= (commandDuration-giveCommandDelay)/currentDifficultyMult){
				gaveCommand = true;
				conditionalCommand.GiveCommand(numToGiveCommands);
				myEnemyReference.SetBreakState(9999f,0f);
			}
		}
	
	}

	private void InitializeAction(){

		//Debug.Log(AttackInRange());
		if ((AttackInRange() || myEnemyReference.OverrideSpacingRequirement) && MeetsMinCommand()){

			gaveCommand = false;
			commandCountdown = commandDuration/currentDifficultyMult;

			if (numToGiveCommandsFixed > -1){
				numToGiveCommands = numToGiveCommandsFixed;
			}
			else{
				numToGiveCommands = Mathf.RoundToInt(Random.Range(numToGiveRandomMin, numToGiveRandomMax));
			}
			
			myEnemyReference.myAnimator.SetTrigger(animationKey);
			//Debug.Log("Attempting to animate single attack!");
			if (signalObj != null){
				Vector3 signalPos =  transform.position;
				signalPos.z = transform.position.z+1f;
				GameObject signal = Instantiate(signalObj, signalPos, Quaternion.identity)
					as GameObject;
				signal.transform.parent = myEnemyReference.transform;
			}
			//myEnemyReference.AttackFlashEffect();
			
	
		
	
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

	private bool MeetsMinCommand(){
		int numSummoned = conditionalCommand.NumSpawnsActive();

		return numSummoned >= minSpawnsToCommand;
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
