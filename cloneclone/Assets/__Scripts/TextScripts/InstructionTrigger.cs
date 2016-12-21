using UnityEngine;
using System.Collections;

public class InstructionTrigger : MonoBehaviour {

	public string instructionString;
	private InstructionTextS instructionRef;
	private bool isShowing = false;

	public int turnedOffIfClearedCombat = -1;

	void Start(){

		instructionRef = GameObject.Find("InstructionText").GetComponent<InstructionTextS>();
		instructionString = instructionString.Replace("NEWLINE", "\n");

		if (turnedOffIfClearedCombat > -1){
			if (PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.Contains(turnedOffIfClearedCombat)){
				gameObject.SetActive(false);
			}
		}

	}

	void OnDisable(){
		if (isShowing){
			instructionRef.SetShowing(false);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			instructionRef.SetShowing(true, instructionString);
			isShowing = true;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			instructionRef.SetShowing(false);
			isShowing = false;
		}
	}
}
