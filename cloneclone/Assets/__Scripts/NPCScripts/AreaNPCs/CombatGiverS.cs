﻿using UnityEngine;
using System.Collections;

public class CombatGiverS : MonoBehaviour {

	[Header("Set Properties")]
	public CombatGiverUIItemS[] possChoices;
	public CombatGiverUIS combatChooseUI;

	[Header("Talk Properties")]
	public NPCDialogueSet introSet;
	public NPCDialogueSet chosenSet;
	public NPCDialogueSet talkSet;
	public NPCDialogueSet saleSet;
	public NPCDialogueSet goodbyeSet;
	private int currentDialogue = 0;
	private bool combatChosen = false;

	private bool selectButtonDown = false;
	private bool cancelButtonDown = false;
	private bool stickReset = false;
	private PlayerController playerRef;
	private ControlManagerS controlRef;

	private PlayerDetectS playerDetect;
	[Header("Animation Properties")]
	public Animator myAnimator;
	public string talkKey;
	public string idleKey;

	public static int chosenSpecialCombat = -1;

	[HideInInspector]
	public int merchantState = 0; // 0 = intro, 1 = menu, 2 = talk, 3 = buy, 4 = leave, 5= chosen

	private bool talking = false;

	// Use this for initialization
	void Start () {

		playerDetect = GetComponentInChildren<PlayerDetectS>();

	}
	
	// Update is called once per frame
	void Update () {

		if (playerDetect.PlayerInRange() || talking){
			if (!controlRef){
				playerRef = playerDetect.player;
				controlRef = playerRef.myControl;
			}
			else{
				if (controlRef.TalkButton()){
					if (!selectButtonDown){
						selectButtonDown = true;
						if (!talking){
						talking = true;
						playerRef.SetTalking(true);
						CameraFollowS.F.SetNewPOI(gameObject);
						currentDialogue = 0;
							if (!combatChosen){
						merchantState = 0;
						DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[currentDialogue], false, false, true);
							}else{
								merchantState = 5;
								DialogueManagerS.D.SetDisplayText(chosenSet.dialogueStrings[currentDialogue], false, true, true);
							}
					}
					else{
						if (!DialogueManagerS.D.doneScrolling){
							DialogueManagerS.D.CompleteText();
						}else{
							if (merchantState != 1){
								currentDialogue++;
								if (merchantState == 0){
									if (currentDialogue >= introSet.dialogueStrings.Length){
										//merchantUIRef.TurnOn(this);
										currentDialogue = 0;
										merchantState = 1;
									}else{
										DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[currentDialogue], false, false, true);
									}
								}else if (merchantState == 2){
									if (currentDialogue >= talkSet.dialogueStrings.Length){
											combatChooseUI.ShowMenus();
										currentDialogue = 0;
										merchantState = 1;
											DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[introSet.dialogueStrings.Length-1], false, false, true);
									}else{
										DialogueManagerS.D.SetDisplayText(talkSet.dialogueStrings[currentDialogue], false, true, true);
									}
								}else if (merchantState == 3){
									if (currentDialogue >= saleSet.dialogueStrings.Length){
											talking = false;
											DialogueManagerS.D.EndText();
											playerRef.SetTalking(false);
											CameraFollowS.F.ResetPOI();
											myAnimator.SetTrigger(idleKey);
									}else{
										DialogueManagerS.D.SetDisplayText(saleSet.dialogueStrings[currentDialogue], false, false, true);
									}
									}else if (merchantState == 5){
										if (currentDialogue >= chosenSet.dialogueStrings.Length){
											talking = false;
											DialogueManagerS.D.EndText();
											playerRef.SetTalking(false);
											CameraFollowS.F.ResetPOI();
											combatChooseUI.TurnOff();
											myAnimator.SetTrigger(idleKey);
										}else{
											DialogueManagerS.D.SetDisplayText(chosenSet.dialogueStrings[currentDialogue], false, true, true);
										}
									}else{
										if (currentDialogue >= goodbyeSet.dialogueStrings.Length){
										talking = false;
										DialogueManagerS.D.EndText();
										playerRef.SetTalking(false);
										CameraFollowS.F.ResetPOI();
											combatChooseUI.TurnOff();
											myAnimator.SetTrigger(idleKey);
									}else{
										DialogueManagerS.D.SetDisplayText(goodbyeSet.dialogueStrings[currentDialogue], false, true, true);
									}
								}
							}else{
									combatChooseUI.SelectOption();
							}
						}
						}
					
				}
				}else{
					selectButtonDown = false;
				}

				if (merchantState == 1){
					if (Mathf.Abs(controlRef.HorizontalMenu()) > 0.1f || Mathf.Abs(controlRef.VerticalMenu()) > 0.1f){
						if (stickReset){
							if (controlRef.HorizontalMenu() > 0.1f || controlRef.VerticalMenu() > 0.1f){
								combatChooseUI.MoveSelector(-1);
							}else{
								combatChooseUI.MoveSelector(1);
							}
						}
						stickReset = false;
					}else{
						stickReset = true;
					}

					if (controlRef.ExitButton()){
						if (!cancelButtonDown){
							combatChooseUI.ExitOption();
						}
						cancelButtonDown = true;
					}else{
						cancelButtonDown = false;
					}
				}
			}
		}
	
	}

	public void SelectOption(int chosenOption){

		if (possChoices[chosenOption].CanSelect()){
			chosenSpecialCombat = possChoices[chosenOption].combatID;
			combatChooseUI.TurnOff();
			BuyMessage();
		}
		
	}

	public void ResetMessage(){
		DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[introSet.dialogueStrings.Length-1], false, false, true);
		
	}

	void BuyMessage(){
		merchantState = 3;
		currentDialogue = 0;
		DialogueManagerS.D.SetDisplayText(saleSet.dialogueStrings[currentDialogue], false, false, true);

	}

	public void StartTalk(){
		myAnimator.SetTrigger(talkKey);
		merchantState = 2;
		currentDialogue = 0;
		DialogueManagerS.D.SetDisplayText(talkSet.dialogueStrings[currentDialogue], false, true, true);
	}

	public void StartExit(){
		merchantState = 4;
		currentDialogue = 0;
		DialogueManagerS.D.SetDisplayText(goodbyeSet.dialogueStrings[currentDialogue], false, true, true);
	}
}
