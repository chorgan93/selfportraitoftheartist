using UnityEngine;
using System.Collections;

public class EnemyDodgeBehaviorS : EnemyBehaviorS {

	[Header("Dodge Properties")]
	public float dodgeForce;
	public float dodgeDragAmt;
	public float slideDragAmt;
	public float dodgeMaxTime;
	public float triggerSlideTime;
	public float invulnerablePercentTime;
	private float dodgeCount;
	private float invulnerableTime;

	private bool setDrag = false;

	[Header("Follow Up Properties")]
	public int[] possBehaviorSteps;
	private int behaviorToExecute;

	void InitializeAction(){

		myEnemyReference.myRigidbody.velocity = Vector3.zero;
		myEnemyReference.SetInvulnerable(true);
		dodgeCount = 0f;
		myEnemyReference.myRigidbody.drag = dodgeDragAmt;

		invulnerableTime = invulnerablePercentTime*dodgeMaxTime;

		Vector3 dodgeAccel = Random.insideUnitSphere.normalized;
		dodgeAccel.z = 0f;
		myEnemyReference.myRigidbody.AddForce(dodgeAccel*dodgeForce);
		setDrag = false;

	}

	void FixedUpdate(){
		DodgeUpdate();
	}

	void DodgeUpdate(){
		if (BehaviorActing()){

			BehaviorUpdate();

			dodgeCount += Time.deltaTime;
			if (myEnemyReference.invulnerable && dodgeCount >= invulnerableTime){
				myEnemyReference.SetInvulnerable(false);
			}

			if (dodgeCount >= triggerSlideTime && !setDrag){
				myEnemyReference.myRigidbody.drag = slideDragAmt;
				setDrag = true;
			}

			if (dodgeCount >= dodgeMaxTime || myEnemyReference.behaviorBroken){
				EndAction();
			}

		}
	}

	public override void StartAction(bool setAnimTrigger=true){

		base.StartAction();
		InitializeAction();

	}

	public override void EndAction (bool doNextAction = false)
	{
		myEnemyReference.SetInvulnerable(false);
		if (possBehaviorSteps.Length > 0){
			base.EndAction (doNextAction);
			behaviorToExecute = possBehaviorSteps[Mathf.FloorToInt(Random.Range(0, possBehaviorSteps.Length))];
			myEnemyReference.currentState.behaviorSet[behaviorToExecute].SetEnemy(myEnemyReference);
			myEnemyReference.currentState.behaviorSet[behaviorToExecute].StartAction();
		}else{
			base.EndAction();
		}
	}
}
