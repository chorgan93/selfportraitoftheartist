using UnityEngine;
using System.Collections;

public class ExamineTriggerS : MonoBehaviour {

	public string examineLabel = "";
	public string examineLabelNoController = "";
	public string examineString;
	public string unlockString;
	public int costToExamine = -1;
	public GameObject examineSound;

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
	public int staminaNum = -1;
	public int numToAdd = 1;
	public bool keyItem = false;
	public ActivateOnExamineS myTrigger;
	public BuddyS buddyToGive;
	public bool forceBuddySwitch = false;
	public PlayerWeaponS mantraToGive;

	public bool advanceProgress = false;
	public int setProgress = -1;
	public bool saveOnPickup = false;

	public bool inInfiniteMode = false;
	private InfinitySpawnS parentInfinite;

	[Header("FOR DEMO REWORK AFTER")]
	public bool teleportItem = false;
	public string teleportScene = "InfiniteScene";
	public bool fullRevive = false;

	public float lookTime = -1f;

	void Start () {

		if (inventoryNum >= 0 || buddyToGive || (mantraToGive != null)){
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

	}

	// Update is called once per frame
	void Update () {

		if (turnOffBarrier){
			if (!turnOffBarrier.gameObject.activeSelf){
				gameObject.SetActive(false);
				if (playerInRange){
					pRef.SetExamining(false);
				}
			}
		}
	
		if (playerInRange){

			if (!pRef._inCombat){

			if (!pRef.myControl.TalkButton()){
				talkButtonDown = false;
			}

			if (pRef.myControl.TalkButton() && !talkButtonDown){
				talkButtonDown = true;

				if (!talking && examineString != ""){
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
							DialogueManagerS.D.SetDisplayText(examineString);
						}else{
							DialogueManagerS.D.SetDisplayText(unlockString);
						}
					}

				}else{
					
					if (costToExamine < PlayerCollectionS.currencyCollected && !CameraEffectsS.E.isFading){
						if (examineString == "" && examineSound != null){
							Instantiate(examineSound);
						}
	
						if (costToExamine > 0){
							pRef.myStats.uiReference.cDisplay.AddCurrency(-costToExamine);
						}
	
						if (DialogueManagerS.D.doneScrolling){
							pRef.SetTalking(false);
							CameraFollowS.F.ResetPOI();
							DialogueManagerS.D.EndText();
							talking = false;
	
							if (inInfiniteMode){
								parentInfinite.AddClear();
							}

							if (teleportItem){
								if (!fullRevive){
									InfinityS.savedLastDifficulty = 1;
								}
								PlayerInventoryS.I.SaveLoadout(pRef.equippedWeapons, pRef.subWeapons, pRef.equippedBuddies);
								CameraEffectsS.E.SetNextScene(teleportScene);
								CameraEffectsS.E.FadeIn();
							}
	
							if (myTrigger){
								myTrigger.TurnOn();
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
							}
	
							if (unlocking){
								turnOffBarrier.TurnOff();
								pRef.SetExamining(false);
								Destroy(gameObject);
							}
	
							if (consumable){
								pRef.SetExamining(false);
								Destroy(gameObject);
							}
						}else{
							DialogueManagerS.D.CompleteText();
						}
					}
				}
			}
			}else{
				if (pRef.examining){
					pRef.SetExamining(false);
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

		if (PlayerInventoryS.I.collectedItems.Count > 0){
			foreach (int i in PlayerInventoryS.I.collectedItems){
				if (i == inventoryNum && inventoryNum != 0){
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

		if (inventoryNum == 0 && PlayerInventoryS.I.CheckHeal(healNum)){
			gameObject.SetActive(false);
		}
		if (inventoryNum == 1 && PlayerInventoryS.I.CheckStim(staminaNum)){
			gameObject.SetActive(false);
		}
		if (inventoryNum == 2 && PlayerInventoryS.I.CheckCharge(chargeNum)){
			gameObject.SetActive(false);
		}


	}

	private void AddPickup(){

		for (int i = 0; i < numToAdd; i++){
			if (inventoryNum >= 0){
				PlayerInventoryS.I.AddToInventory(inventoryNum, keyItem);
			}
			if (healNum >= 0){
				PlayerInventoryS.I.AddHeal(healNum);
			}
			if (chargeNum >= 0){
				PlayerInventoryS.I.AddCharge(chargeNum);
			}
			if (staminaNum >= 0){
				PlayerInventoryS.I.AddStamina(staminaNum);
			}
		}

		if (advanceProgress){
			StoryProgressionS.AdvanceStory();
		}else if (setProgress > -1){
			StoryProgressionS.SetStory(setProgress);
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
			if (pRef.myDetect.allEnemiesInRange.Count <= 0){
				if (examineLabelNoController != ""){
					if (!pRef.myControl.ControllerAttached()){
						pRef.SetExamining(true, examineLabelNoController);
					}else{
						pRef.SetExamining(true, examineLabel);
					}
				}else{
					pRef.SetExamining(true, examineLabel);
				}
			}
			playerInRange = true;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			pRef.SetExamining(false);
			playerInRange = false;
		}
	}
}
