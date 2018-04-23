using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockedDoorS : MonoBehaviour {

	public string lockString;
	public string unlockString;
	public string examineLabel;
	public string examineLabelNoController;
	public Vector3 examinePos = new Vector3(0, 1f, 0);
	public int keyID;

	private bool isTalking = false;
	private bool talkButtonDown = false;
	private bool unlocking = false;

	private PlayerController pRef;
	private PlayerInteractCheckS interactRef;
	private bool playerInRange;

	public GameObject unlockSound;
	public SpriteRenderer mySprite;
	public List <SpriteRenderer> additionalSprites = new List<SpriteRenderer>();
	public Collider myCollider;
	private Color fadeColor;
	private bool fading = false;
	private float fadeRate = 1f;

	public GameObject setLook;
	public float resetLookTime = 0f;
	private float resetLookCount;
	private bool differentLook = false;
	private bool doResetLook = false;

	public ActivateOnDoorUnlockS lockedActivations;
	public ActivateOnDoorUnlockS unlockActivations;
	[Header("Code Properties")]
	public KeypadS myKeypad;
	private bool inKeypad = false;
	public string keypadFailString;
	private bool inKeypadFail = false;

	[Header("Sprite Properties")]
	public Sprite keyItemSprite;
	public Color keyItemSpriteColor = Color.white;
	public Color keyItemOutlineColor = Color.red;

	// Use this for initialization
	void Start () {

		if (PlayerInventoryS.I.clearedWalls.Contains(keyID)){
			TurnOff();
		}else{
			fadeColor = mySprite.color;
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (fading && !isTalking){
			fadeColor = mySprite.color;
			fadeColor.a -= fadeRate*Time.deltaTime;
			if (fadeColor.a <= 0){
				if (doResetLook){
					doResetLook = false;
					differentLook = false;
					CameraFollowS.F.ResetPOI();
					CameraFollowS.F.EndZoom();
				}
				gameObject.SetActive(false);
				fading = false;
			}else{
				mySprite.color = fadeColor;

				if (additionalSprites.Count > 0){
					for (int i = 0; i < additionalSprites.Count; i++){
						additionalSprites[i].color = fadeColor;
					}
				}
			}
		}


		if (pRef){
			if (!pRef.inCombat && !CameraEffectsS.E.isFading && !InGameMenuManagerS.menuInUse && !inKeypad){
			if (pRef.myControl.GetCustomInput(3)){

				if (!talkButtonDown){
					if (!isTalking && !fading && !pRef.talking){
						TriggerExamine();
					}
					else{
							if (isTalking){
						if (DialogueManagerS.D.doneScrolling){
								//Debug.Log("End text!");
							DialogueManagerS.D.EndText();
								if (differentLook){
									doResetLook = true;
									//Debug.Log("Start fade timer!");
								}else if (setLook){
									CameraFollowS.F.ResetPOI();
								}
							if (unlocking){
									TurnOffFade();
								}else if (myKeypad != null && !inKeypadFail){
									TurnOnKeypad();
								}else{
									EndExamine();
								}
							}else{
								DialogueManagerS.D.CompleteText();
							}
						}
						}
				}
				talkButtonDown = true;
			}else{
				talkButtonDown = false;
			}
			}else{
				if (pRef.examining){
					pRef.SetExamining(false, examinePos);
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
		if (setLook != null){
			CameraFollowS.F.SetNewPOI(setLook);
			if (resetLookTime > 0){
				resetLookCount = resetLookTime;
				differentLook = true;
			}
		}
		KeyItemUIS.K.EvaluateItems(true);
		pRef.SetTalking(true);
		pRef.SetExamining(true, examinePos);
		isTalking = true;
		unlocking = true;
		if (keyItemSprite){
			DialogueManagerS.D.SetItemFind(keyItemSprite, keyItemSpriteColor, keyItemOutlineColor);
		}
		DialogueManagerS.D.SetDisplayText(unlockString);
		fading = true;

		if (unlockSound){
			Instantiate(unlockSound);
		}
	}

	private void TriggerExamine(bool keypadFail = false){

		if (playerInRange || keypadFail){
		inKeypadFail = keypadFail;
		if (PlayerInventoryS.I.CheckForItem(keyID)){
				PlayerInventoryS.I.AddClearedWall(keyID);
				TriggerUnlock();
		}
		else{
			if (setLook != null){
				CameraFollowS.F.SetNewPOI(setLook);
			}
			pRef.SetTalking(true);
			pRef.SetExamining(true, examinePos);
			isTalking = true;
			if (keypadFail){
				DialogueManagerS.D.SetDisplayText(keypadFailString);
			}else{
				DialogueManagerS.D.SetDisplayText(lockString);
			}
		}
		}

	}

	private void TurnOff(){
		if (playerInRange){
			pRef.SetExamining(false, examinePos);
			pRef.SetTalking(false);
		}
		gameObject.SetActive(false);

		for (int i = 0; i < additionalSprites.Count; i++){
			additionalSprites[i].gameObject.SetActive(false);
		}

		TriggerOnOff();
	}

	private void TurnOffFade(){
		if (playerInRange){
			pRef.SetExamining(false, examinePos);
			pRef.SetTalking(false);
		}
		isTalking = false;
		myCollider.enabled  =false;
		fading = true;

		TriggerOnOff();
	}

	private void TurnOnKeypad(){
		pRef.SetTalking(true);
		inKeypad = true;
		myKeypad.TurnOn(this, pRef.myControl);
	}

	public void EndKeypad(bool correct){
		inKeypad = false;
		if (correct){
			PlayerInventoryS.I.AddToInventory(keyID, false, true);
			TriggerExamine();
		}else{
			TriggerExamine(true);
		}
	}

	void TriggerOnOff(){
		if (unlockActivations != null){
			unlockActivations.Activate();
		}
	}

	void TriggerLockOnOff(){
		if (lockedActivations != null){
			lockedActivations.Activate();
		}
	}

	private void EndExamine(){
		pRef.SetTalking(false);
		pRef.SetExamining(true, examinePos);
		isTalking = false;
		TriggerLockOnOff();
		inKeypadFail = false;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			if (!pRef){
				pRef = other.gameObject.GetComponent<PlayerController>();
				interactRef = pRef.GetComponentInChildren<PlayerInteractCheckS>();
			}
			if (!fading){
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
			interactRef.AddDoor(this);
			playerInRange = true;
		}
	}
	
	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			pRef.SetExamining(false, examinePos);
			playerInRange = false;
			interactRef.RemoveDoor(this);
		}
	}
}
