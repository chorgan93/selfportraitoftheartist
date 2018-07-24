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

	private BlockDisplay3DS parryEffect;

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
			if (myEnemyReference.GetPlayerReference() != null){
				if (!limitReached && defendTimeCountdown >= currentStartCounterTime && myEnemyReference.GetPlayerReference().CanBeCountered(currentEndCounterTime)){
				limitReached = true;
				defendTimeCountdown = limitReachedTime;
					if (parryEffect){
						/*parryEffect.FireParry(myEnemyReference.transform.position, myEnemyReference.GetPlayerReference().transform.position,
							myEnemyReference.bloodColor, Color.red);**/
							}
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
				CameraShakeS.C.SmallShake();
				CameraShakeS.C.SloAndPunch(0.5f, 0.7f, 0.35f);
				//CameraShakeS.C.DelaySleep(0.3f, 0.1f);
				if (parryEffect){
					parryEffect.FireParryEffect(myEnemyReference.GetPlayerReference().transform.position);
				}

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

	public void SetBlockRef(BlockDisplay3DS myBlock){
		parryEffect = myBlock;
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
