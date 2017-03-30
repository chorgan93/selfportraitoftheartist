using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnPlayerHealthS : MonoBehaviour {

	public PlayerStatsS playerRef;
	
	public float healthToActivate = 0.2f;
	public List<GameObject> turnOnObjects;
	public List<GameObject> turnOffObjects;

	private bool turnedOn = false;

	void Update(){
		if (!turnedOn){
			if (playerRef.currentHealth <= healthToActivate){
				TurnOn();
			}
		}
	}

	public void TurnOn(){

		foreach (GameObject eh in turnOnObjects){
			eh.SetActive(true);
		}
		foreach (GameObject bleh in turnOffObjects){
			bleh.SetActive(false);
		}

			turnedOn = false;

	}
}
