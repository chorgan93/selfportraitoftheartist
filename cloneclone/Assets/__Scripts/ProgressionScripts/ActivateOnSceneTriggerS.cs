using UnityEngine;
using System.Collections;

public class ActivateOnSceneTriggerS : MonoBehaviour {

	public GameObject[] turnOnObjects;
	public GameObject[] turnOffObjects;


	public void TurnOnOff () {
		for (int i = 0; i < turnOnObjects.Length; i++){
			turnOnObjects[i].SetActive(true);
		}
		for (int i = 0; i < turnOffObjects.Length; i++){
			turnOnObjects[i].SetActive(false);
		}
	}
}
