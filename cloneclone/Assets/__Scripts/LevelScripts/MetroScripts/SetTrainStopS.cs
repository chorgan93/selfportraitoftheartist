using UnityEngine;
using System.Collections;

public class SetTrainStopS : MonoBehaviour {

	public int trainStop = -1;
	public int trainDirection = 1;

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			TrainCarS.currentDirection = trainDirection;
			TrainCarS.currentStop = trainStop;
		}
	}
}
