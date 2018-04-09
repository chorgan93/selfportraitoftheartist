using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStatusReferencesS : MonoBehaviour {

	public List<EnemySpawnEnemyBehavior> spawnEnemyReferences = new List<EnemySpawnEnemyBehavior>();
	public List<EnemyS> buffedEnemies = new List<EnemyS>();


	public void AddBuffedEnemy(EnemyS newBuff){
		if (!buffedEnemies.Contains(newBuff) && !newBuff.isCorrupted){
			newBuff.SetCorruption(true, true);
			buffedEnemies.Add(newBuff);
		}
	}

	public void RemoveBuffs(){
		for (int i = 0; i < buffedEnemies.Count; i++){
			buffedEnemies[i].ResetCorruption();
		}
		buffedEnemies.Clear();
	}

	public void ResetMessage(){
		if (spawnEnemyReferences.Count > 0){
			for (int i = 0; i < spawnEnemyReferences.Count; i++){
				spawnEnemyReferences[i].ResetSpawn();
			}
		}
		RemoveBuffs();
	}

	public void KillMessage(){
		if (spawnEnemyReferences.Count > 0){
			for (int i = 0; i < spawnEnemyReferences.Count; i++){
				spawnEnemyReferences[i].KillAll();
			}
		}

		RemoveBuffs();
	}

	public void StartWitchTime(){
		if (spawnEnemyReferences.Count > 0){
			for (int i = 0; i < spawnEnemyReferences.Count; i++){
				spawnEnemyReferences[i].SendWitchMessage(true);
			}
		}
	}
	public void EndWitchTime(){
		if (spawnEnemyReferences.Count > 0){
			for (int i = 0; i < spawnEnemyReferences.Count; i++){
				spawnEnemyReferences[i].SendWitchMessage(false);
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
