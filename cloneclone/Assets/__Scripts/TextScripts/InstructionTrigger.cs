using UnityEngine;
using System.Collections;

public class InstructionTrigger : MonoBehaviour {

	public string instructionString;
	private InstructionTextS instructionRef;

	void Start(){

		instructionRef = GameObject.Find("InstructionText").GetComponent<InstructionTextS>();

	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			instructionRef.SetShowing(true, instructionString);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			instructionRef.SetShowing(false);
		}
	}
}
