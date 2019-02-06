using UnityEngine;
using System.Collections;

public class MerchantS : MonoBehaviour {

	[Header("Set Properties")]
	public MerchantItemS[] itemsForSale;
	public MerchantUIS merchantUIRef;

	[Header("Talk Properties")]
	public NPCDialogueSet introSet;
	public NPCDialogueSet talkSet;
	public NPCDialogueSet saleSet;
	public NPCDialogueSet goodbyeSet;
	private int currentDialogue = 0;

	private bool selectButtonDown = false;
	private bool cancelButtonDown = false;
	private bool stickReset = false;
	private PlayerController playerRef;
	private ControlManagerS controlRef;

	private PlayerDetectS playerDetect;
	private PlayerCurrencyDisplayS cDisplay;

    public int stopTalkingAtProgress = 54;
    private bool stopTalking = false;

    private SpriteDistortionS childDistortion;
    private float distortRandomMin = 0.1f;
    private float distortRandomMax = 0.6f;
    private float distortCountdown;
    private float distortOffTime = 0.08f;

    private string[] stopTalkingStrings = new string[4] {". . .", ". . . . . .", "...", " . . . . . . . ."};

	[HideInInspector]
	public int merchantState = 0; // 0 = intro, 1 = menu, 2 = talk, 3 = buy, 4 = leave

	private bool talking = false;

	// Use this for initialization
	void Start () {

		playerDetect = GetComponentInChildren<PlayerDetectS>();
		cDisplay = GameObject.Find("SinBorder").GetComponent<PlayerCurrencyDisplayS>();

        if (StoryProgressionS.storyProgress.Contains(stopTalkingAtProgress)){
            stopTalking = true;
            Color fadeCol = GetComponent<SpriteRenderer>().color;
            fadeCol.a *= 0.4f;
            GetComponent<SpriteRenderer>().color = fadeCol;

            childDistortion = GetComponentInChildren<SpriteDistortionS>();
            distortCountdown = Random.Range(distortRandomMin, distortRandomMax);
        }

	}
	
