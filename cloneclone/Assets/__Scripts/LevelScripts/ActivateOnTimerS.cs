using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnTimerS : MonoBehaviour {
	
	public float timeToActivate = 30f;
	public List<GameObject> turnOnObjects;
	public List<GameObject> turnOffObjects;

	private bool turnedOn = false;

	void Update(){
		if (!turnedOn){
		timeToActivate -= Time.deltaTime;
			if (timeToActivate <= 0){
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
