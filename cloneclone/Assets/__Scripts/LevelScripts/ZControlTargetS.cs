using UnityEngine;
using System.Collections;

public class ZControlTargetS : MonoBehaviour {

	public Transform myTargetRef;

	// Use this for initialization
	void Start () {

		if (myTargetRef == null){
			myTargetRef = transform;
		}
		ZControllerS.Z.AddTarget(this);
	
	}
}
