using UnityEngine;
using System.Collections;

public class EnemySeekBehavior : EnemyBehaviorS {

	[Header("Behavior Duration")]
	public float wanderTimeFixed = -1f;
	public float wanderTimeMin;
	public float wanderTimeMax;

	[Header("Movement Variables")]
	public GameObject poi;
	public float wanderDragAmt = -1f;
	public float wanderSpeedFixed = -1f;
	public float wanderSpeedMin;
	public float wanderSpeedMax;

	private float currentWanderSpeed;

	[Header("Target Variables")]
	public float moveTargetRange = 5f;
	public float moveTargetChangeMin;
	public float moveTargetChangeMax;
	
	private float wanderTimeCountdown;
	private float changeWanderTargetCountdown;
	private Vector3 currentMoveTarget;
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (BehaviorActing()){

			DetermineTarget();

			DoMovement();

			wanderTimeCountdown -= Time.deltaTime;
			if (wanderTimeCountdown <= 0){
				EndAction();
			}
		}
		
	}
	
	private void InitializeAction(){

		if (poi == null || poi == myEnemyReference.gameObject){
			if (myEnemyReference.GetTargetReference() != null){
				poi = myEnemyReference.GetTargetReference().gameObject;
			}
			else{
				poi = myEnemyReference.gameObject;
			}
		}
		
		if (wanderTimeFixed > 0){
			wanderTimeCountdown = wanderTimeFixed;
		}
		else{
			wanderTimeCountdown = Random.Range(wanderTimeMin, wanderTimeMax);
		}

		if (wanderSpeedFixed > 0){
			currentWanderSpeed = wanderSpeedFixed;
		}
		else{
			currentWanderSpeed = Random.Range(wanderSpeedMin, wanderSpeedMax);
		}

		changeWanderTargetCountdown = Random.Range(moveTargetChangeMin, moveTargetChangeMax);
		
		currentMoveTarget = transform.position + Random.insideUnitSphere*moveTargetRange;
		currentMoveTarget.z = transform.position.z;

		if (wanderDragAmt > 0){
			myEnemyReference.myRigidbody.drag = wanderDragAmt;
		}
		
	}

	private void DoMovement(){

		if (!myEnemyReference.hitStunned){
			myEnemyReference.myRigidbody.AddForce((currentMoveTarget-transform.position).normalized
		                                      *currentWanderSpeed*Time.deltaTime);
		}

	}

	private void DetermineTarget(){

		changeWanderTargetCountdown -= Time.deltaTime;

		if (changeWanderTargetCountdown <= 0){
			changeWanderTargetCountdown = Random.Range(moveTargetChangeMin, moveTargetChangeMax);

			currentMoveTarget = poi.transform.position + Random.insideUnitSphere*moveTargetRange;
			currentMoveTarget.z = transform.position.z;
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
