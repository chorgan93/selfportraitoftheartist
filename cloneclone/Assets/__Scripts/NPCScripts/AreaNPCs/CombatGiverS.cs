using UnityEngine;
using System.Collections;

public class CombatGiverS : MonoBehaviour {

	[Header("Set Properties")]
	public CombatGiverUIItemS[] possChoices;
	public CombatGiverUIS combatChooseUI;
	public GameObject turnOnCombatObj;
	public int giveHealItem = -1;
	private bool giveHeal = false;

	[Header("Talk Properties")]
	public NPCDialogueSet introSet;
	public NPCDialogueSet chosenSet;
	public NPCDialogueSet talkSet;
	public NPCDialogueSet saleSet;
	public NPCDialogueSet goodbyeSet;
	public NPCDialogueSet giveHealSet;
	public NPCDialogueSet completedAllIntroSet;
	public NPCDialogueSet completedAllGoodbyeSet;
	private int currentDialogue = 0;
	private bool combatChosen = false;
	private bool completedAll = false;

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

	[Header("Item Properties")]
	public Sprite giveHealSprite;
	public Color giveHealColor;
	public Color giveHealOutlineColor;
	public int giveHealInt = 1;

	public static int chosenSpecialCombat = -1;

	[HideInInspector]
	public int merchantState = 0; // 0 = intro, 1 = menu, 2 = talk, 3 = buy, 4 = leave, 5= chosen

	private bool talking = false;

	// Use this for initialization
	void Start () {

		playerDetect = GetComponentInChildren<PlayerDetectS>();

		CheckCompletedAll();
		CheckGiveHeal();

	}
	
