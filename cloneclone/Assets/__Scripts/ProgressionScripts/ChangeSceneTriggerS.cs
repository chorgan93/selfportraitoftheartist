using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeSceneTriggerS : MonoBehaviour {

	public string nextSceneString = "";
	public bool requireExamine = false;
	public string awaitResponseString = "";
	public string[] additionalResponseStrings;
	private int numResponses = 0;
	private int currentResponse = 0;
	private bool requiresResponse = false;
	public float delayTransition = -1f;
	private bool waitingForChange = false;
	public int whereToSpawn = 0;
	public int doorNum = -1;
	public string examineString = "";
	public string examineStringNoController = "";
	public Vector3 examinePos = new Vector3(0, 1f, 0);
	public Sprite openedSprite;
	private SpriteRenderer _myRender;
	private bool loading = false;
	private bool examining = false;
	private bool talkButtonDown = false;
	private PlayerController pRef;
	public bool doWakeUp = false;
	public bool dontResetItemsOnWakeUp =false;
	public int setProgressOnActivate = -1;

	private bool startedNewScene = false;

	private bool awaitingResponse = false;

	[Header("On/Off Properties")]
	public ActivateOnSceneTriggerS activateS;

	void Start(){

		if (awaitResponseString != "" && requireExamine){
			requiresResponse = true;
			numResponses = 1;
		}
		if (additionalResponseStrings != null){
			numResponses+=additionalResponseStrings.Length;
		}
		if (openedSprite != null){
			_myRender=GetComponentInChildren<SpriteRenderer>();
			if (PlayerInventoryS.I.openedDoors.Contains(doorNum)){
				_myRender.sprite = openedSprite;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {



		if (waitingForChange){
			delayTransition -= Time.deltaTime;
			if (delayTransition <= 0 && !startedNewScene){
				TransitionScene();
			}
		}
		else if (requireExamine && examining && !loading && !awaitingResponse){

			if (pRef.myDetect.allEnemiesInRange.Count <= 0){
				if (pRef.myControl.ControllerAttached() || examineStringNoController == ""){
					pRef.SetExamining(true, examinePos, examineString);
				}else{
					pRef.SetExamining(true, examinePos, examineStringNoController);
				}
			}

			if (pRef.myControl.GetCustomInput(3)){
				if (!talkButtonDown && pRef.examining && !pRef.talking){
					if (!requiresResponse){
					StartNextScene();
					}else{
						DialogueManagerS.D.SetDisplayText(awaitResponseString,false,false,false,true);
						awaitingResponse = true;
					}
					pRef.SetTalking(true);
				}
				talkButtonDown = true;
			}else{
				talkButtonDown = false;
			}
		}else if (awaitingResponse){
			if (DialogueManagerS.D.doneScrolling){
				if (pRef.myControl.GetCustomInput(3)){
					talkButtonDown = true;
				}else{
					talkButtonDown = false;
				}
				if (!DialogueManagerS.D.dialogueResponse.getChoiceActive){
					if (DialogueManagerS.D.dialogueResponse.GetChoiceMade() == 0){

						currentResponse++;
						if (currentResponse >= numResponses){
							StartNextScene();
							DialogueManagerS.D.EndText();
							if (loading){
								pRef.SetTalking(true);
							}
						}else{

							DialogueManagerS.D.SetDisplayText(additionalResponseStrings[currentResponse-1],false,false,false,true);
						}
					}else{
						pRef.SetTalking(false);
						awaitingResponse = false;
						currentResponse = 0;
						DialogueManagerS.D.EndText();
						if (loading){
							pRef.SetTalking(true);
						}
					}
			}
			}
			else {
				if (pRef.myControl.GetCustomInput(3)){
					if (!talkButtonDown){
						DialogueManagerS.D.CompleteText();
					}
					talkButtonDown = true;
				}else{
					talkButtonDown = false;
				}


				}
			}

	
	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Player" && nextSceneString!="" && !loading){

			if (!pRef){
				pRef = other.GetComponent<PlayerController>();
			}
			if (!pRef.myStats.PlayerIsDead()){
			if (!requireExamine){
				
				pRef.SetTalking(true);
				StartNextScene();
			}else{
				if (pRef.myDetect.allEnemiesInRange.Count <= 0){
					if (pRef.myControl.ControllerAttached() || examineStringNoController == ""){
						pRef.SetExamining(true, examinePos, examineString);
					}else{
						pRef.SetExamining(true, examinePos, examineStringNoController);
					}
				}
				examining = true;
			}
			}
		}

	}

	void OnTriggerExit(Collider other){
		if (requireExamine){
			if (other.gameObject.tag == "Player"){
				pRef.SetExamining(false, examinePos);
				examining = false;
			}
		}
	}

	void StartNextScene(){

		if (delayTransition <= 0f){
			TransitionScene();
		}else{
			pRef.SetTalking(true);
			pRef.SetExamining(false, Vector3.zero);
			waitingForChange = true;
		}
		if (activateS != null){
			activateS.TurnOnOff();
		}
	}

	void TransitionScene(){
			
		GameOverS.tempReviveScene = "";

		if (SceneManagerS.inInfiniteScene){
			PlayerInventoryS.I.dManager.ClearAll();
		}
		List<int> saveBuddyList = new List<int>();
		saveBuddyList.Add(pRef.ParadigmIBuddy().buddyNum);
		if (pRef.ParadigmIIBuddy() != null){
			saveBuddyList.Add(pRef.ParadigmIIBuddy().buddyNum);
		}
		PlayerInventoryS.I.SaveLoadout(pRef.equippedWeapons, pRef.subWeapons, saveBuddyList);

		if (setProgressOnActivate > -1){
			StoryProgressionS.SetStory(setProgressOnActivate);
		}

		SpawnPosManager.whereToSpawn = whereToSpawn;
		CameraEffectsS.E.SetNextScene(nextSceneString);
		CameraEffectsS.E.FadeIn();
		loading = true;

		VerseDisplayS.V.EndVerse();


		if (doWakeUp){
			PlayerController.doWakeUp = true;
			if (dontResetItemsOnWakeUp){
				PlayerController.dontHealWakeUp = true;
			}
		}

		if (requireExamine && doorNum > -1){
			PlayerInventoryS.I.AddOpenDoor(doorNum);
		}
		startedNewScene = true;
	}

	void OnDisable(){
		if (examining && pRef != null){
			examining = false;
			pRef.SetExamining(false, Vector3.zero);
		}
	}
}
