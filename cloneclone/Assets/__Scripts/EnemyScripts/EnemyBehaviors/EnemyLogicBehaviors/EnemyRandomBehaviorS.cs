﻿using UnityEngine;
using System.Collections;

public class EnemyRandomBehaviorS : EnemyBehaviorS {

	public EnemyBehaviorS[] possBehaviors;
	private int behaviorToExecute;

	public override void StartAction(bool setAnimTrigger=true){

		behaviorToExecute = Mathf.FloorToInt(Random.Range(0, possBehaviors.Length));
		possBehaviors[behaviorToExecute].SetEnemy(myEnemyReference);
		possBehaviors[behaviorToExecute].StartAction();

	}
}
