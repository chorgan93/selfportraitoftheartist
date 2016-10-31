using UnityEngine;
using System.Collections;

public class ZControlTargetS : MonoBehaviour {

	public Transform myTargetRef;

	// Use this for initialization
	void Start () {

		if (myTargetRef == null){
			myTargetRef = transform;
		}
		if (ZControllerS.Z != null){
		ZControllerS.Z.AddTarget(this);
		}
	
	}
}
