using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnTimerS : MonoBehaviour {
	
	public float timeToActivate = 30f;
	public List<GameObject> turnOnObjects;
	public List<GameObject> turnOffObjects;

	public List<EnemySpawnerS> turnOffEnemies;

	private bool turnedOn = false;

    public CreditsManagerS credits;
    private bool checkForCredits = false;

    private void Start()
    {
        checkForCredits = (credits != null);
    }

	void Update(){
		if (!turnedOn){
            if (checkForCredits)
            {
                if (credits.fastForwarding)
                {
                    timeToActivate -= Time.deltaTime * credits.fastForwardMult;
                }
                else
                {
                    timeToActivate -= Time.deltaTime;
                }
            }
            else
            {
                timeToActivate -= Time.deltaTime;
            }
			if (timeToActivate <= 0){
				TurnOn();
			}
		}
	}

	public void TurnOn(){

		for (int i = 0; i < turnOnObjects.Count; i++){ 
			if (turnOnObjects[i] != null){
				turnOnObjects[i].SetActive(true);
			}
		}
		foreach (GameObject bleh in turnOffObjects){
			bleh.SetActive(false);
		}

		for (int i = 0; i < turnOffEnemies.Count; i++){
			turnOffEnemies[i].currentSpawnedEnemy.gameObject.SetActive(false);
		}

		turnedOn = true;

	}
}
