using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnTouchS : MonoBehaviour {

	public List<GameObject> turnOnObjects;
	public List<GameObject> turnOffObjects;

	private bool turnedOn = false;


	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" && !turnedOn){

			foreach (GameObject eh in turnOnObjects){
				eh.SetActive(true);
			}
			foreach (GameObject bleh in turnOffObjects){
				bleh.SetActive(false);
			}

			turnedOn = false;
		}
	}
}
