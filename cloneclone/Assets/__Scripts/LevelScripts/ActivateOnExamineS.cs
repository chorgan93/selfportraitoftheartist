using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnExamineS : MonoBehaviour {

	public List<GameObject> turnOnObjects;
	public List<GameObject> turnOffObjects;

	private bool turnedOn = false;


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
