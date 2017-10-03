using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGiveSpawnCommandS : EnemyBehaviorS {

	public EnemySpawnEnemyBehavior targetSpawns;
	public int commandStepToGive = 0;

	//assigned in other behaviors

	public void GiveCommand(int minCommand = 9999){
		if(minCommand <= 0){
			minCommand = 9999;
		}

		int currentCommand = 0;
		// give command, then move on
		if (targetSpawns.EnemiesAreActive()){
			for (int i = 0; i < targetSpawns.spawnReferences.Length; i++){
				if (targetSpawns.spawnReferences[i].SpawnedEnemyIsActive() 
					&& targetSpawns.spawnReferences[i].currentSpawnedEnemy.currentState != null
					&& currentCommand < minCommand){
					targetSpawns.spawnReferences[i].currentSpawnedEnemy.currentState.SetTargetBehavior(commandStepToGive);
					currentCommand++;
				}
			}
		}

		//EndAction();

	}

	public int NumSpawnsActive(){
		int numSpawned = 0;
		for (int i = 0; i < targetSpawns.spawnReferences.Length; i++){
			if (targetSpawns.spawnReferences[i].enemySpawned){
				if (!targetSpawns.spawnReferences[i].currentSpawnedEnemy.isDead
					&& targetSpawns.spawnReferences[i].currentSpawnedEnemy.gameObject.activeSelf){
					numSpawned++;
				}
			}
		}
		return numSpawned;
	}

}
