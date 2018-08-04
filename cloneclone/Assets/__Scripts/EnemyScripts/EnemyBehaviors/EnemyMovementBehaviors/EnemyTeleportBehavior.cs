using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyTeleportBehavior : EnemyBehaviorS {

	[Header("Behavior Duration")]
	public float teleportTimeWarmup;
	public float teleportTimeCooldown;
	public float teleportTimeFinalCooldown;

	private float teleportTimeCountdown;

    [Header("TeleportProperties")]
    public bool effectOnTeleport = false;
	public int numTeleportsFixed = -1;
	public int numTeleportsMin = -1;
	public int numTeleportsMax = -1;
	private int numTeleports;
	private int currentTeleport = 0;
	public string unTeleportKey = "";
	public string finalUnTeleportKey = "";
	public GameObject spawnOnTeleport;
	public bool dontSpawnOnFinal = false;

	[Header ("Behavior Physics")]
	public float teleportDragAmt = -1;
	public bool setVelocityToZeroOnStart = false;

	private bool teleported = false;

	private GameObject[] telePoints;
	private List<PlayerDetectS> telePointRefs = new List<PlayerDetectS>();
	private PlayerDetectS lastTeleport = null;

    [Header("Walk Properties")]
    public bool walkWhileTeleporting = false;
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

    private bool failedTeleCheck = false;


    private bool didWallRedirect = false;

	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){
			
			BehaviorUpdate();
            if (walkWhileTeleporting)
            {

                DetermineTarget();

                DoMovement();
            }

			teleportTimeCountdown -= Time.deltaTime*currentDifficultyMult;
			if (teleportTimeCountdown <= 0){
				if (teleported){
					currentTeleport++;
					if (currentTeleport > numTeleports){
						EndAction();
					}else{
						teleported = false;
						teleportTimeCountdown = teleportTimeWarmup;
						if (animationKey != ""){
							myEnemyReference.myAnimator.SetTrigger(animationKey);
						}
						if (teleportDragAmt > 0){
							myEnemyReference.myRigidbody.drag = teleportDragAmt*EnemyS.FIX_DRAG_MULT;
						}

						if (setVelocityToZeroOnStart){
							myEnemyReference.myRigidbody.velocity = Vector3.zero;
						}
					}
				}else{
					Teleport();
				}
			}
		}
	
	}

	private void InitializeAction(){

		if (numTeleportsFixed > 0){
			numTeleports = numTeleportsFixed;
		}
		else{
			numTeleports = Mathf.CeilToInt(Random.Range(numTeleportsMin, numTeleportsMax));
		}
		currentTeleport = 0;
		teleported = false;

		teleportTimeCountdown = teleportTimeWarmup;

		lastTeleport = null;

		if (telePointRefs.Count <= 0 && !failedTeleCheck){
			telePoints = GameObject.FindGameObjectsWithTag("Teleport");
            if (telePoints.Length <= 0)
            {
                failedTeleCheck = true;
#if UNITY_EDITOR
                Debug.LogError("NO TELEPORT POINTS IN SCENE!!");
#endif
            }
            else
            {
                for (int i = 0; i < telePoints.Length; i++)
                {
                    telePointRefs.Add(telePoints[i].GetComponent<PlayerDetectS>());
                }
            }
		}

        if (!failedTeleCheck)
        {
            if (teleportTimeFinalCooldown <= 0)
            {
                teleportTimeFinalCooldown = teleportTimeCooldown;
            }

            if (teleportDragAmt > 0)
            {
                myEnemyReference.myRigidbody.drag = teleportDragAmt * EnemyS.FIX_DRAG_MULT;
            }

            if (setVelocityToZeroOnStart)
            {
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
            EndAction();
        }

	}

	private void Teleport(){

		myEnemyReference.transform.position = GetTeleportPos();

		if (spawnOnTeleport && (!dontSpawnOnFinal || (dontSpawnOnFinal && currentTeleport<numTeleports-1))){
			Instantiate(spawnOnTeleport, transform.position, Quaternion.identity);
		}

        if (effectOnTeleport){
            CameraEffectsS.E.BlurEffect();
        }

		teleported = true;
		if (finalUnTeleportKey != "" && currentTeleport >= numTeleports-1){
			myEnemyReference.myAnimator.SetTrigger(finalUnTeleportKey);
		}
		else if (unTeleportKey != ""){
			myEnemyReference.myAnimator.SetTrigger(unTeleportKey);
		}
		if (currentTeleport < numTeleports-1){
			teleportTimeCountdown = teleportTimeCooldown;
		}else{
			teleportTimeCountdown = teleportTimeFinalCooldown;
		}
	}

	Vector3 GetTeleportPos(){
		Vector3 returnPos = myEnemyReference.transform.position;

		List<PlayerDetectS> possiblePts = new List<PlayerDetectS>();
		for (int i = 0; i < telePointRefs.Count; i++){
			if (telePointRefs[i] != lastTeleport && !telePointRefs[i].PlayerInRange()){
				possiblePts.Add(telePointRefs[i]);
			}
		}

		int chosenPt = Mathf.FloorToInt(Random.Range(0, possiblePts.Count));
		lastTeleport = possiblePts[chosenPt];

		returnPos = possiblePts[chosenPt].transform.position;
		returnPos.z = myEnemyReference.transform.position.z;

		return returnPos;
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
