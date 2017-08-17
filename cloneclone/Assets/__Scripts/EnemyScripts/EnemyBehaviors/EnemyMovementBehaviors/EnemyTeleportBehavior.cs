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
	public int numTeleportsFixed = -1;
	public int numTeleportsMin = -1;
	public int numTeleportsMax = -1;
	private int numTeleports;
	private int currentTeleport = 0;
	public string unTeleportKey = "";

	[Header ("Behavior Physics")]
	public float teleportDragAmt = -1;
	public bool setVelocityToZeroOnStart = false;

	private bool teleported = false;

	private GameObject[] telePoints;
	private List<PlayerDetectS> telePointRefs = new List<PlayerDetectS>();
	private PlayerDetectS lastTeleport = null;

	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){
			
			BehaviorUpdate();

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

		if (telePointRefs.Count <= 0){
			telePoints = GameObject.FindGameObjectsWithTag("Teleport");
			for (int i = 0; i < telePoints.Length; i++){
				telePointRefs.Add(telePoints[i].GetComponent<PlayerDetectS>());
			}
		}

		if (teleportTimeFinalCooldown <= 0){
			teleportTimeFinalCooldown = teleportTimeCooldown;
		}

		if (teleportDragAmt > 0){
			myEnemyReference.myRigidbody.drag = teleportDragAmt*EnemyS.FIX_DRAG_MULT;
		}

		if (setVelocityToZeroOnStart){
			myEnemyReference.myRigidbody.velocity = Vector3.zero;
		}

	}

	private void Teleport(){

		myEnemyReference.transform.position = GetTeleportPos();

		teleported = true;
		if (unTeleportKey != ""){
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
}
