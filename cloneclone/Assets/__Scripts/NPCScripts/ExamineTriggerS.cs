using UnityEngine;
using System.Collections;

public class ExamineTriggerS : MonoBehaviour {

	public string examineLabel = "";
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
	public ActivateOnExamineS myTrigger;

	public bool inInfiniteMode = false;
	private InfinitySpawnS parentInfinite;

	[Header("FOR DEMO REWORK AFTER")]
	public bool teleportItem = false;
	public string teleportScene = "InfiniteScene";
	public bool fullRevive = false;

	public float lookTime = -1f;

	void Start () {

		if (inventoryNum >= 0){
			CheckInventory();
		}

		if (lookTime > 0){
			CameraFollowS.F.AddToQueue(gameObject, lookTime);
		}

		if (inInfiniteMode){
			parentInfinite = GetComponentInParent<InfinitySpawnS>();
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
	
		if (playerInRange && pRef.myDetect.closestEnemy == null){

			if (!pRef.myControl.TalkButton()){
				talkButtonDown = false;
			}

			if (pRef.myControl.TalkButton() && !talkButtonDown){
				talkButtonDown = true;

				if (!talking && examineString != ""){
					if (costToExamine < PlayerCollectionS.currencyCollected && !CameraEffectsS.E.isFading){

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
								CameraEffectsS.E.SetNextScene(teleportScene);
								CameraEffectsS.E.FadeIn();
							}
	
							if (myTrigger){
								myTrigger.TurnOn();
							}
	
							if (inventoryNum >= -1){
								AddPickup();
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
		}

	}

	private void CheckInventory () {

		if (PlayerInventoryS.I.collectedItems.Count > 0){
			foreach (int i in PlayerInventoryS.I.collectedItems){
				if (i == inventoryNum){
					gameObject.SetActive(false);
				}
			}
		}

	}

	private void AddPickup(){

		if (inventoryNum >= 0){
			// PlayerInventoryS.I.AddToInventory(inventoryNum);
	
			// add stamina
			if (inventoryNum >= 0 && inventoryNum <= 11){
				pRef.myStats.AddStamina();
			}
	
			// add health
			if (inventoryNum >= 12 && inventoryNum <= 20){
				pRef.myStats.AddHealth();
			}

			// 
		}else{
			// full recovery
			pRef.myStats.FullRecover();
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
			pRef.SetExamining(true, examineLabel);
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
