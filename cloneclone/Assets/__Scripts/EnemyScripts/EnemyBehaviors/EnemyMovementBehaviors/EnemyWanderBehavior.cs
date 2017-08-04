using UnityEngine;
using System.Collections;

public class EnemyWanderBehavior : EnemyBehaviorS {

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

	[Header("Buddy Variables")]
	public float chanceToTriggerBuddy = -1f;
	public EnemyBuddyS[] buddiesToUse;
	
	private float wanderTimeCountdown;
	private float changeWanderTargetCountdown;
	private Vector3 currentMoveTarget;

	private bool didWallRedirect = false;
	// Update is called once per frame
	void FixedUpdate () {
		
		if (BehaviorActing()){
			
			BehaviorUpdate();

			DetermineTarget();

			DoMovement();

			wanderTimeCountdown -= Time.deltaTime;
			if (wanderTimeCountdown <= 0){
				EndAction();
			}
		}
		
	}
	
	private void InitializeAction(){
		
		if (wanderTimeFixed > 0){
			wanderTimeCountdown = wanderTimeFixed;
		}
		else{
			wanderTimeCountdown = Random.Range(wanderTimeMin, wanderTimeMax);
		}

		didWallRedirect = false;

		wanderTimeCountdown/=currentDifficultyMult;

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
			myEnemyReference.myRigidbody.drag = wanderDragAmt*EnemyS.FIX_DRAG_MULT;
		}

		TriggerBuddyEffect();
		
	}

	void TriggerBuddyEffect(){
		if (chanceToTriggerBuddy > 0 && buddiesToUse.Length > 0){
			float buddyChance = Random.Range(0,1f);
			if (buddyChance <= chanceToTriggerBuddy){
				for (int i = 0; i < buddiesToUse.Length;i++){
					buddiesToUse[i].TriggerAction();
				}
			}
		}
	}

	private void DoMovement(){

		if (!myEnemyReference.hitStunned){
			myEnemyReference.myRigidbody.AddForce((currentMoveTarget-transform.position).normalized
		                                      *currentWanderSpeed*Time.deltaTime);
		}

	}

	private void DetermineTarget(){

		if (myEnemyReference.hitWall && !didWallRedirect){
			WallRedirect();
		}

		changeWanderTargetCountdown -= Time.deltaTime;

		if (changeWanderTargetCountdown <= 0){
			changeWanderTargetCountdown = Random.Range(moveTargetChangeMin, moveTargetChangeMax);

			currentMoveTarget = transform.position + Random.insideUnitSphere*moveTargetRange;
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
		Debug.Log(myEnemyReference.enemyName + " did Wall Redirect!", myEnemyReference.gameObject);
		didWallRedirect =  true;
	}
}
