using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnCombatS : MonoBehaviour {

	public List<GameObject> turnOnObjects;
    public List<GameObject> turnOffObjects;
    public List<EnemySpawnerS> turnOffEnemies;

	private bool turnedOn = false;


	public void Activate(){
		if (!turnedOn){

			foreach (GameObject eh in turnOnObjects){
				eh.SetActive(true);
			}
			foreach (GameObject bleh in turnOffObjects){
				bleh.SetActive(false);
			}
            foreach (EnemySpawnerS meh in turnOffEnemies)
            {
                if (meh.currentSpawnedEnemy){
                    meh.currentSpawnedEnemy.gameObject.SetActive(false);
                }
            }

			turnedOn = true;
		}
	}
}
