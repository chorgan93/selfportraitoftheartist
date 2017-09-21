using UnityEngine;
using System.Collections;

public class MemoS : MonoBehaviour {

	public string[] examineStrings;
	public int startMemoIndex;
	public int endMemoIndex;
	public string examineLabel;
	public string examineLabelNoController = "";
	public Vector3 examinePos = new Vector3(0, 1f, 0);
	public int memoID;
	private int currentStep = 0;

	private bool isTalking = false;
	private bool talkButtonDown = false;

	private PlayerController pRef;
	private bool playerInRange;

	// Use this for initialization
	void Start () {

		if (PlayerInventoryS.I.collectedKeyItems.Contains(memoID)){
			TurnOff();
		}else{
			AddLineBreaks();
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (playerInRange && pRef != null){
			if (pRef.myControl.TalkButton()){

				if (!talkButtonDown){
					if (!isTalking){
						TriggerExamine();
					}
					else{
						if (DialogueManagerS.D.doneScrolling){

							if (currentStep < examineStrings.Length-1){
								currentStep++;
								DialogueManagerS.D.SetDisplayText(examineStrings[currentStep], 
								                                  (currentStep>=startMemoIndex && currentStep < endMemoIndex));
							}
							else{
								DialogueManagerS.D.EndText();
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
		}
	
	}

	void AddLineBreaks(){
		for (int i = 0; i < examineStrings.Length; i++){
			if (examineStrings[i].Contains("NEWLINE")){
				examineStrings[i] = examineStrings[i].Replace("NEWLINE", "\n");
			}
		}
	}

	private void TriggerExamine(){
		
		pRef.SetTalking(true);
		pRef.SetExamining(true, examinePos);
		isTalking = true;
		currentStep = 0;
		DialogueManagerS.D.SetDisplayText(examineStrings[currentStep]);

	}

	private void TurnOff(){
		if (playerInRange){
			pRef.SetExamining(false, examinePos);
			pRef.SetTalking(false);
		}
		gameObject.SetActive(false);
	}

	private void EndExamine(){
		pRef.SetTalking(false);
		pRef.SetExamining(false, examinePos);
		PlayerInventoryS.I.AddKeyItem(memoID);
		isTalking = false;
		TurnOff();
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			if (!pRef){
				pRef = other.gameObject.GetComponent<PlayerController>();
			}
			if (examineLabelNoController != ""){
				if (!pRef.myControl.ControllerAttached()){
					pRef.SetExamining(true, examinePos, examineLabelNoController);
				}else{
					pRef.SetExamining(true, examinePos, examineLabel);
				}
			}else{
				pRef.SetExamining(true, examinePos, examineLabel);
			}
			playerInRange = true;
		}
	}
	
	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			pRef.SetExamining(false, examinePos);
			playerInRange = false;
		}
	}
}
