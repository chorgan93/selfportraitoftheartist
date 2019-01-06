using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTrainS : MonoBehaviour {

    public TrainCarS targetTrain;

	// Use this for initialization
	void Start () {
        targetTrain.EndTrainExternal();
	}
}
