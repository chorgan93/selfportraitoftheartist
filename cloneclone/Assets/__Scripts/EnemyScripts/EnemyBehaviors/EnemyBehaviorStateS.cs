﻿using UnityEngine;
using System.Collections;

public class EnemyBehaviorStateS : MonoBehaviour {

	private EnemyS myEnemy;

	public string stateName;

	public bool activeState = true; // whether enemy needs to be active/inactive

	public PlayerDetectS minDistance; // null = any distance, otherwise = PlayerDetectS state

	public float minHealthPercentage = -1f; // -1 = not health dependent, otherwise 1-100 (inclusive)

	public bool overrideImmediate = false; // if TRUE, begins immediately once conditions are met

	public bool stateIgnoresHitstun = false; // if TRUE, cannot be interrupted by hitstun

	public EnemyBehaviorS[] behaviorSet;

	private int currentActingBehavior = 0;
	

	public bool isActive(){

		bool active = true;

		// do checks for all conditions
		if (myEnemy.isActive != activeState){

			active = false;

		}

		if (minDistance != null){
			if (!minDistance.PlayerInRange()){
				active = false;
			}
		}

		if (minHealthPercentage > 0 && myEnemy.currentHealth/myEnemy.maxHealth*100f <= minHealthPercentage){
			active = false;
		}

		// after all checks, if active = true && immediate == true, force mode switch
		if (active && overrideImmediate){
			myEnemy.ForceBehaviorState(this);
		}

		return active;

	}

	public void StartActions(){

		currentActingBehavior = 0;
		behaviorSet[currentActingBehavior].StartAction();

	}

	public void NextBehavior(){

		currentActingBehavior++;
		if (currentActingBehavior > behaviorSet.Length-1){
			currentActingBehavior = 0;
		}
		behaviorSet[currentActingBehavior].StartAction();

	}

	public void EndBehavior(){
		behaviorSet[currentActingBehavior].EndAction();
	}

	public void SetEnemy(EnemyS enemy){

		myEnemy = enemy;

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
}
