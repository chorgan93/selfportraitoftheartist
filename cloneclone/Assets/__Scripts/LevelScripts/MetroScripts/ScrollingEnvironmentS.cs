using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingEnvironmentS : MonoBehaviour {

    public float travelDistance = 50f;
    public Vector3 travelDirection = Vector3.zero;
    public float currentTravelProgress = 0f;
    public float travelRate = 10f;
    private Vector3 startPosition;

    public bool disableOnStart = true;

	// Use this for initialization
	void Start () {

        startPosition = transform.position;
        EvaluatePosition();

        enabled = !disableOnStart;
    }
	
	// Update is called once per frame
	void Update () {
        currentTravelProgress += travelRate * Time.deltaTime * WitchMult();
        if (currentTravelProgress >= travelDistance){
            currentTravelProgress -= travelDistance;
        }
        EvaluatePosition();
	}

    void EvaluatePosition(){

        transform.position = startPosition + travelDirection * currentTravelProgress;

    }

    float WitchMult(){
        if (PlayerSlowTimeS.witchTimeActive){
            return .02f;
        }else{
            return 1f;
        }
    }
}
