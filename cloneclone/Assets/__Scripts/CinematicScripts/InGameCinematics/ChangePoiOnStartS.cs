using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePoiOnStartS : MonoBehaviour {

    public GameObject newPOI;

	// Use this for initialization
	void Start () {
        CameraFollowS.F.SetNewPOI(newPOI);
	}
	
}
