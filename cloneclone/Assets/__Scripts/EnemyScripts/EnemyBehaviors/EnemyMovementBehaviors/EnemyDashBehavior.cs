using UnityEngine;
using System.Collections;

public class EnemyDashBehavior : EnemyBehaviorS {

	[Header("Behavior Duration")]
	public float timeBetweenDashes = 0.8f;
	private float dashDuration;
	public int numDashesFixed = -1;
	public int numDashesMin = 0;
	public int numDashesMax = 0;
	private int numDashesRemain;

	[Header("Movement Variables")]
	public GameObject poi;
	public float dashDragAmt = -1f;
	public float dashForce = 50f;

	[Header("Target Variables")]
	public float moveTargetRange = 5f;
	private Vector3 currentDashTarget;
	
	// Update is called once per frame
	void FixedUpdate () {
		
		BehaviorUpdate();
		
		if (BehaviorActing()){


			dashDuration -= Time.deltaTime*currentDifficultyMult;
			if (dashDuration <= 0){
				numDashesRemain--;
				if (numDashesRemain <= 0){
					EndAction();
				}else{
					TriggerDash();
				}
			}
		}
		
	}
	
	private void InitializeAction(){

		allowStun = false;

		if (poi == null || poi == myEnemyReference.gameObject){
			if (myEnemyReference.GetTargetReference() != null){
				poi = myEnemyReference.GetTargetReference().gameObject;
			}
			else{
				poi = myEnemyReference.gameObject;
			}
		}

		if (dashDragAmt > 0){
			myEnemyReference.myRigidbody.drag = dashDragAmt;
		}

		if (numDashesFixed > 0){
			numDashesRemain = numDashesFixed;
		}else{
			numDashesRemain = Mathf.RoundToInt(Random.Range(numDashesMin, numDashesMax));
		}

		TriggerDash();
		
	}

	private void TriggerDash(){

		DetermineTarget();

		if (animationKey != ""){
			myEnemyReference.myAnimator.SetTrigger(animationKey);
		}

		dashDuration = timeBetweenDashes;
		myEnemyReference.myRigidbody.velocity = Vector3.zero;
		myEnemyReference.myRigidbody.AddForce((currentDashTarget-transform.position).normalized
			*dashForce*Time.deltaTime, ForceMode.Impulse);
		

	}

	private void DetermineTarget(){


		currentDashTarget = poi.transform.position + Random.insideUnitSphere*moveTargetRange;
		currentDashTarget.z = transform.position.z;


	}
	
	public override void StartAction (bool setAnimTrigger = true)
	{
		base.StartAction (false);
		InitializeAction();

	}
	
	public override void EndAction (bool doNextAction = true)
	{
		base.EndAction (doNextAction);
	}
}
