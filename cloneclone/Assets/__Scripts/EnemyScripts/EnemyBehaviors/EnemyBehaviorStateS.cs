using UnityEngine;
using System.Collections;

public class EnemyBehaviorStateS : MonoBehaviour {

    // class to handle sequencing individual enemy behaviors (actions)

    //_________________________________________________BASIC PROPERTIES

	private EnemyS myEnemy; // reference to my individual enemy stat manager
	public string stateName; // name of state for debug purposes


    //_________________________________________________STATE REQUIREMENTS

	public bool activeState = true; // whether enemy needs to be active/inactive for state to run
	public PlayerDetectS minDistance; // null = runs any distance, otherwise = whether minDistance detects a target

	public float minHealthPercentage = -1f; // health % required, -1 = non-health dependent, otherwise must be below 1-100% (inclusive)
	public float activateAtEnemyActiveTime = -1f; // activate this state after X seconds when set >=0
	public float activateAtPlayerHealth = -1f; // activate this state at player health <X when set >=0

    //_________________________________________________SEQUENCING PROPERTIES

	public bool onlyActOnce = false; // when TRUE, state will only happen once in full
	public bool resetActOnceOnReset = false; // if TRUE, _doNotActAgain becomes FALSE if combat state resets

	private bool _doNotActAgain = false; // becomes TRUE onlyActOnce has been met
	public bool doNotActAgain { get { return _doNotActAgain; } }

	public bool overrideImmediate = false; // if TRUE, begins immediately once conditions are met
	public bool stateIgnoresHitstun = false; // if TRUE, state cannot be interrupted by hitstun

    //_________________________________________________BEHAVIOR PROPERTIES

	public EnemyBehaviorS[] behaviorSet; // sequence of individual behaviors that comprise this state
    public EnemyDodgeBehaviorS overrideDodge; // replaces preset dodge behaviors, if needed
    public int critResetStep = 0; // when this state is interrupted, which behavior it resets to

	private int currentActingBehavior = 0;
	public int currentBehaviorStep { get { return currentActingBehavior; } }

    public bool debugBehaviorState = false; // if TRUE, will output behavior changes to console
	

	public bool isActive(){

        // check that occurs when enemy is determining current behavior state
        // returns TRUE when enemy should run this behavior state

		bool active = true;

		// do checks for all conditions

        // determine if enemy is in correct activation state
		if (myEnemy.isActive != activeState){

			active = false;

		}

        // determine if distance to player/target requirement is met
		if (minDistance != null){
			minDistance.FindTarget();
			if (!minDistance.PlayerInRange()){
				active = false;
			}
		}

        // determine if enemy meets health requirement, if applicable
		if (minHealthPercentage > 0 && myEnemy.currentHealth/myEnemy.actingMaxHealth > minHealthPercentage){
			active = false;
		}

        // the following are special activation triggers for a special boss fight ending (Messiah 1)

        // check if activation triggers are met in re: enemy active time and player health requirements
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

		// after all checks, if active = TRUE && immediate == TRUE, force state switch to this state
		if (active && overrideImmediate){
			myEnemy.ForceBehaviorState(this);
		}

		return active;

	}

	public void StartActions(bool fromCrit = false, bool fromReset = false){

        // called when behavior state starts acting

        // if enemy state was interrupted, reset to appropriate step
		if (fromCrit || fromReset){
			currentActingBehavior = critResetStep;
		}else{
            // if not, start at beginning step
		    currentActingBehavior = 0;
		}

        // replace default dodge behaviors, if appropriate
        if (overrideDodge){
            if (behaviorSet[currentActingBehavior] is EnemySingleAttackBehavior){
                behaviorSet[currentActingBehavior].GetComponent<EnemySingleAttackBehavior>().dodgeCheck = overrideDodge;
            }
        }

        // start appropriate enemy behavior
		behaviorSet[currentActingBehavior].StartAction(); 


        if (debugBehaviorState)
        {
            // output acting behavior name to console when needed
            Debug.Log("Starting behavior: " + behaviorSet[currentActingBehavior].behaviorName, myEnemy.gameObject);
        }

    }

	public void NextBehavior(){

        // end current behavior and move onto next in sequence

        // advance behavior
		behaviorSet[currentActingBehavior].EndAction(false);
		currentActingBehavior++;

        // if at end of sequence, move onto next if state should only act once
		if (currentActingBehavior > behaviorSet.Length-1){
			if (onlyActOnce){
				_doNotActAgain = true;
				myEnemy.CheckBehaviorStateSwitch(false);
			}else{
                // otherwise, loop back to the beginning
				currentActingBehavior = 0;
			}
		}

        // start next action in behavior sequence
		if (!_doNotActAgain){
			behaviorSet[currentActingBehavior].StartAction();
            if (debugBehaviorState){
                Debug.Log("Starting behavior: " + behaviorSet[currentActingBehavior].behaviorName, myEnemy.gameObject);
            }
		}

	}

	public void EndBehavior(bool doNext = true){

        // ends current action, then begins the next in the sequence if doNext = TRUE

		if (currentActingBehavior < behaviorSet.Length){
			behaviorSet[currentActingBehavior].EndAction(doNext);
		}
	}

	public void SetEnemy(EnemyS enemy, bool fromReset = false){

        // initialize this behavior state and link to individual enemy manager

		myEnemy = enemy;

		if (fromReset && resetActOnceOnReset){
			_doNotActAgain = false;
		}

		foreach(EnemyBehaviorS behavior in behaviorSet){
			behavior.SetEnemy(enemy); // set individual behavior's enemy reference
			behavior.SetState(this); // set individual behavior's state to this state
		}

	}

	public void InitializeActions(){

        // initialize all individual actions so they have proper parameters set

        foreach (EnemyBehaviorS behavior in behaviorSet){
			behavior.SetState(this);
		}

	}

	public void SetActingBehaviorNum(int newB){

        // update behavior step

		currentActingBehavior = newB;
	}

	public void SetTargetBehavior(int newBehavior){

        // sets new enemy behavior to a specific set in the sequence

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

        // stops all acting actions without advancing the sequence
        // used for sequence-breaking behaviors, such as reordering or responding to player actions

		for (int i = 0; i < behaviorSet.Length; i++){
			if (behaviorSet[i].behaviorActive){
				behaviorSet[i].CancelAction();
			}
		}
	}
}
