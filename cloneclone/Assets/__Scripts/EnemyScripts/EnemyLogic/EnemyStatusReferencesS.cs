using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStatusReferencesS : MonoBehaviour {

	public List<EnemySpawnEnemyBehavior> spawnEnemyReferences = new List<EnemySpawnEnemyBehavior>();


	public void ResetMessage(){
		if (spawnEnemyReferences.Count > 0){
			for (int i = 0; i < spawnEnemyReferences.Count; i++){
				spawnEnemyReferences[i].ResetSpawn();
			}
		}
	}

	public void KillMessage(){
		if (spawnEnemyReferences.Count > 0){
			for (int i = 0; i < spawnEnemyReferences.Count; i++){
				spawnEnemyReferences[i].KillAll();
			}
		}
	}

	public void FeatherMessage(Color swapCol){
		if (spawnEnemyReferences.Count > 0){
			for (int i = 0; i < spawnEnemyReferences.Count; i++){
				spawnEnemyReferences[i].FeatherAll(swapCol);
			}
		}
	}
}
