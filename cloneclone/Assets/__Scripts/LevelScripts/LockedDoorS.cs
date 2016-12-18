using UnityEngine;
using System.Collections;

public class LockedDoorS : MonoBehaviour {

	public string lockString;
	public string unlockString;
	public string examineLabel;
	public int keyID;

	private bool isTalking = false;
	private bool talkButtonDown = false;
	private bool unlocking = false;

	private PlayerController pRef;
	private PlayerInteractCheckS interactRef;
	private bool playerInRange;

	// Use this for initialization
	void Start () {

		if (PlayerInventoryS.I.clearedWalls.Contains(keyID)){
			TurnOff();
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (playerInRange && pRef != null){
			if (!pRef.inCombat){
			if (pRef.myControl.TalkButton()){

				if (!talkButtonDown){
					if (!isTalking){
						TriggerExamine();
					}
					else{
						if (DialogueManagerS.D.doneScrolling){
							DialogueManagerS.D.EndText();
							if (unlocking){
									TurnOff();
							}else{
									EndExamine();
								}
							}else{
								DialogueManagerS.D.CompleteText();
							}
						}
				}
				talkButtonDown = true;
			}else{
				talkButtonDown = false;
			}
			}else{
				if (pRef.examining){
					pRef.SetExamining(false);
				}
			}
		}
	
	}

	public void CheckUnlock(int id){
		if (id == keyID){
			TriggerUnlock();
			PlayerInventoryS.I.AddClearedWall(keyID);
		}
	}

	private void TriggerUnlock(){
		pRef.SetTalking(true);
		pRef.SetExamining(true);
		isTalking = true;
		unlocking = true;
		DialogueManagerS.D.SetDisplayText(unlockString);
	}

	private void TriggerExamine(){
		
		pRef.SetTalking(true);
		pRef.SetExamining(true);
		isTalking = true;
		DialogueManagerS.D.SetDisplayText(lockString);

	}

	private void TurnOff(){
		if (playerInRange){
			pRef.SetExamining(false);
			pRef.SetTalking(false);
		}
		gameObject.SetActive(false);
	}

	private void EndExamine(){
		pRef.SetTalking(false);
		pRef.SetExamining(false);
		isTalking = false;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			if (!pRef){
				pRef = other.gameObject.GetComponent<PlayerController>();
				interactRef = pRef.GetComponentInChildren<PlayerInteractCheckS>();
			}
			pRef.SetExamining(true, examineLabel);
			interactRef.AddDoor(this);
			playerInRange = true;
		}
	}
	
	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			pRef.SetExamining(false);
			playerInRange = false;
			interactRef.RemoveDoor(this);
		}
	}
}
