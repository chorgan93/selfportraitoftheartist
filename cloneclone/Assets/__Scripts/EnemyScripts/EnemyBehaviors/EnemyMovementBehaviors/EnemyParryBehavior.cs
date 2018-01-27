using UnityEngine;
using System.Collections;

public class EnemyParryBehavior : EnemyBehaviorS {

	[Header("Behavior Duration")]
	public float defendTimeFixed = -1f;
	public float defendTimeMin;
	public float defendTimeMax;

	[Header ("Behavior Physics")]
	public float defendDragAmt = -1;
	public bool setVelocityToZeroOnStart = false;
	public float counterKnockback = 500f;

	[Header ("Break Properties")]
	public float allowCounterTime = 0.5f;
	private float currentStartCounterTime = 0.5f;
	public float counterWindowEnd = 0.2f;
	private float currentEndCounterTime = 0.5f;

	[Header ("Next Action Properties")]
	public PlayerDetectS rangeDetect;
	public int nextActionCounter = 0;
	public int nextActionEnd = 0;
	public int nextActionOutOfRange = 0;

	private float limitReachedTime = 0.3f;

	private bool limitReached = false;
	private bool switchTriggered = false;
	private bool outOfRange = false;

	private float defendTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){
			
			BehaviorUpdate();
			if (myEnemyReference.GetPlayerReference() != null){
				if (!limitReached && defendTimeCountdown >= currentStartCounterTime && myEnemyReference.GetPlayerReference().CanBeCountered(currentEndCounterTime)){
				limitReached = true;
				defendTimeCountdown = limitReachedTime;
			}
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
		outOfRange = false;
		if (rangeDetect.PlayerInRange() || myEnemyReference.OverrideSpacingRequirement){
			if (animationKey != ""){
				myEnemyReference.myAnimator.SetTrigger(animationKey);
				//Debug.Log("Attempting to set Defend animation trigger!");

				if (soundObj){
					Instantiate(soundObj);
				}

				if (signalObj != null){
					Vector3 signalPos =  transform.position;
					signalPos.z = transform.position.z+1f;
					GameObject signal = Instantiate(signalObj, signalPos, Quaternion.identity)
						as GameObject;
					signal.transform.parent = myEnemyReference.transform;
				}

			}
		if (defendTimeFixed > 0){
			defendTimeCountdown = defendTimeFixed;
		}
		else{
			defendTimeCountdown = Random.Range(defendTimeMin, defendTimeMax);
		}

		defendTimeCountdown/=currentDifficultyMult;
			currentStartCounterTime=allowCounterTime/currentDifficultyMult;
			currentEndCounterTime=counterWindowEnd*currentDifficultyMult;

		if (defendDragAmt > 0){
			myEnemyReference.myRigidbody.drag = defendDragAmt;
		}

		if (setVelocityToZeroOnStart){
			myEnemyReference.myRigidbody.velocity = Vector3.zero;
		}
		}else{
			outOfRange = true;
			EndAction();

		}

	}

	public override void StartAction (bool useAnimTrigger = true)
	{
		base.StartAction (false);
		InitializeAction();

	}

	public override void EndAction (bool doNextAction = true)
	{

		if (myEnemyReference.currentState != null){

			base.EndAction(false);
		if (limitReached){
				// counter player
				myEnemyReference.GetPlayerReference().myStats.TakeDamage(myEnemyReference,0,
					counterKnockback*(myEnemyReference.GetPlayerReference().transform.position-transform.position).normalized,
					0.3f,true,true);

				// continue actions
				stateRef.behaviorSet[nextActionCounter].SetEnemy(myEnemyReference);
				stateRef.behaviorSet[nextActionCounter].StartAction();
				stateRef.SetActingBehaviorNum(nextActionCounter);
			}else if (outOfRange){
				stateRef.behaviorSet[nextActionOutOfRange].SetEnemy(myEnemyReference);
				stateRef.behaviorSet[nextActionOutOfRange].StartAction();
				stateRef.SetActingBehaviorNum(nextActionOutOfRange);
			}else{
				stateRef.behaviorSet[nextActionEnd].SetEnemy(myEnemyReference);
				stateRef.behaviorSet[nextActionEnd].StartAction();
				stateRef.SetActingBehaviorNum(nextActionEnd);
		}
		}else{
			base.EndAction();
		}
	}
}
