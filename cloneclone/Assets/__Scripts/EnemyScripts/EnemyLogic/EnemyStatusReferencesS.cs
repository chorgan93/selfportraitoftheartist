using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStatusReferencesS : MonoBehaviour {

	public List<EnemySpawnEnemyBehavior> spawnEnemyReferences = new List<EnemySpawnEnemyBehavior>();
	public List<EnemyS> buffedEnemies = new List<EnemyS>();


	void LateUpdate(){
		CleanBuffList();
	}

	void CleanBuffList(){
		if (buffedEnemies.Count > 0){
			for (int i = buffedEnemies.Count-1; i >= 0; i--){
				if (!buffedEnemies[i].isCorrupted){
					buffedEnemies.RemoveAt(i);
				}
			}
		}
	}

	public void AddBuffedEnemy(EnemyS newBuff){
		if (!buffedEnemies.Contains(newBuff) && !newBuff.isCorrupted){
			buffedEnemies.Add(newBuff);
		}
		newBuff.SetCorruption(true);
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
