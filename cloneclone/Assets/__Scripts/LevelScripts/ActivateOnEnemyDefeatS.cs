using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnEnemyDefeatS : MonoBehaviour {

	public List<GameObject> turnOnObjects;
	public List<GameObject> turnOffObjects;
	public List<BarrierS> turnOffBarriers;

	public List<EnemyS> checkEnemies;
	public List<EnemySpawnerS> checkEnemySpawners;
	public List<DestructibleItemS> checkDestruction;
	int defeatedEnemies = 0;

	private bool turnedOn = false;



	void Update () {

		if (!turnedOn){
			defeatedEnemies = 0;

			for (int i = 0; i < checkEnemies.Count; i++){
				if (checkEnemies[i].isDead){
					defeatedEnemies++;
				}
			}
			for (int j = 0; j < checkEnemySpawners.Count; j++){
				if (checkEnemySpawners[j].sentMessage || 
					(checkEnemySpawners[j].enemySpawnID > -1 && PlayerInventoryS.I.dManager.enemiesDefeated.Contains(checkEnemySpawners[j].enemySpawnID))){
					defeatedEnemies++;
				}
			}
			for (int k = 0; k < checkDestruction.Count; k++){
				if (checkDestruction[k].destroyed){
					defeatedEnemies++;
				}
			}

			if (defeatedEnemies >= (checkEnemies.Count+checkEnemySpawners.Count+checkDestruction.Count)){
				TurnOnOff();
			}
		}

	}

	void TurnOnOff(){
			
			foreach (BarrierS bleh in turnOffBarriers){
				bleh.TurnOff();
			}

		foreach (GameObject eh in turnOnObjects){
			eh.SetActive(true);
		}

		foreach (GameObject ne in turnOffObjects){
			ne.SetActive(false);
		}

			turnedOn = true;

	}
}