	// Update is called once per frame
	void Update () {

        if (stopTalking){
            DistortHandler();
        }

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
						merchantState = 0;
                            if (stopTalking)
                            {
                                DialogueManagerS.D.SetDisplayText(stopTalkingStrings[Mathf.FloorToInt(Random.Range(0,stopTalkingStrings.Length))], 
                                                                  false, false, true);
                            }
                            else
                            {
                                DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[currentDialogue], false, false, true);
                            }
					}
					else if (talking){
						if (!DialogueManagerS.D.doneScrolling){
							DialogueManagerS.D.CompleteText();
						}else{
							if (merchantState != 1){
								currentDialogue++;
                                    if (merchantState == 0)
                                    {
                                        if (currentDialogue >= introSet.dialogueStrings.Length || stopTalking)
                                        {
                                            merchantUIRef.TurnOn(this);
                                            currentDialogue = 0;
                                            merchantState = 1;
                                        }
                                        else
                                        {
                                            if (stopTalking)
                                            {
                                                DialogueManagerS.D.SetDisplayText(stopTalkingStrings[Mathf.FloorToInt(Random.Range(0, stopTalkingStrings.Length))],
                                                                                  false, false, true);
                                            }
                                            else
                                            {
                                                DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[currentDialogue], false, false, true);
                                            }
                                        }
                                    }
                                    else if (merchantState == 2)
                                    {
                                        if (currentDialogue >= talkSet.dialogueStrings.Length || stopTalking){
										merchantUIRef.ShowMenus();
										currentDialogue = 0;
										merchantState = 1;
                                            if (stopTalking)
                                            {
                                                DialogueManagerS.D.SetDisplayText(stopTalkingStrings[Mathf.FloorToInt(Random.Range(0, stopTalkingStrings.Length))],
                                                                                  false, false, true);
                                            }
                                            else
                                            {
                                                DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[introSet.dialogueStrings.Length - 1], false, false, true);
                                            }
									}else{
                                            if (stopTalking)
                                            {
                                                DialogueManagerS.D.SetDisplayText(stopTalkingStrings[Mathf.FloorToInt(Random.Range(0, stopTalkingStrings.Length))],
                                                                                  false, true, true);
                                            }
                                            else
                                            {
                                                DialogueManagerS.D.SetDisplayText(talkSet.dialogueStrings[currentDialogue], false, true, true);
                                            }
									}
								}else if (merchantState == 3){
                                        if (currentDialogue >= saleSet.dialogueStrings.Length || stopTalking){
                                            if (stopTalking)
                                            {
                                                DialogueManagerS.D.SetDisplayText(stopTalkingStrings[Mathf.FloorToInt(Random.Range(0, stopTalkingStrings.Length))],
                                                                                  false, false, true);
                                            }
                                            else
                                            {
                                                DialogueManagerS.D.SetDisplayText(merchantUIRef.GetCurrentDescription(), false, false, true);
                                            }
										DialogueManagerS.D.CompleteText();
										currentDialogue = 0;
										merchantState = 1;
									}else{
                                            if (stopTalking)
                                            {
                                                DialogueManagerS.D.SetDisplayText(stopTalkingStrings[Mathf.FloorToInt(Random.Range(0, stopTalkingStrings.Length))],
                                                                                  false, false, true);
                                            }
                                            else
                                            {
                                                DialogueManagerS.D.SetDisplayText(saleSet.dialogueStrings[currentDialogue], false, false, true);
                                            }
									}
								}else{
                                        if (currentDialogue >= goodbyeSet.dialogueStrings.Length || stopTalking){
										talking = false;
										DialogueManagerS.D.EndText();
										playerRef.SetTalking(false);
										CameraFollowS.F.ResetPOI();
										merchantUIRef.TurnOff();
									}else{
                                            if (stopTalking)
                                            {
                                                DialogueManagerS.D.SetDisplayText(stopTalkingStrings[Mathf.FloorToInt(Random.Range(0, stopTalkingStrings.Length))],
                                                                                  false, false, true);
                                            }
                                            else
                                            {
                                                DialogueManagerS.D.SetDisplayText(goodbyeSet.dialogueStrings[currentDialogue], false, false, true);
                                            }
									}
								}
							}else{
								merchantUIRef.SelectOption();
							}
						}
						}
					
				}
				}else{
					selectButtonDown = false;
				}

				if (merchantState == 1){
#if UNITY_SWITCH
                    if (Mathf.Abs(controlRef.HorizontalMenu()) > 0.5f || Mathf.Abs(controlRef.VerticalMenu()) > 0.5f)
#else
                    if (Mathf.Abs(controlRef.HorizontalMenu()) > 0.1f || Mathf.Abs(controlRef.VerticalMenu()) > 0.1f)
#endif
                    {
						if (stickReset){
#if UNITY_SWITCH
                            if (controlRef.HorizontalMenu() > 0.5f || controlRef.VerticalMenu() > 0.5f)
#else
                            if (controlRef.HorizontalMenu() > 0.1f || controlRef.VerticalMenu() > 0.1f)
#endif
                            {
								merchantUIRef.MoveSelector(-1);
							}else{
								merchantUIRef.MoveSelector(1);
							}
						}
						stickReset = false;
					}else{
						stickReset = true;
					}

					if (controlRef.GetCustomInput(1)){
						if (!cancelButtonDown){
							merchantUIRef.ExitOption();
						}
						cancelButtonDown = true;
					}else{
						cancelButtonDown = false;
					}
				}
			}
		}
	
	}

    void DistortHandler(){
        distortCountdown -= Time.deltaTime;
        if (distortCountdown <= 0)
        {
            if (childDistortion.gameObject.activeSelf)
            {
                childDistortion.gameObject.SetActive(false);
                distortCountdown = distortOffTime;
            }
            else {
                childDistortion.gameObject.SetActive(true);
                distortCountdown = Random.Range(distortRandomMin, distortRandomMax);
            }
        }
    }

	public void AttemptBuy(int itemToBuy){
		if (itemsForSale[itemToBuy].canBeBought()){
			cDisplay.AddCurrency(-itemsForSale[itemToBuy].itemCost);
			itemsForSale[itemToBuy].Buy();
			merchantUIRef.DisplayItems();
            merchantUIRef.UpdateCurrency();
			BuyMessage();
		}
	}

	public void ResetMessage(){
        if (stopTalking)
        {
            DialogueManagerS.D.SetDisplayText(stopTalkingStrings[Mathf.FloorToInt(Random.Range(0, stopTalkingStrings.Length))],
                                              false, false, true);
        }
        else
        {
            DialogueManagerS.D.SetDisplayText(introSet.dialogueStrings[introSet.dialogueStrings.Length - 1], false, false, true);
        }
		
	}

	void BuyMessage(){
		merchantState = 3;
		currentDialogue = 0;
        if (stopTalking)
        {
            DialogueManagerS.D.SetDisplayText(stopTalkingStrings[Mathf.FloorToInt(Random.Range(0, stopTalkingStrings.Length))],
                                              false, false, true);
        }
        else
        {
            DialogueManagerS.D.SetDisplayText(saleSet.dialogueStrings[currentDialogue], false, false, true);
        }

	}

	public void StartTalk(){
		merchantState = 2;
		currentDialogue = 0;
        if (stopTalking)
        {
            DialogueManagerS.D.SetDisplayText(stopTalkingStrings[Mathf.FloorToInt(Random.Range(0, stopTalkingStrings.Length))],
                                              false, true, true);
        }
        else
        {
            DialogueManagerS.D.SetDisplayText(talkSet.dialogueStrings[currentDialogue], false, true, true);
        }
	}

	public void StartExit(){
		merchantState = 4;
		currentDialogue = 0;
        if (stopTalking)
        {
            DialogueManagerS.D.SetDisplayText(stopTalkingStrings[Mathf.FloorToInt(Random.Range(0, stopTalkingStrings.Length))],
                                              false, true, true);
        }
        else
        {
            DialogueManagerS.D.SetDisplayText(goodbyeSet.dialogueStrings[currentDialogue], false, true, true);
        }
	}
}
