using UnityEngine;
using System.Collections;

public class InstructionTrigger : MonoBehaviour {

	public string instructionString;
	public string noControllerInstructionString = "";
	private InstructionTextS instructionRef;
	private bool isShowing = false;

	[Header("Turn Off Conditions")]
	public int turnedOffIfClearedCombat = -1;
	public bool menuInstruction = false;

	void Start(){

		instructionRef = GameObject.Find("InstructionText").GetComponent<InstructionTextS>();
		instructionString = instructionString.Replace("NEWLINE", "\n");
		if (noControllerInstructionString != ""){
			noControllerInstructionString = noControllerInstructionString.Replace("NEWLINE", "\n");
		}

		if (turnedOffIfClearedCombat > -1){
			if (PlayerInventoryS.I.dManager.combatClearedAtLeastOnce != null){
				if (PlayerInventoryS.I.dManager.combatClearedAtLeastOnce.Contains(turnedOffIfClearedCombat)){
					gameObject.SetActive(false);
				}
			}
		}

	}

	void Update(){
		if (menuInstruction){
			if (InGameMenuManagerS.hasUsedMenu){
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
			if (noControllerInstructionString != "" && !other.GetComponent<PlayerController>().myControl.ControllerAttached()){
				instructionRef.SetShowing(true, noControllerInstructionString);
			}else{
				instructionRef.SetShowing(true, instructionString);
			}
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
