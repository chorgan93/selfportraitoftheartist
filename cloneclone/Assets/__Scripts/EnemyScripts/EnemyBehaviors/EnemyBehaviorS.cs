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

	[Header("Vulnerable Properties")]
	public float vulnerableDuration = -1f;
	public float vulnerableDelay = 0f;


	public virtual void StartAction(){

		_behaviorActing = true;
		myEnemy.SetActing(true);
		myEnemy.SetBehavior(this);
		myEnemy.SetStunStatus(allowStun);
		myEnemy.SetFaceStatus(facePlayer);
		if (animationKey != ""){
		myEnemy.myAnimator.SetTrigger(animationKey);
		}

		if (vulnerableDuration > 0){
			myEnemy.SetVulnerableTiming(vulnerableDuration, vulnerableDelay);
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
