using UnityEngine;
using System.Collections;

public class EnemyBehaviorS : MonoBehaviour {

	private EnemyS myEnemy;

	public EnemyS myEnemyReference { get {return myEnemy;}}

	private EnemyBehaviorStateS myBehaviorState;
	public EnemyBehaviorStateS stateRef { get { return myBehaviorState; } }

	private float behaviorActTime = 0f;

	private bool _behaviorActing = false;
	public bool behaviorActive { get { return _behaviorActing; } }

	[Header("Behavior Properties")]
	public string behaviorName; // mostly for editor legibility
	public string animationKey = "";
	public float[] difficultyMult = new float[]{0.9f,1f,1.1f,1.2f};
	[HideInInspector]
	public float currentDifficultyMult;
	public GameObject soundObj;

	[Header("Status Properties")]
	public bool allowStun = false;
	public bool facePlayer = false;
	public bool dontAllowStateChange = false;
	public float defenseMult = 1f;
	public float stunResistMult = 1f;
	public bool setInvincible = false;
	public bool ignorePush = false;
	
	[Header ("Break Properties")]
	public float breakAmt = 9999f;
	public float breakRecoverTime = 1f;
	public float allowParryStartTime = -1f;
	public float allowParryEndTime = -1f;
	
	[Header ("Effect Properties")]
	public GameObject signalObj;
	public EnemyGiveSpawnCommandS spawnCommand;


	public virtual void StartAction(bool setAnimTrigger = true){

		_behaviorActing = true;
		myEnemy.SetActing(true);
		myEnemy.SetBehavior(this);
		myEnemy.SetStunStatus(allowStun);
		myEnemy.SetBreakState(breakAmt, breakRecoverTime);
		myEnemy.SetFaceStatus(facePlayer);
		myEnemy.SetInvulnerable(setInvincible);
		myEnemy.RefreshTarget();
		myEnemy.ResetFaceLock();

		if (spawnCommand){
			spawnCommand.GiveCommand();
		}

		myEnemy.ignorePush = ignorePush;

		myEnemy.SetStateDefenses(defenseMult, stunResistMult);
		myEnemy.ResetAttackCount();

		behaviorActTime = 0f;

		currentDifficultyMult = difficultyMult[DifficultyS.GetSinInt(myEnemyReference.isGold)];
		myEnemy.myAnimator.SetFloat("DifficultySpeed", currentDifficultyMult);
		myEnemy.currentDifficultyAnimationFloat = currentDifficultyMult;

		if (animationKey != "" && setAnimTrigger){
			myEnemy.myAnimator.SetTrigger(animationKey);
			//Debug.Log("Behavior " + behaviorName + " is attempting to send trigger " + animationKey + " through base.StartAction()");
			
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
			
		#if UNITY_EDITOR
		if (myEnemy.debugMark){
			string debugString = myEnemyReference.enemyName + " " + behaviorName + " behavior set!"
				+"\nCurrent Behavior Set: " + myEnemyReference.currentState.stateName + "\nActive Behaviors: " 
				+ myEnemyReference.GetNumberOfActiveBehaviors() + "\n" + myEnemyReference.GetNamesOfActiveBehaviors();
			Debug.Log(debugString, myEnemy.gameObject);
		}
		#endif


	}

	public virtual void BehaviorUpdate(){
		if (_behaviorActing){
			behaviorActTime += Time.deltaTime;
			if (behaviorActTime >= allowParryStartTime/currentDifficultyMult && behaviorActTime <= allowParryEndTime/currentDifficultyMult 
				&& !myEnemy.isCritical){
				myEnemy.canBeParried = true;
			}else{
				myEnemy.canBeParried = false;
			}
		}
	}

	public virtual void EndAction(bool doNextAction = true){

		_behaviorActing = false;
		myEnemy.SetActing(false);

		myEnemy.canBeParried = false;
		myEnemy.OverrideSpacingRequirement = false;

		if (doNextAction){

			myEnemy.CheckBehaviorStateSwitch(dontAllowStateChange);

		}

	}

	public virtual void CancelAction(){
		_behaviorActing = false;
		myEnemy.SetActing(false);
		//myEnemy.SetBehavior(null);
		myEnemy.canBeParried = false;
		myEnemy.OverrideSpacingRequirement = false;
	}

	public void SetBehaviorActing(bool newAct){
		_behaviorActing = newAct;
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
