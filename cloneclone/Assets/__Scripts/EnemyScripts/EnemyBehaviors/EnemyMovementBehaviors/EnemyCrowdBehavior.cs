using UnityEngine;
using System.Collections;

public class EnemyCrowdBehavior : EnemyBehaviorS {

	[Header("Behavior Duration")]
	public float wanderTimeFixed = -1f;
	public float wanderTimeMin;
	public float wanderTimeMax;

	[Header("Movement Variables")]
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

	private Transform targetEnemy;

	private bool didWallRedirect = false;
	
	// Update is called once per frame
	void FixedUpdate () {
		
		BehaviorUpdate();
		
		if (BehaviorActing()){

			DetermineTarget();

			DoMovement();

			wanderTimeCountdown -= Time.deltaTime*currentDifficultyMult;
			if (wanderTimeCountdown <= 0){
				EndAction();
			}
		}
		
	}
	
	private void InitializeAction(){

		if (myEnemyReference.mySpawner.myManager.GetNotAlone(myEnemyReference.mySpawner)){

			FindCrowdTarget();
		didWallRedirect  = false;


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
		currentWanderSpeed*=currentDifficultyMult;

		changeWanderTargetCountdown = Random.Range(moveTargetChangeMin, moveTargetChangeMax);
		
		currentMoveTarget = transform.position + Random.insideUnitSphere*moveTargetRange;
		currentMoveTarget.z = transform.position.z;

		if (wanderDragAmt > 0){
			myEnemyReference.myRigidbody.drag = wanderDragAmt*EnemyS.FIX_DRAG_MULT;
		}
		}else{
			EndAction();
		}
		
	}

	private void DoMovement(){

		if (!myEnemyReference.hitStunned){
			myEnemyReference.myRigidbody.AddForce((currentMoveTarget-transform.position).normalized
		                                      *currentWanderSpeed*Time.deltaTime);
		}

	}

	void FindCrowdTarget(){
		targetEnemy = myEnemyReference.mySpawner.myManager.GetCrowdPoint(myEnemyReference.mySpawner).transform;
	}

	private void DetermineTarget(){

		/*if (myEnemyReference.hitWall && !didWallRedirect){
			WallRedirect();
		}*/
		changeWanderTargetCountdown -= Time.deltaTime;

		if (changeWanderTargetCountdown <= 0){
			changeWanderTargetCountdown = Random.Range(moveTargetChangeMin, moveTargetChangeMax);

			currentMoveTarget = targetEnemy.transform.position + Random.insideUnitSphere*moveTargetRange;
			currentMoveTarget.z = transform.position.z;
			didWallRedirect = false;
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

	void WallRedirect(){
		Vector3 wallRedirect = Vector3.zero;
		float targetDistance = (currentMoveTarget-transform.position).magnitude;
		wallRedirect = Quaternion.Euler(0,0,180f)*(currentMoveTarget-transform.position).normalized;
		wallRedirect*=targetDistance;
		wallRedirect.z = transform.position.z;
		currentMoveTarget = wallRedirect;
		didWallRedirect =  true;
	}
}
