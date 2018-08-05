using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExamineTriggerS : MonoBehaviour {

	public string examineLabel = "";
	public string examineLabelNoController = "";
	public Vector3 examinePos = new Vector3(0, 1f, 0);
	public string examineString;
	public string examineStringKeyUsed = "";
	public string unlockString;
	public int costToExamine = -1;
	public GameObject examineSound;
	public float delayOnTime = 0f;
	private float delayOnTimeStart;

	public int keyInt = -1;
	public BarrierS turnOffBarrier;
	private bool unlocking = false;

	public GameObject newPoi;
	public GameObject lockObject;

	private bool playerInRange = false;
	private PlayerController pRef;
	private bool talkButtonDown = false;
	private bool talking = false;

	public bool consumable = false;
	public int inventoryNum = -1;
	public int healNum = -1;
	public int chargeNum = -1;
	public int laNum = -1;
	public int vpNum = -1;
	public int numToAdd = 1;
	public bool keyItem = false;
	public Sprite keyItemSprite;
	public Color keyItemSpriteColor = Color.white;
	public Color keyItemOutlineColor = Color.red;
	public ActivateOnExamineS myTrigger;
	public BuddyS buddyToGive;
	public bool forceBuddySwitch = false;
	public PlayerWeaponS mantraToGive;
	public int virtueToGive = -1;
	public int techToGive = -1;
	public int laToGive = -1;

	public bool advanceProgress = false;
	public int setProgress = -1;
	public int removeProgress = -1;
	public bool saveOnPickup = false;
	public bool isTapeTV = false;

	public bool inInfiniteMode = false;
	private InfinitySpawnS parentInfinite;
	public bool hasTextFollowUp = false;

	[Header("Special Trigger Properties")]
	public GameObject highwaySwitchSpawn;
    public bool ignoreInCombat = false;

	[Header("FOR DEMO REWORK AFTER")]
	public bool teleportItem = false;
	public string teleportScene = "InfiniteScene";
	public bool fullRevive = false;
    public bool lookAnimation = false;

    [Header("Ritual Properties")]
    public bool killFamiliarRitual = false;
    public bool fullCorruptionRitual = false;


	public float lookTime = -1f;

	void Start () {

		delayOnTimeStart = delayOnTime;

		if (inventoryNum >= 0 || techToGive > -1 || virtueToGive > 0 || buddyToGive || (mantraToGive != null)){
			CheckInventory();
		}

		if (lookTime > 0){
			CameraFollowS.F.AddToQueue(gameObject, lookTime);
		}

		if (inInfiniteMode){
			parentInfinite = GetComponentInParent<InfinitySpawnS>();
		}

		if (examineString.Contains("NEWLINE")){
			examineString = examineString.Replace("NEWLINE","\n");
		}
		if (examineString.Contains("PLAYERNAME")){
			examineString = examineString.Replace("PLAYERNAME", TextInputUIS.playerName);
		}

		if (examineStringKeyUsed != ""){
			examineStringKeyUsed = examineStringKeyUsed.Replace("NEWLINE","\n");
			examineStringKeyUsed = examineStringKeyUsed.Replace("PLAYERNAME",TextInputUIS.playerName);
		}



	}

	// Update is called once per frame
	void Update () {

		if (turnOffBarrier){
			if (!turnOffBarrier.gameObject.activeSelf){
				gameObject.SetActive(false);
				if (playerInRange){
					pRef.SetExamining(false, examinePos);
				}
			}
		}
	
		if (pRef){
			if (delayOnTime > 0){
				delayOnTime -= Time.deltaTime;
				talkButtonDown = true;
			}
            if ((!pRef._inCombat || (pRef.inCombat && ignoreInCombat)) &&  !CameraEffectsS.E.isFading  && !InGameMenuManagerS.menuInUse){

				if (!pRef.myControl.GetCustomInput(3) && delayOnTime <= 0){
				talkButtonDown = false;
			}

				if (pRef.myControl.GetCustomInput(3) && !talkButtonDown && delayOnTime <= 0f){
				talkButtonDown = true;

					if (!talking && examineString != "" && !pRef.talking){
						if (playerInRange){
						if (costToExamine < PlayerCollectionS.currencyCollected && !CameraEffectsS.E.isFading && !pRef.talking){

						pRef.SetTalking(true);
						if (newPoi){
							CameraFollowS.F.SetNewPOI(newPoi);
						}

						if (costToExamine > 0){
							pRef.myStats.uiReference.cDisplay.AddCurrency(-costToExamine);
						}
	
						if (examineSound){
							Instantiate(examineSound);
						}
						talking = true;
	
						if (keyInt >= 0){
							CheckUnlock();
						}
	
						if (!unlocking){
									if (keyItemSprite){
										DialogueManagerS.D.SetItemFind(keyItemSprite, keyItemSpriteColor, keyItemOutlineColor);
									} 
										
										if (examineStringKeyUsed != "" && PlayerInventoryS.I.clearedWalls.Contains(inventoryNum)){
							DialogueManagerS.D.SetDisplayText(examineStringKeyUsed);
									}else{
										DialogueManagerS.D.SetDisplayText(examineString);
									}

									//Debug.Log("Set text! " + DialogueManagerS.D.doneScrolling);
						}else{
							DialogueManagerS.D.SetDisplayText(unlockString);
						}
					}
						}

				}else{
						if (costToExamine < PlayerCollectionS.currencyCollected && !CameraEffectsS.E.isFading 
                            && ((talking && examineString != "") || (examineString == "" && playerInRange && !pRef.talking))){
						if (examineString == "" && examineSound != null){
							Instantiate(examineSound);
						}
                            if (lookAnimation){
                                pRef.DoLookAway();
                            }

							if (costToExamine > 0){
								pRef.myStats.uiReference.cDisplay.AddCurrency(-costToExamine);
							}

							//Debug.Log("Second press! " + DialogueManagerS.D.doneScrolling);
	
                            if (DialogueManagerS.D.doneScrolling && !lookAnimation){
								if (!teleportItem){
									CameraFollowS.F.ResetPOI();
									//Debug.Log("End text!");
									if (!hasTextFollowUp){
										pRef.SetTalking(false);
									DialogueManagerS.D.EndText();
									}
							talking = false;
								}else{
									DialogueManagerS.D.SetDisplayText("");
									DialogueManagerS.D.FreezeDialogue();
								}
	
							if (inInfiniteMode){
								parentInfinite.AddClear();
							}

							if (teleportItem){
								if (!fullRevive){
									InfinityDemoS.savedLastDifficulty = 1;
								}
									List<int> saveBuddyList = new List<int>();
									saveBuddyList.Add(pRef.ParadigmIBuddy().buddyNum);
									if (pRef.ParadigmIIBuddy() != null){
										saveBuddyList.Add(pRef.ParadigmIIBuddy().buddyNum);
									}
                                    if (!pRef.isNatalie)
                                    {
                                        PlayerInventoryS.I.SaveLoadout(pRef.equippedWeapons, pRef.subWeapons, saveBuddyList);
                                    }
								CameraEffectsS.E.SetNextScene(teleportScene);
								CameraEffectsS.E.FadeIn();
									if (isTapeTV){
										VideotapePlayerS.backFromTape = true;
									}
                                    if (killFamiliarRitual){
                                        PlayerController.killedFamiliar = true;
                                        DarknessPercentUIS.resetToZero = true;
                                        DarknessPercentUIS.savedDarknessNum = PlayerStatsS._currentDarkness;
                                    }else if (fullCorruptionRitual){
                                        DarknessPercentUIS.setTo100 = true;
                                        DarknessPercentUIS.hasReached100 = true;
                                        DarknessPercentUIS.savedDarknessNum = PlayerStatsS._currentDarkness;
                                    }
							}
	
							if (myTrigger){
								myTrigger.TurnOn();
							}
								if (highwaySwitchSpawn){
									GameObject switchSpawn = Instantiate(highwaySwitchSpawn) as GameObject;
									switchSpawn.SetActive(true);
								}
	
							if (inventoryNum >= -1){
								AddPickup();
							}

							if (buddyToGive){
									InGameMenuManagerS.allowMenuUse = true;
								PlayerInventoryS.I.unlockedBuddies.Add(buddyToGive);
								if (pRef.equippedBuddies.Count < 2){
									pRef.equippedBuddies.Add(buddyToGive.gameObject);
									if (pRef.equippedWeapons.Count < 2){
										pRef.equippedWeapons.Add(pRef.EquippedWeapon());
										pRef.subWeapons.Add(pRef.EquippedWeaponAug());
									}
								}
							}
							if (mantraToGive != null){
								PlayerInventoryS.I.unlockedWeapons.Add(mantraToGive);
								if (pRef.equippedWeapons.Count < 2){
									pRef.equippedWeapons.Add(mantraToGive);
									pRef.subWeapons.Add(mantraToGive);
									if (pRef.equippedBuddies.Count < 2){
										pRef.equippedBuddies.Add(pRef.equippedBuddies[0]);
									}
								}
									if (saveOnPickup){
										StoryProgressionS.SaveProgress();
									}
							}

								if (virtueToGive > 0){
									PlayerInventoryS.I.AddEarnedVirtue(virtueToGive);
								}
								if (techToGive > -1){
									PlayerInventoryS.I.AddEarnedTech(techToGive);
								}
	
							if (unlocking){
								turnOffBarrier.TurnOff();
									pRef.SetExamining(false, examinePos);
								Destroy(gameObject);
							}
	
							if (consumable){
									pRef.SetExamining(false, examinePos);
								Destroy(gameObject);
							}

						}else{
								DialogueManagerS.D.CompleteText();
								//Debug.Log("Complete text!");
						}
					}
				}
			}
			}else{
				if (pRef.examining){
					pRef.SetExamining(false, examinePos);
				}
			}
		}
		

	}

	private void CheckInventory () {

		if (mantraToGive != null){
			if (PlayerInventoryS.I.unlockedWeapons.Contains(mantraToGive)){
				gameObject.SetActive(false);
			}
		}
		if (buddyToGive){
			if (PlayerInventoryS.I.unlockedBuddies.Contains(buddyToGive)){
				gameObject.SetActive(false);
			}
		}
		if (virtueToGive > 0){
			if (PlayerInventoryS.I._earnedVirtues.Contains(virtueToGive)){
				gameObject.SetActive(false);
			}
		}
		if (techToGive > -1){
			if (PlayerInventoryS.I.earnedTech.Contains(techToGive)){
				gameObject.SetActive(false);
			}
		}

		if (PlayerInventoryS.I.collectedItems.Count > 0){
			foreach (int i in PlayerInventoryS.I.collectedItems){
				if (i == inventoryNum && inventoryNum > 2 && inventoryNum != 420){
					gameObject.SetActive(false);
				}
			}
		}
		if (PlayerInventoryS.I.collectedKeyItems.Count > 0 && keyItem){
			foreach (int j in PlayerInventoryS.I.collectedKeyItems){
				if (j == inventoryNum){
					gameObject.SetActive(false);
				}
			}
		}

		// check if picked up rewind
		if (inventoryNum == 0 && PlayerInventoryS.I.CheckHeal(healNum)){
			gameObject.SetActive(false);
		}
		// check if picked up virtue increase
		if (inventoryNum == 2 && PlayerInventoryS.I.CheckVP(vpNum)){
			gameObject.SetActive(false);
		}

		// check if picked up la increase
		if (inventoryNum == 420 && PlayerInventoryS.I.CheckStim(laNum)){
			gameObject.SetActive(false);
		}

		// check if picked up health essence
		if (inventoryNum == 1 && PlayerInventoryS.I.CheckCharge(chargeNum)){
			gameObject.SetActive(false);
		}


	}

	private void AddPickup(){

		for (int i = 0; i < numToAdd; i++){
			if (inventoryNum >= 0){
				PlayerInventoryS.I.AddToInventory(inventoryNum, keyItem);
				KeyItemUIS.K.EvaluateItems();
			}

			// add rewind
			if (healNum >= 0){
				PlayerInventoryS.I.AddHeal(healNum);
			}

			// add health essence
			if (chargeNum >= 0){
				PlayerInventoryS.I.AddCharge(chargeNum);
			}

			// add la
			if (laNum >= 0){
				PlayerInventoryS.I.AddLaPickup(laNum, laToGive);
			}

			// add vp increase
			if (vpNum >= 0){
				PlayerInventoryS.I.AddVP(vpNum);
				pRef.myStats.AddStat(3);
			}
		}

		if (setProgress > -1){
			StoryProgressionS.SetStory(setProgress);
		}
		if (removeProgress > -1){
			StoryProgressionS.RemoveProgress(removeProgress);
		}
		
		if (saveOnPickup){
			StoryProgressionS.SaveProgress();
		}


	}

	private void CheckUnlock(){

		foreach (int i in PlayerInventoryS.I.collectedItems){
			if (i == keyInt){
				unlocking = true;
			}
		}

	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" && !unlocking){
			if (!pRef){
				pRef = other.gameObject.GetComponent<PlayerController>();
			}
            if (pRef.myDetect.allEnemiesInRange.Count <= 0 || ignoreInCombat){
				if (examineLabelNoController != ""){
					if (!pRef.myControl.ControllerAttached()){
						pRef.SetExamining(true, examinePos, examineLabelNoController);
					}else{
						pRef.SetExamining(true, examinePos, examineLabel);
					}
				}else{
					pRef.SetExamining(true, examinePos, examineLabel);
				}
			}
			playerInRange = true;
		}
	}

	/*void OnEnable(){
		delayOnTime = delayOnTimeStart;
	}**/

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			pRef.SetExamining(false, examinePos);
			playerInRange = false;
		}
	}

	void OnDisable(){
		if (pRef != null){
			if (pRef.examining){
				pRef.SetExamining(false, Vector3.zero);
			}
		}
	}
}
