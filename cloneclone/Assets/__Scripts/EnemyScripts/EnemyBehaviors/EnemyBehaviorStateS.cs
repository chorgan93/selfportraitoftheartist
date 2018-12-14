using UnityEngine;
using System.Collections;

public class EnemyBehaviorStateS : MonoBehaviour {

	private EnemyS myEnemy;

	public string stateName;

	public bool activeState = true; // whether enemy needs to be active/inactive

	public PlayerDetectS minDistance; // null = any distance, otherwise = PlayerDetectS state

	public float minHealthPercentage = -1f; // -1 = not health dependent, otherwise 1-100 (inclusive)
	public float activateAtEnemyActiveTime = -1f;
	public float activateAtPlayerHealth = -1f;

	public bool onlyActOnce = false;
	public bool resetActOnceOnReset = false;
	private bool _doNotActAgain = false;
	public bool doNotActAgain { get { return _doNotActAgain; } }

	public bool overrideImmediate = false; // if TRUE, begins immediately once conditions are met

	public bool stateIgnoresHitstun = false; // if TRUE, cannot be interrupted by hitstun

    public EnemyDodgeBehaviorS overrideDodge;

	public EnemyBehaviorS[] behaviorSet;
	public int critResetStep = 0;

	private int currentActingBehavior = 0;
	public int currentBehaviorStep { get { return currentActingBehavior; } }

    public bool debugBehaviorState = false;
	

	public bool isActive(){

		bool active = true;

		// do checks for all conditions
		if (myEnemy.isActive != activeState){

			active = false;

		}

		if (minDistance != null){
			minDistance.FindTarget();
			if (!minDistance.PlayerInRange()){
				active = false;
			}
		}

		if (minHealthPercentage > 0 && myEnemy.currentHealth/myEnemy.actingMaxHealth > minHealthPercentage){
			active = false;
		}

		// the following are special activation triggers for first messiah fight ending
		if (activateAtEnemyActiveTime > -1f && activateAtPlayerHealth > -1){
			if (activateAtEnemyActiveTime > myEnemy.enemyActiveTime){
				active = false;
			}
			else if (myEnemy.GetPlayerReference() != null){
				if (activateAtPlayerHealth < myEnemy.GetPlayerReference().myStats.currentHealth){
					active = false;
				}
			}
		}

		// after all checks, if active = true && immediate == true, force mode switch
		if (active && overrideImmediate){
			myEnemy.ForceBehaviorState(this);
		}

		return active;

	}

	public void StartActions(bool fromCrit = false, bool fromReset = false){

		if (fromCrit || fromReset){
			currentActingBehavior = critResetStep;
		}else{
		currentActingBehavior = 0;
		}
        if (overrideDodge){
            if (behaviorSet[currentActingBehavior] is EnemySingleAttackBehavior){
                behaviorSet[currentActingBehavior].GetComponent<EnemySingleAttackBehavior>().dodgeCheck = overrideDodge;
            }
        }
		behaviorSet[currentActingBehavior].StartAction(); 
        if (debugBehaviorState)
        {
            Debug.Log("Starting behavior: " + behaviorSet[currentActingBehavior].behaviorName, myEnemy.gameObject);
        }

    }

	public void NextBehavior(){

		behaviorSet[currentActingBehavior].EndAction(false);
		currentActingBehavior++;
		if (currentActingBehavior > behaviorSet.Length-1){
			if (onlyActOnce){
				_doNotActAgain = true;
				myEnemy.CheckBehaviorStateSwitch(false);
			}else{
				currentActingBehavior = 0;
			}
		}

		if (!_doNotActAgain){
			behaviorSet[currentActingBehavior].StartAction();
            if (debugBehaviorState){
                Debug.Log("Starting behavior: " + behaviorSet[currentActingBehavior].behaviorName, myEnemy.gameObject);
            }
		}

	}

	public void EndBehavior(bool doNext = true){
		if (currentActingBehavior < behaviorSet.Length){
			behaviorSet[currentActingBehavior].EndAction(doNext);
		}
	}

	public void SetEnemy(EnemyS enemy, bool fromReset = false){

		myEnemy = enemy;

		if (fromReset && resetActOnceOnReset){
			_doNotActAgain = false;
		}

		foreach(EnemyBehaviorS behavior in behaviorSet){
			behavior.SetEnemy(enemy);
			behavior.SetState(this);
		}

	}

	public void InitializeActions(){

		foreach(EnemyBehaviorS behavior in behaviorSet){
			behavior.SetState(this);
		}

	}

	public void SetActingBehaviorNum(int newB){
		currentActingBehavior = newB;
	}

	public void SetTargetBehavior(int newBehavior){
		if (newBehavior < behaviorSet.Length){
			CancelAllActions();
			SetActingBehaviorNum(newBehavior);
			behaviorSet[newBehavior].StartAction(); 
            if (debugBehaviorState)
            {
                Debug.Log("Starting behavior: " + behaviorSet[newBehavior].behaviorName, myEnemy.gameObject);
            }
        }
	}

	public void CancelAllActions(){
		for (int i = 0; i < behaviorSet.Length; i++){
			if (behaviorSet[i].behaviorActive){
				behaviorSet[i].CancelAction();
			}
		}
	}
}
