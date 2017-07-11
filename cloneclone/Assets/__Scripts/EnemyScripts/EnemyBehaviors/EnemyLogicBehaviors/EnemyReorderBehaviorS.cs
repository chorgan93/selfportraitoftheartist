﻿using UnityEngine;
using System.Collections;

public class EnemyReorderBehaviorS : EnemyBehaviorS {

	public int[] possBehaviorSteps;
	private int behaviorToExecute;

	public override void StartAction(bool setAnimTrigger=true){

		behaviorToExecute = possBehaviorSteps[Mathf.FloorToInt(Random.Range(0, possBehaviorSteps.Length))];
		base.CancelAction();
		myEnemyReference.currentState.behaviorSet[behaviorToExecute].SetEnemy(myEnemyReference);
		myEnemyReference.currentState.behaviorSet[behaviorToExecute].StartAction();
		myEnemyReference.currentState.SetActingBehaviorNum(behaviorToExecute);

	}

	public override void EndAction(bool dontMatter = false){
		base.EndAction(false);
	}
}
