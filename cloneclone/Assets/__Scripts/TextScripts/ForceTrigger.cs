using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForceTrigger : MonoBehaviour {

	public Vector3 forceToApply;
	private List<PlayerController> playersInRange;


	// Use this for initialization
	void Start () {

		playersInRange = new List<PlayerController>();
	
	}

	void FixedUpdate(){
		if (playersInRange.Count > 0){
			for (int i = 0; i < playersInRange.Count; i++){
				playersInRange[i].myRigidbody.AddForce(forceToApply*Time.deltaTime, ForceMode.Acceleration);
			}
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			if (other.gameObject.GetComponent<PlayerController>() != null){
				playersInRange.Add(other.gameObject.GetComponent<PlayerController>());
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			if (other.gameObject.GetComponent<PlayerController>() != null){
				playersInRange.Remove(other.gameObject.GetComponent<PlayerController>());
			}
		}
	}
}
