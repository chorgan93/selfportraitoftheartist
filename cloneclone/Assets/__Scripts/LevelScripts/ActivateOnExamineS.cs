using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateOnExamineS : MonoBehaviour {

	public List<GameObject> turnOnObjects;
	public List<GameObject> turnOffObjects;
	public List<BarrierS> offBarriers;
	public float timeBetweenBarriers = 0.32f;
	public float timeTurnOffBarriers = 0.5f;

	private bool turnedOn = false;


	public void TurnOn(){

		foreach (GameObject eh in turnOnObjects){
			eh.SetActive(true);
		}
		foreach (GameObject bleh in turnOffObjects){
			bleh.SetActive(false);
		}

		int barrierCount = 0;
		foreach(BarrierS o in offBarriers){

			o.delayTurnOffTime = timeBetweenBarriers*offBarriers.Count;
			o.turnOffTime = timeTurnOffBarriers;
			o.TurnOff();
			
			barrierCount++;
		}

			turnedOn = false;

	}
}
