using UnityEngine;
using System.Collections;

public class InstructionTrigger : MonoBehaviour {

	public string instructionString;
	public string noControllerInstructionString = "";
	private InstructionTextS instructionRef;
	private bool isShowing = false;

	[Header("Instruction Objects (Optional)")]
	public InstructionFloatS[] instructionObjs;
	private bool useInstructionObjs = false;

	[Header("Turn Off Conditions")]
	public int turnedOffIfClearedCombat = -1;

	public enum TutorialType {Text, Attack, Reset, Dodge, ParadigmShift, Menu, ChangeItem};
	public TutorialType tutorialType = TutorialType.Text;
	private int playerLightAttacks;
	private int playerHeavyAttacks;
	private int playerFamiliarAttacks;
	private int playerDodges;
	private int playerSprints;
	private int playerResets;
	private int playerShifts;
	private int playerSwaps;
	public int newTextSize = -1;

	void Start(){

		if (instructionObjs != null){
			if (instructionObjs.Length > 0){
				useInstructionObjs = true;
			}
		}

		if(!useInstructionObjs){
			instructionRef = GameObject.Find("InstructionText").GetComponent<InstructionTextS>();
			instructionString = instructionString.Replace("NEWLINE", "\n");
			if (noControllerInstructionString != ""){
				noControllerInstructionString = noControllerInstructionString.Replace("NEWLINE", "\n");
			}
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
		if (tutorialType == TutorialType.Menu){
			if (InGameMenuManagerS.hasUsedMenu){
				gameObject.SetActive(false);
			}
		}

		if (tutorialType == TutorialType.Attack){
			if ((playerLightAttacks >= 3 || (playerLightAttacks >= 1 && playerHeavyAttacks >= 1)) 
			    && playerFamiliarAttacks >= 1){
				gameObject.SetActive(false);
			}
		}

		if (tutorialType == TutorialType.Reset){
			if (playerResets >= 1){
				gameObject.SetActive(false);
			}
		}

		if (tutorialType == TutorialType.Dodge){
			if (playerDodges >= 2 && playerSprints >= 1){
				gameObject.SetActive(false);
			}
		}

		if (tutorialType == TutorialType.ParadigmShift){
			if (playerShifts >= 2){
				gameObject.SetActive(false);
			}
		}

		if (tutorialType == TutorialType.ChangeItem){
			if (playerSwaps >= 2){
				gameObject.SetActive(false);
			}
		}

	}

	void OnDisable(){
		if (!useInstructionObjs){
		if (isShowing){
			instructionRef.SetShowing(false);
		}
		}else{
			for (int i = 0; i < instructionObjs.Length; i++){
				instructionObjs[i].HideInstruction();
			}
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" && !PlayerStatDisplayS.RECORD_MODE){
			if (useInstructionObjs){
				for (int i = 0; i < instructionObjs.Length; i++){
					instructionObjs[i].ShowInstruction(other.transform, other.GetComponent<PlayerController>().myControl.ControllerAttached());
				}
			}else{
				if (noControllerInstructionString != "" && !other.GetComponent<PlayerController>().myControl.ControllerAttached()){
					instructionRef.SetShowing(true, noControllerInstructionString);
				}else{
					instructionRef.SetShowing(true, instructionString);
				}
			

				if (newTextSize > -1){
					instructionRef.SetTextSize(newTextSize);
				}
			}
			other.gameObject.GetComponent<PlayerController>().SetTutorial(this);
			isShowing = true;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player" && !PlayerStatDisplayS.RECORD_MODE){
			if (useInstructionObjs){
				for (int i = 0; i < instructionObjs.Length; i++){
					instructionObjs[i].HideInstruction();
				}
			}else{
				instructionRef.SetShowing(false);
			}
			isShowing = false;
		}
	}

				public void AddLightAttack(){
					playerLightAttacks++;
				}
				public void AddHeavyAttack(){
					playerHeavyAttacks++;
				}
	public void AddFamiliarAttack(){
		playerFamiliarAttacks++;
	}
	public void AddDodge(){
		playerDodges++;
	}
	public void AddSprint(){
		playerSprints++;
	}
	public void AddReset(){
		playerResets++;
	}
	public void AddShift(){
		playerShifts++;
	}

	public void AddSwap(){
		playerSwaps++;
	}
}
