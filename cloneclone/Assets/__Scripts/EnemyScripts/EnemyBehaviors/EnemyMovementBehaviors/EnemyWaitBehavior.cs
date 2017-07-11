using UnityEngine;
using System.Collections;

public class EnemyWaitBehavior : EnemyBehaviorS {

	[Header("Behavior Duration")]
	public float waitTimeFixed = -1f;
	public float waitTimeMin;
	public float waitTimeMax;

	[Header ("Behavior Physics")]
	public float waitDragAmt = -1;
	public bool setVelocityToZeroOnStart = false;

	private float waitTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){
			
			BehaviorUpdate();

			waitTimeCountdown -= Time.deltaTime;
			if (waitTimeCountdown <= 0){
				EndAction();
			}
		}
	
	}

	private void InitializeAction(){

		if (waitTimeFixed > 0){
			waitTimeCountdown = waitTimeFixed;
		}
		else{
			waitTimeCountdown = Random.Range(waitTimeMin, waitTimeMax);
		}

		waitTimeCountdown/=currentDifficultyMult;

		if (waitDragAmt > 0){
			myEnemyReference.myRigidbody.drag = waitDragAmt;
		}

		if (setVelocityToZeroOnStart){
			myEnemyReference.myRigidbody.velocity = Vector3.zero;
		}

	}

	public override void StartAction (bool useAnimTrigger = true)
	{
		base.StartAction ();
		InitializeAction();

	}

	public override void EndAction (bool doNextAction = true)
	{
		base.EndAction (doNextAction);
	}
}
