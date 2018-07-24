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
	private bool outOfRange = false;

	private float defendTimeCountdown;

    [Header("Walk Properties")]
    public bool walkWhileDefending = false;
    public GameObject poi;
    public string searchPOIName = "";
    public float wanderDragAmt = -1f;
    public float wanderSpeedFixed = -1f;
    public float wanderSpeedMin;
    public float wanderSpeedMax;

    private float currentWanderSpeed;
    public float moveTargetRange = 5f;
    public float moveTargetChangeMin;
    public float moveTargetChangeMax;

    private float wanderTimeCountdown;
    private float changeWanderTargetCountdown;
    private Vector3 currentMoveTarget;


    private bool didWallRedirect = false;

	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){
			
			BehaviorUpdate();

            if (walkWhileDefending)
            {

                DetermineTarget();

                DoMovement();
            }

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
                    didWallRedirect = false;
            if (searchPOIName != "" && poi == null)
            {
                GameObject searchPoi = GameObject.Find(searchPOIName);
                if (searchPoi)
                {
                    poi = searchPoi;
                }
            }
            if (poi == null || poi == myEnemyReference.gameObject)
            {
                if (myEnemyReference.GetTargetReference() != null)
                {
                    poi = myEnemyReference.GetTargetReference().gameObject;
                }
                else
                {
                    poi = myEnemyReference.gameObject;
                }
            }
                    if (wanderSpeedFixed > 0)
            {
                currentWanderSpeed = wanderSpeedFixed;
            }
            else
            {
                currentWanderSpeed = Random.Range(wanderSpeedMin, wanderSpeedMax);
            }
            currentWanderSpeed *= currentDifficultyMult;
                    changeWanderTargetCountdown = Random.Range(moveTargetChangeMin, moveTargetChangeMax);

            currentMoveTarget = transform.position + Random.insideUnitSphere * moveTargetRange;
            currentMoveTarget.z = transform.position.z;

            if (wanderDragAmt > 0)
            {
                myEnemyReference.myRigidbody.drag = wanderDragAmt * EnemyS.FIX_DRAG_MULT;
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

                private void DoMovement()
    {

        if (!myEnemyReference.hitStunned)
        {
            myEnemyReference.myRigidbody.AddForce((currentMoveTarget - transform.position).normalized
                                              * currentWanderSpeed * Time.deltaTime);
        }

    }

    private void DetermineTarget()
    {

        /*if (myEnemyReference.hitWall && !didWallRedirect){
            WallRedirect();
        }*/
        changeWanderTargetCountdown -= Time.deltaTime;

        if (changeWanderTargetCountdown <= 0)
        {
            changeWanderTargetCountdown = Random.Range(moveTargetChangeMin, moveTargetChangeMax);

            currentMoveTarget = poi.transform.position + Random.insideUnitSphere * moveTargetRange;
            currentMoveTarget.z = transform.position.z;
            didWallRedirect = false;
        }

    }
                void WallRedirect()
    {
        Vector3 wallRedirect = Vector3.zero;
        float targetDistance = (currentMoveTarget - transform.position).magnitude;
        wallRedirect = Quaternion.Euler(0, 0, 180f) * (currentMoveTarget - transform.position).normalized;
        wallRedirect *= targetDistance;
        wallRedirect.z = transform.position.z;
        currentMoveTarget = wallRedirect;
        didWallRedirect = true;
    }
}
