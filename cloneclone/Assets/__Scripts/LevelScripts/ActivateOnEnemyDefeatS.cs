using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnEnemyDefeatS : MonoBehaviour {

	public List<GameObject> turnOnObjects;
	public List<GameObject> turnOffObjects;
	public List<BarrierS> turnOffBarriers;

	public List<EnemyS> checkEnemies;
	int defeatedEnemies = 0;

	private bool turnedOn = false;


	void Update () {

		if (!turnedOn){
			defeatedEnemies = 0;

			foreach (EnemyS e in checkEnemies){
				if (e.isDead){
					defeatedEnemies++;
				}
			}

			if (defeatedEnemies >= checkEnemies.Count){
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
