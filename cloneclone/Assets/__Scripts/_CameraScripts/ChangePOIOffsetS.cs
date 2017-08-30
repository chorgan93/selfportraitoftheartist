using UnityEngine;
using System.Collections;

public class ChangePOIOffsetS : MonoBehaviour {

	public bool activateOnStart = false;
	private bool activated = false;
	public Vector3 newOffset = Vector3.zero;

	// Use this for initialization
	void Start () {

		if (activateOnStart){
			ChangePOIOffset();
		}
	
	}
	
	void OnTriggerEnter(Collider other){
		if (!activated){
			if (other.gameObject.tag == "Player"){
				ChangePOIOffset();
			}
		}
	}

	void ChangePOIOffset(){
		CameraPOIS.POI.SetOffset(newOffset);
		activated = true;
	}
}
