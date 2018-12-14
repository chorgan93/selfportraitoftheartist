using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyReorderBehaviorS : EnemyBehaviorS {

	public int[] possBehaviorSteps;
	private int selectedBehaviorStep = 0;
	private List<EnemyBehaviorS> possBehaviors = new List<EnemyBehaviorS>();
	private int behaviorToExecute;

    public bool debugReorder;

	public override void StartAction(bool setAnimTrigger=false){

		if (possBehaviors.Count <= 0){
			for (int i = 0; i < possBehaviorSteps.Length; i++){
				possBehaviors.Add(stateRef.behaviorSet[possBehaviorSteps[i]]);
			}
		}

		// activate behavior
		behaviorToExecute = Mathf.FloorToInt(Random.Range(0, possBehaviors.Count));
		possBehaviors[behaviorToExecute].SetEnemy(myEnemyReference);
		//Debug.LogError("Reorder to: " + possBehaviors[behaviorToExecute].behaviorName);

		// Debug.Log("Current reorder step: " + stateRef.currentBehaviorStep);

		// set state current behavior int 
		selectedBehaviorStep = possBehaviorSteps[behaviorToExecute];
			stateRef.SetActingBehaviorNum(selectedBehaviorStep);
		//myEnemyReference.OverrideSpacingRequirement = true;
			possBehaviors[behaviorToExecute].StartAction();

        if (debugReorder) { Debug.Log("set current behavior to step " + selectedBehaviorStep, myEnemyReference.gameObject); }

	}

	public int GetNewBehaviorInt(){
		return (possBehaviorSteps[Mathf.FloorToInt(Random.Range(0, possBehaviorSteps.Length))]);
	}
}
