using UnityEngine;
using System.Collections;

public class EnemyWaitBehavior : EnemyBehaviorS {

	[Header("Behavior Duration")]
	public float waitTimeFixed = -1f;
	public float waitTimeMin;
	public float waitTimeMax;

	[Header ("Behavior Physics")]
	public float waitDragAmt = -1;
	public bool setVelocityToZeroOnStart = false;

	private float waitTimeCountdown;


    [Header("Special Case Properties")]
    public bool resetFight = false;
    public float resetFightTimeMult = 0.8f;
    private float resetFightTime = 0f;
    public int maxResets = 2;
    public float minHealthForRewind = 0.5f;
	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){
			
			BehaviorUpdate();

			waitTimeCountdown -= Time.deltaTime*currentDifficultyMult;
			if (waitTimeCountdown <= 0){
				//Debug.Log(behaviorName +" ended bc of time out!" + waitTimeCountdown);
				EndAction();
			}
            if (waitTimeCountdown <= resetFightTime && resetFight){
                myEnemyReference.GetPlayerReference().ResetCombat();
                maxResets--;
            }
		}
	
	}

	private void InitializeAction(){

		if (waitTimeFixed > 0){
			waitTimeCountdown = waitTimeFixed;
		}
		else{
			waitTimeCountdown = Random.Range(waitTimeMin, waitTimeMax);
		}
        resetFightTime = waitTimeCountdown * resetFightTimeMult;
		//Debug.Log(behaviorName +" action started! " + waitTimeCountdown);

		if (waitDragAmt > 0){
			myEnemyReference.myRigidbody.drag = waitDragAmt*EnemyS.FIX_DRAG_MULT;
		}

		if (setVelocityToZeroOnStart){
			myEnemyReference.myRigidbody.velocity = Vector3.zero;
		}

	}

    public override void StartAction(bool setAnimTrigger = true)
    {
        if (!resetFight || (resetFight && maxResets > 0 && myEnemyReference.currentHealth <= myEnemyReference.actingMaxHealth*minHealthForRewind)) { 
            base.StartAction(setAnimTrigger);
            InitializeAction();
        }else{
            EndAction();
        }

	}

	public override void EndAction (bool doNextAction = true)
	{
		base.EndAction (doNextAction);
	}
}
