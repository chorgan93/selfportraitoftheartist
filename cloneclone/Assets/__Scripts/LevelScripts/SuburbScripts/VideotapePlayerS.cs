using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VideotapePlayerS : MonoBehaviour {

	[Header("Object References")]
	public GameObject tvOff;
	public ExamineTriggerS tvOn;
	public ExamineTriggerS tvAmbient;
	public GameObject vcrOff;

	[Header("Examine Strings")]
	public string examineTapeA;
	public string examineTapeB;
	public string examineTapeC;
	public string examineTapeIn;
	public string examineNoTapes;
	public string ambientIntercut;

	private bool seenOneTape = false; // need this for turning on ambient tv vs. off tv
	private bool seenAllTapes = false;
	private bool tapeIsLoaded = false;

	[Header("Progression Stats")]
	public int keyTapeA; // added to "cleared walls" to see which tapes have been watched
	public int itemNumTapeA; // checks against inventory for unlock
	public int keyTapeB;
	public int itemNumTapeB;
	public int keyTapeC;
	public int itemNumTapeC;

	[Header("Navigation Stats")]
	public string sceneStringTapeA;
	public string sceneStringTapeB;
	public string sceneStringTapeC;
	public BarrierS[] barriersToTurnOff;
	public GameObject barrier2Dholder;
	public int progressNumToAdd = -1;
	private bool lookingAtTV = false;

	// management variables
	private PlayerDetectS playerDetect;
	private PlayerController playerRef;
	private bool talkButtonDown;
	private bool talking;

	public static bool backFromTape = false;
	private static bool hasSeenLoadMessage = false;


	public Sprite keyItemSprite;
	public Color keyItemSpriteColor = Color.white;
	public Color keyItemOutlineColor = Color.red;

	void Start(){
		playerDetect = GetComponentInChildren<PlayerDetectS>();
		tvAmbient.examineString = tvAmbient.examineString.Replace("[TVNUM]", PlayerInventoryS.I.tvNum.ToString());
		InitializeTVs();
	}

	void Update(){
		if (playerDetect.PlayerInRange() && !playerRef){
			playerRef = playerDetect.player;
		}

			if (playerRef != null){
			if (backFromTape && !CameraEffectsS.E.isFading && !playerRef.talking){
				if(PlayerInventoryS.I.clearedWalls.Contains(keyTapeA) && PlayerInventoryS.I.clearedWalls.Contains(keyTapeB) &&
					PlayerInventoryS.I.clearedWalls.Contains(keyTapeC)){
					for (int i = 0; i < barriersToTurnOff.Length; i++){
						barriersToTurnOff[i].TurnOff();
					}
					barrier2Dholder.SetActive(false);
					StoryProgressionS.storyProgress.Add(progressNumToAdd);
				}
				else if (!hasSeenLoadMessage){
					TriggerExamine(4);
					hasSeenLoadMessage = true;
				}
				backFromTape = false;
			}
				if (!playerRef.inCombat && !CameraEffectsS.E.isFading && !InGameMenuManagerS.menuInUse){
					if (playerRef.myControl.GetCustomInput(3)){

						if (!talkButtonDown){
						if (!talking && !playerRef.talking && playerDetect.PlayerInRange()){
							TriggerExamine(SetUpTape());
							}
						else if (talking){
								if (DialogueManagerS.D.doneScrolling){
									DialogueManagerS.D.EndText();
									if (lookingAtTV){
										CameraFollowS.F.ResetPOI();
									}
										
									EndExamine();

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
				if (playerRef.examining){
					playerRef.SetExamining(false, playerDetect.examinePos);
					}
				}
			}

	}

	void TriggerExamine(int typeOfExamine){
		playerRef.SetTalking(true);
		playerRef.SetExamining(true, playerDetect.examinePos);
		talking = true;
		switch (typeOfExamine){
			case 0:
				// give tape is in message
			DialogueManagerS.D.SetDisplayText(examineTapeIn);
			LookAtMe();
				break;
			case 1:
			// insert tape a 
			if (keyItemSprite){
				DialogueManagerS.D.SetItemFind(keyItemSprite, keyItemSpriteColor, keyItemOutlineColor);
			}
			DialogueManagerS.D.SetDisplayText(examineTapeA);
			LookAtTV();
				break;
			case 2:
			// insert tape b
			if (keyItemSprite){
				DialogueManagerS.D.SetItemFind(keyItemSprite, keyItemSpriteColor, keyItemOutlineColor);
			}
			DialogueManagerS.D.SetDisplayText(examineTapeB);
			LookAtTV();
				break;
			case 3:
			// insert tape c
			if (keyItemSprite){
				DialogueManagerS.D.SetItemFind(keyItemSprite, keyItemSpriteColor, keyItemOutlineColor);
			}
			DialogueManagerS.D.SetDisplayText(examineTapeC);
			LookAtTV();
				break;
		case 4:
			// intercut message (to get point across)
			DialogueManagerS.D.SetDisplayText(ambientIntercut);
			LookAtTV();
			break;
			default:
			// no tape message
			DialogueManagerS.D.SetDisplayText(examineNoTapes);
				break;
		}
	}

	void LookAtTV(){
		CameraFollowS.F.SetNewPOI(tvOff);
		lookingAtTV = true;
	}
	void LookAtMe(){
		CameraFollowS.F.SetNewPOI(gameObject);
		lookingAtTV = true;
	}

	void EndExamine(){
		lookingAtTV = false;
		playerRef.SetTalking(false);
		playerRef.SetExamining(true, playerDetect.examinePos);
		talking = false;
	}

	void TurnOffAllTVs(){
		tvOff.SetActive(false);
		tvOn.gameObject.SetActive(false);
		tvAmbient.gameObject.SetActive(false);
	}
	void InitializeTVs(){
		TurnOffAllTVs();
		if (PlayerInventoryS.I.clearedWalls.Contains(keyTapeA) && PlayerInventoryS.I.clearedWalls.Contains(keyTapeB) &&
			PlayerInventoryS.I.clearedWalls.Contains(keyTapeC) && !backFromTape){
			tvAmbient.gameObject.SetActive(true);
			vcrOff.SetActive(true);
			for (int i = 0; i < barriersToTurnOff.Length; i++){
				barriersToTurnOff[i].gameObject.SetActive(false);
			}
			barrier2Dholder.SetActive(false);
			StoryProgressionS.storyProgress.Add(progressNumToAdd);
			gameObject.SetActive(false);
		}
		else if (PlayerInventoryS.I.clearedWalls.Contains(keyTapeA) || PlayerInventoryS.I.clearedWalls.Contains(keyTapeB) ||
			PlayerInventoryS.I.clearedWalls.Contains(keyTapeC)){
			tvAmbient.gameObject.SetActive(true);
		}else{
			tvOff.gameObject.SetActive(true);
		}
	}
	int SetUpTape(){

		if (!tapeIsLoaded){
		TurnOffAllTVs();

		bool foundTape = false;
		int tapeMessage = 0;
		if (!PlayerInventoryS.I.clearedWalls.Contains(keyTapeA)){
			if (PlayerInventoryS.I.CheckForItem(itemNumTapeA)){
				tvOn.gameObject.SetActive(true);
				tvOn.teleportScene = sceneStringTapeA;
				foundTape = true;
				tapeMessage = 1;
				PlayerInventoryS.I.AddClearedWall(keyTapeA);
			}
		}

		if (!PlayerInventoryS.I.clearedWalls.Contains(keyTapeB) && !foundTape){
			if (PlayerInventoryS.I.CheckForItem(itemNumTapeB)){
				tvOn.gameObject.SetActive(true);
				tvOn.teleportScene = sceneStringTapeB;
				foundTape = true;
				tapeMessage = 2;
				PlayerInventoryS.I.AddClearedWall(keyTapeB);
			}
		}

		if (!PlayerInventoryS.I.clearedWalls.Contains(keyTapeC) && !foundTape){
			if (PlayerInventoryS.I.CheckForItem(itemNumTapeC)){
				tvOn.gameObject.SetActive(true);
				tvOn.teleportScene = sceneStringTapeC;
				foundTape = true;
				tapeMessage = 3;
				PlayerInventoryS.I.AddClearedWall(keyTapeC);
			}
		}

		if (!foundTape){
			if (PlayerInventoryS.I.clearedWalls.Contains(keyTapeA) || PlayerInventoryS.I.clearedWalls.Contains(keyTapeB) ||
				PlayerInventoryS.I.clearedWalls.Contains(keyTapeC)){
				tvAmbient.gameObject.SetActive(true);
			}else{
				tvOff.gameObject.SetActive(true);
				}
				tapeMessage = -1;
			}else{
				tapeIsLoaded = true;
			}

		return tapeMessage;
		}else{
			return 0;
		}
	}
}
