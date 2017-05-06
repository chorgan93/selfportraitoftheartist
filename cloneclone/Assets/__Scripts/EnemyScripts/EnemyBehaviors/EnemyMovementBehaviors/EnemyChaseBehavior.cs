using UnityEngine;
using System.Collections;

public class EnemyChaseBehavior : EnemyBehaviorS {
	
	[Header("Behavior Duration")]
	public float chaseTimeFixed = -1f;
	public float chaseTimeMin;
	public float chaseTimeMax;
	public PlayerDetectS chaseEndRange;
	
	[Header("Movement Variables")]
	public float chaseDragAmt = -1f;
	public float chaseSpeedFixed = -1f;
	public float chaseSpeedMin;
	public float chaseSpeedMax;
	
	private float currentchaseSpeed;
	
	private float chaseTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (BehaviorActing()){
			
			BehaviorUpdate();

			
			DoMovement();
			
			chaseTimeCountdown -= Time.deltaTime;

			if (chaseEndRange != null){
				if (chaseEndRange.currentTarget!=null){
					EndAction();
				}
			}

			if (chaseTimeCountdown <= 0){
				EndAction();
			}
		}
		
	}
	
	private void InitializeAction(){
		
		if (chaseTimeFixed > 0){
			chaseTimeCountdown = chaseTimeFixed;
		}
		else{
			chaseTimeCountdown = Random.Range(chaseTimeMin, chaseTimeMax);
		}
		
		if (chaseSpeedFixed > 0){
			currentchaseSpeed = chaseSpeedFixed;
		}
		else{
			currentchaseSpeed = Random.Range(chaseSpeedMin, chaseSpeedMax);
		}

		currentchaseSpeed*=currentDifficultyMult;

		
		if (chaseDragAmt > 0){
			myEnemyReference.myRigidbody.drag = chaseDragAmt;
		}
		
	}
	
	private void DoMovement(){
		
		if (!myEnemyReference.hitStunned){
			if (myEnemyReference.GetTargetReference()){
				myEnemyReference.myRigidbody.AddForce((myEnemyReference.GetTargetReference().transform.position
		                                       -transform.position).normalized*currentchaseSpeed*Time.deltaTime);
			}else{
				myEnemyReference.myRigidbody.AddForce((myEnemyReference.GetPlayerReference().transform.position
				                                       -transform.position).normalized*currentchaseSpeed*Time.deltaTime);
			}
		}
		
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
