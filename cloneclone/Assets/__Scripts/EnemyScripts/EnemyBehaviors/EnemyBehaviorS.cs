using UnityEngine;
using System.Collections;

public class EnemyBehaviorS : MonoBehaviour {

	private EnemyS myEnemy;

	public EnemyS myEnemyReference { get {return myEnemy;}}

	private EnemyBehaviorStateS myBehaviorState;

	private bool _behaviorActing = false;

	public string behaviorName; // mostly for editor legibility
	public string animationKey = "";

	[Header("Status Properties")]
	public bool allowStun = false;
	public bool facePlayer = false;
	public bool dontAllowStateChange = false;
	
	[Header ("Break Properties")]
	public float breakAmt = 9999f;
	public float breakRecoverTime = 1f;


	public virtual void StartAction(bool setAnimTrigger = true){

		_behaviorActing = true;
		myEnemy.SetActing(true);
		myEnemy.SetBehavior(this);
		myEnemy.SetStunStatus(allowStun);
		myEnemy.SetBreakState(breakAmt, breakRecoverTime);
		myEnemy.SetFaceStatus(facePlayer);
		if (animationKey != "" && setAnimTrigger){
		myEnemy.myAnimator.SetTrigger(animationKey);
		}



	}

	public virtual void EndAction(){

		_behaviorActing = false;
		myEnemy.SetActing(false);

		myEnemy.CheckBehaviorStateSwitch(dontAllowStateChange);
		

	}

	public virtual void SetEnemy(EnemyS newEnemy){
		myEnemy = newEnemy;
	}

	public virtual void SetState(EnemyBehaviorStateS myState){
		myBehaviorState = myState;
	}

	public virtual bool BehaviorActing(){
		return (_behaviorActing && !myEnemy.isCritical && !myEnemy.hitStunned);
	}
}
