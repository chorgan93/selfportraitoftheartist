using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGiveSpawnCommandS : EnemyBehaviorS {

	public EnemySpawnEnemyBehavior targetSpawns;
	private int commandStepToGive = 0;

	public override void StartAction(bool setAnimTrigger=false){

		// give command, then move on
		if (targetSpawns.EnemiesAreActive()){
			for (int i = 0; i < targetSpawns.spawnReferences.Length; i++){
				if (targetSpawns.spawnReferences[i].SpawnedEnemyIsActive() 
					&& targetSpawns.spawnReferences[i].currentSpawnedEnemy.currentState != null){
					targetSpawns.spawnReferences[i].currentSpawnedEnemy.currentState.SetTargetBehavior(commandStepToGive);
				}
			}
		}

		EndAction();

	}

}