	// Update is called once per frame
	void Update () {

		if (playerDetect.PlayerInRange() || talking){
			if (!controlRef){
				playerRef = playerDetect.player;
				controlRef = playerRef.myControl;
			}
			else{
				if (controlRef.GetCustomInput(3)){
					if (!selectButtonDown){
						selectButtonDown = true;
                        if (!talking && !InGameMenuManagerS.menuInUse){
						talking = true;
						playerRef.SetTalking(true);
						CameraFollowS.F.SetNewPOI(gameObject);
							currentDialogue = 0;
							if (myAnimator){
							myAnimator.SetTrigger(talkKey);
							}
							if (!combatChosen){
								if (!giveHeal){
						merchantState = 0;
									if (!completedAll){
						DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[currentDialogue], false, true, true);
									}else{
										DialogueManagerS.D.SetDisplayText(completedAllIntroSet.dialogueStrings[currentDialogue], false, true, true);
									}
								}else{
									merchantState = 4;
									DialogueManagerS.D.SetDisplayText(giveHealSet.dialogueStrings[currentDialogue], false, true, true);
								}
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
										if ((!completedAll && currentDialogue >= introSet.dialogueStrings.Length) ||
											(completedAll && currentDialogue >= completedAllIntroSet.dialogueStrings.Length)){
											combatChooseUI.TurnOn(this);
										currentDialogue = 0;
										merchantState = 1;
									}else{
											if (!completedAll){
										DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[currentDialogue], false, true, true);
											}else{
												DialogueManagerS.D.SetDisplayText(completedAllIntroSet.dialogueStrings[currentDialogue], false, true, true);
											}
									}
								}else if (merchantState == 2){
									if (currentDialogue >= talkSet.dialogueStrings.Length){
											combatChooseUI.ShowMenus();
										currentDialogue = 0;
										merchantState = 1;
											if (!completedAll){
											DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[introSet.dialogueStrings.Length-1], false, true, true);
											}else{
												DialogueManagerS.D.SetDisplayText(completedAllIntroSet.dialogueStrings[introSet.dialogueStrings.Length-1], false, true, true);
											}
									}else{
										DialogueManagerS.D.SetDisplayText(talkSet.dialogueStrings[currentDialogue], false, true, true);
									}
								}else if (merchantState == 3){
									if (currentDialogue >= saleSet.dialogueStrings.Length){
											talking = false;
											DialogueManagerS.D.EndText();
											playerRef.SetTalking(false);
											CameraFollowS.F.ResetPOI();
											if (myAnimator){
											myAnimator.SetTrigger(idleKey);
											}
											combatChosen = true;
									}else{
										DialogueManagerS.D.SetDisplayText(saleSet.dialogueStrings[currentDialogue], false, true, true);
									}
									}else if (merchantState == 5){
										if (currentDialogue >= chosenSet.dialogueStrings.Length){
											talking = false;
											DialogueManagerS.D.EndText();
											playerRef.SetTalking(false);
											CameraFollowS.F.ResetPOI();
											combatChooseUI.TurnOff();
											if (myAnimator){
											myAnimator.SetTrigger(idleKey);
											}
										}else{
											DialogueManagerS.D.SetDisplayText(chosenSet.dialogueStrings[currentDialogue], false, true, true);
										}
									}else{
										if (!giveHeal){
											if ((!completedAll && currentDialogue >= goodbyeSet.dialogueStrings.Length) ||
												(completedAll && currentDialogue >= completedAllGoodbyeSet.dialogueStrings.Length)){
										talking = false;
										DialogueManagerS.D.EndText();
										playerRef.SetTalking(false);
										CameraFollowS.F.ResetPOI();
											combatChooseUI.TurnOff();
											if (myAnimator){
											myAnimator.SetTrigger(idleKey);
											}
									}else{
												if (!completedAll){
										DialogueManagerS.D.SetDisplayText(goodbyeSet.dialogueStrings[currentDialogue], false, true, true);
												}else{
													DialogueManagerS.D.SetDisplayText(completedAllGoodbyeSet.dialogueStrings[currentDialogue], false, true, true);
												}
									}
										}else{
											if (currentDialogue >= giveHealSet.dialogueStrings.Length){
												PlayerInventoryS.I.AddToInventory(1);
												PlayerInventoryS.I.AddCharge(giveHealItem);
												talking = false;
												DialogueManagerS.D.EndText();
												playerRef.SetTalking(false);
												CameraFollowS.F.ResetPOI();
												combatChooseUI.TurnOff();
												giveHeal = false;
												if (myAnimator){
													myAnimator.SetTrigger(idleKey);
												}
											}else{
												if (currentDialogue == giveHealInt){
													DialogueManagerS.D.SetItemFind(giveHealSprite, giveHealColor, giveHealOutlineColor);
													DialogueManagerS.D.SetDisplayText(giveHealSet.dialogueStrings[currentDialogue], false, true, true);
												}else{
													if (currentDialogue == giveHealInt+1){
														DialogueManagerS.D.EndItemFind();
													}
													DialogueManagerS.D.SetDisplayText(giveHealSet.dialogueStrings[currentDialogue], false, true, true);
												}
											}
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

					if (controlRef.GetCustomInput(13)){
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
			turnOnCombatObj.gameObject.SetActive(true);
			BuyMessage();
		}
		
	}

	public void ResetMessage(){
		if (!completedAll){
		DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[introSet.dialogueStrings.Length-1], false, true, true);
		}else{
			DialogueManagerS.D.SetDisplayText(completedAllIntroSet.dialogueStrings[introSet.dialogueStrings.Length-1], false, true, true);
		}
		
	}

	void BuyMessage(){
		merchantState = 3;
		currentDialogue = 0;
		DialogueManagerS.D.SetDisplayText(saleSet.dialogueStrings[currentDialogue], false, true, true);

	}

	public void StartTalk(){
		merchantState = 2;
		currentDialogue = 0;
		DialogueManagerS.D.SetDisplayText(talkSet.dialogueStrings[currentDialogue], false, true, true);
	}

	public void StartExit(){
		merchantState = 4;
		currentDialogue = 0;
		if (!completedAll){
		DialogueManagerS.D.SetDisplayText(goodbyeSet.dialogueStrings[currentDialogue], false, true, true);
		}else{
			DialogueManagerS.D.SetDisplayText(completedAllGoodbyeSet.dialogueStrings[currentDialogue], false, true, true);
		}
	}

	void CheckCompletedAll(){
		if (giveHealItem >= 0){
			int combatsCompleted = 0;
			if (PlayerInventoryS.I.dManager.specialConditionCombatCleared != null){
			for (int i = 0; i < possChoices.Length; i++){
				if (PlayerInventoryS.I.dManager.specialConditionCombatCleared.Contains(possChoices[i].combatID)){
					combatsCompleted++;
				}
			}
			}
			if (combatsCompleted >= possChoices.Length){
				completedAll = true;
			}
		}
	}

	void CheckGiveHeal(){
		if (giveHealItem >= 0){
			if (!PlayerInventoryS.I.CheckCharge(giveHealItem)){
				int combatsCompleted = 0;
				if (PlayerInventoryS.I.dManager.specialConditionCombatCleared != null){
				for (int i = 0; i < possChoices.Length; i++){
					if (PlayerInventoryS.I.dManager.specialConditionCombatCleared.Contains(possChoices[i].combatID)){
						combatsCompleted++;
					}
				}
				}
				if (combatsCompleted >= possChoices.Length){
					giveHeal = true;
				}
			}
		}
	}
}
