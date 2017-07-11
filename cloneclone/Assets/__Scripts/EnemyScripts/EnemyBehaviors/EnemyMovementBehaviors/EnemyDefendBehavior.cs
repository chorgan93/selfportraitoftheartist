using UnityEngine;
using System.Collections;

public class EnemyDefendBehavior : EnemyBehaviorS {

	[Header("Behavior Duration")]
	public float defendTimeFixed = -1f;
	public float defendTimeMin;
	public float defendTimeMax;

	[Header ("Behavior Physics")]
	public float defendDragAmt = -1;
	public bool setVelocityToZeroOnStart = false;

	[Header ("Break Properties")]
	public int numBreaks = -1;
	public int breakAttackMin = 2;
	public int breakAttackMax = 5;
	private int currentBreak = 0;

	[Header ("Next Action Properties")]
	public PlayerDetectS rangeDetect;
	public int nextActionCounter = 0;
	public int nextActionEnd = 0;
	public int nextActionOutOfRange = 0;

	private float limitReachedTime = 0.3f;

	private bool limitReached = false;
	private bool switchTriggered = false;

	private float defendTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){
			
			BehaviorUpdate();

			if (!limitReached && myEnemyReference.numAttacksTaken >= currentBreak){
				limitReached = true;
				defendTimeCountdown = limitReachedTime;
			}

			defendTimeCountdown -= Time.deltaTime;
			if (defendTimeCountdown <= 0 && !switchTriggered){
				switchTriggered = true;
				EndAction();
			}
		}
	
	}

	private void InitializeAction(){

		limitReached = false;
		switchTriggered = false;
		if (rangeDetect.PlayerInRange()){

		if (defendTimeFixed > 0){
			defendTimeCountdown = defendTimeFixed;
		}
		else{
			defendTimeCountdown = Random.Range(defendTimeMin, defendTimeMax);
		}

		if (numBreaks > 0){
			currentBreak = numBreaks;
		}else{
			currentBreak = Mathf.RoundToInt(Random.Range(breakAttackMin, breakAttackMax));
		}

		defendTimeCountdown/=currentDifficultyMult;

		if (defendDragAmt > 0){
			myEnemyReference.myRigidbody.drag = defendDragAmt;
		}

		if (setVelocityToZeroOnStart){
			myEnemyReference.myRigidbody.velocity = Vector3.zero;
		}
		}else{
			base.CancelAction();
			myEnemyReference.currentState.behaviorSet[nextActionOutOfRange].SetEnemy(myEnemyReference);
			myEnemyReference.currentState.behaviorSet[nextActionOutOfRange].StartAction();
			myEnemyReference.currentState.SetActingBehaviorNum(nextActionOutOfRange);
		}

	}

	public override void StartAction (bool useAnimTrigger = true)
	{
		base.StartAction ();
		InitializeAction();

	}

	public override void EndAction (bool doNextAction = true)
	{

		base.CancelAction();
		if (myEnemyReference.currentState != null){
		if (limitReached){
			myEnemyReference.currentState.behaviorSet[nextActionCounter].SetEnemy(myEnemyReference);
			myEnemyReference.currentState.behaviorSet[nextActionCounter].StartAction();
			myEnemyReference.currentState.SetActingBehaviorNum(nextActionCounter);
		}else{
			myEnemyReference.currentState.behaviorSet[nextActionEnd].SetEnemy(myEnemyReference);
			myEnemyReference.currentState.behaviorSet[nextActionEnd].StartAction();
			myEnemyReference.currentState.SetActingBehaviorNum(nextActionEnd);
		}
		}
	}
}
