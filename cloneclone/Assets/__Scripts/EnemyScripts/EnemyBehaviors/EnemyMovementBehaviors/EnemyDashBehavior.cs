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
	public float applyDashTimeMult = 0.8f;

	[Header("Target Variables")]
	public float moveTargetRange = 5f;
	private Vector3 currentDashTarget;
	private Vector3 currentPOIPos;

	private float applyDashCountdown;
	private Vector3 dashNormal = Vector3.zero;
	private float dashDirection = 1f;
	
	// Update is called once per frame
	void FixedUpdate () {
		
		BehaviorUpdate();
		
		if (BehaviorActing()){


			dashDuration -= Time.deltaTime*currentDifficultyMult;
			applyDashCountdown -= Time.deltaTime*currentDifficultyMult;
			if (applyDashCountdown > 0){
				DetermineAndAddForce();
				if (myEnemyReference.hitWall){
					dashDirection *= -1f;
				}
			}
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

		//DetermineTarget();

		if (animationKey != ""){
			myEnemyReference.myAnimator.SetTrigger(animationKey);
		}

		currentPOIPos = poi.transform.position+ Random.insideUnitSphere*moveTargetRange;
		currentPOIPos.z = transform.position.z;

		dashDuration = timeBetweenDashes;
		applyDashCountdown = timeBetweenDashes*applyDashTimeMult;
		myEnemyReference.myRigidbody.velocity = Vector3.zero;
		if (Random.Range(0f,1f) < 0.5f){
			dashDirection*= -1f;
		}
		/*myEnemyReference.myRigidbody.AddForce((currentDashTarget-transform.position).normalized
			*dashForce*Time.deltaTime, ForceMode.Impulse);**/
		

	}

	void DetermineAndAddForce(){
		currentDashTarget = (currentPOIPos-myEnemyReference.transform.position).normalized;
		dashNormal = Vector3.zero;
		dashNormal.x = currentDashTarget.y;
		dashNormal.y = -currentDashTarget.x;
		myEnemyReference.myRigidbody.AddForce(dashNormal*dashDirection
			*dashForce*Time.deltaTime, ForceMode.Acceleration);
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
