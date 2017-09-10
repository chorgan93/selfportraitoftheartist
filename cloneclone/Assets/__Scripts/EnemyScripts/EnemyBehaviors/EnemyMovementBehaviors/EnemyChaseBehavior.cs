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
	private float minChaseTime;
	private float minChaseMult = 0.9f;

	private bool didWallRedirect = false;
	private bool redirecting = false;
	private float redirectTime = 0.4f;
	private float preventRedirectTime = 0.3f;
	private float preventRedirectCountdown = 0f;
	private float redirectCountdown;
	private Vector3 redirectTarget;
	private Vector3 currentMoveTarget;

	private bool initialFace;
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (BehaviorActing()){
			
			BehaviorUpdate();

			
			DoMovement();
			
			chaseTimeCountdown -= Time.deltaTime;

			if (chaseEndRange != null){
				if (chaseEndRange.currentTarget!=null && chaseTimeCountdown<=minChaseTime){
					chaseTimeCountdown = 0;
				}
			}

			if (chaseTimeCountdown <= 0){
				EndAction();
			}
		}
		
	}
	
	private void InitializeAction(){

		preventRedirectCountdown = preventRedirectTime;
		if (chaseTimeFixed > 0){
			chaseTimeCountdown = chaseTimeFixed;
		}
		else{
			chaseTimeCountdown = Random.Range(chaseTimeMin, chaseTimeMax);
		}
		minChaseTime = chaseTimeCountdown*minChaseMult;
		
		if (chaseSpeedFixed > 0){
			currentchaseSpeed = chaseSpeedFixed;
		}
		else{
			currentchaseSpeed = Random.Range(chaseSpeedMin, chaseSpeedMax);
		}

		currentchaseSpeed*=currentDifficultyMult;
		didWallRedirect = false;
		redirecting = false;
		
		if (chaseDragAmt > 0){
			myEnemyReference.myRigidbody.drag = chaseDragAmt;
		}

		initialFace = facePlayer;
		
	}
	
	private void DoMovement(){

		if (myEnemyReference.hitWall && !didWallRedirect && preventRedirectCountdown <= 0f){
			WallRedirect();
		}

		if (!myEnemyReference.hitStunned){
			preventRedirectCountdown -= Time.deltaTime;
			if (redirecting){
				redirectCountdown -= Time.deltaTime;
				if (redirectCountdown <= 0){
					redirecting = false;
					myEnemyReference.SetFaceStatus(initialFace);
					didWallRedirect = false;
				}
				myEnemyReference.myRigidbody.AddForce((redirectTarget
					-transform.position).normalized*currentchaseSpeed*Time.deltaTime);
			}else{
			if (myEnemyReference.GetTargetReference()){
				myEnemyReference.myRigidbody.AddForce((myEnemyReference.GetTargetReference().transform.position
		                                       -transform.position).normalized*currentchaseSpeed*Time.deltaTime);
					currentMoveTarget = myEnemyReference.GetTargetReference().transform.position;
			}else{
				myEnemyReference.myRigidbody.AddForce((myEnemyReference.GetPlayerReference().transform.position
				                                       -transform.position).normalized*currentchaseSpeed*Time.deltaTime);
					currentMoveTarget = myEnemyReference.GetPlayerReference().transform.position;
			}
			}
		}
		
	}
	
	public override void StartAction (bool setAnimTrigger = true)
	{
		base.StartAction ();
		//Debug.Log("Attempting to animate chase");
		InitializeAction();

	}
	
	public override void EndAction (bool doNextAction = true)
	{
		base.EndAction (doNextAction);
	}

	void WallRedirect(){
		redirectTarget = Vector3.zero;
		float targetDistance = (currentMoveTarget-transform.position).magnitude;
		redirectTarget = Quaternion.Euler(0,0,180f)*(currentMoveTarget-transform.position).normalized;
		redirectTarget*=targetDistance;
		redirectTarget.z = transform.position.z;
		//Debug.Log(myEnemyReference.enemyName + " did Wall Redirect!", myEnemyReference.gameObject);
		didWallRedirect =  true;
		redirecting = true;
		redirectCountdown = redirectTime;
		myEnemyReference.SetFaceStatus(false);
	}
}
