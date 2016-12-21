using UnityEngine;
using System.Collections;

public class LockedDoorS : MonoBehaviour {

	public string lockString;
	public string unlockString;
	public string examineLabel;
	public int keyID;

	private bool isTalking = false;
	private bool talkButtonDown = false;
	private bool unlocking = false;

	private PlayerController pRef;
	private PlayerInteractCheckS interactRef;
	private bool playerInRange;

	public GameObject unlockSound;
	public SpriteRenderer mySprite;
	public Collider myCollider;
	private Color fadeColor;
	private bool fading = false;
	private float fadeRate = 1f;

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
				gameObject.SetActive(false);
				fading = false;
			}else{
				mySprite.color = fadeColor;
			}
		}

		if (playerInRange && pRef != null){
			if (!pRef.inCombat){
			if (pRef.myControl.TalkButton()){

				if (!talkButtonDown){
					if (!isTalking && !fading){
						TriggerExamine();
					}
					else{
						if (DialogueManagerS.D.doneScrolling){
							DialogueManagerS.D.EndText();
							if (unlocking){
									TurnOffFade();
							}else{
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
			}else{
				if (pRef.examining){
					pRef.SetExamining(false);
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
		pRef.SetTalking(true);
		pRef.SetExamining(true);
		isTalking = true;
		unlocking = true;
		DialogueManagerS.D.SetDisplayText(unlockString);
		fading = true;

		if (unlockSound){
			Instantiate(unlockSound);
		}
	}

	private void TriggerExamine(){

		if (PlayerInventoryS.I.CheckForItem(keyID)){
			TriggerUnlock();
			PlayerInventoryS.I.AddClearedWall(keyID);
		}
		else{
			pRef.SetTalking(true);
			pRef.SetExamining(true);
			isTalking = true;
			DialogueManagerS.D.SetDisplayText(lockString);
		}

	}

	private void TurnOff(){
		if (playerInRange){
			pRef.SetExamining(false);
			pRef.SetTalking(false);
		}
		gameObject.SetActive(false);
	}

	private void TurnOffFade(){
		if (playerInRange){
			pRef.SetExamining(false);
			pRef.SetTalking(false);
		}
		isTalking = false;
		myCollider.enabled  =false;
		fading = true;
	}

	private void EndExamine(){
		pRef.SetTalking(false);
		pRef.SetExamining(false);
		isTalking = false;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			if (!pRef){
				pRef = other.gameObject.GetComponent<PlayerController>();
				interactRef = pRef.GetComponentInChildren<PlayerInteractCheckS>();
			}
			pRef.SetExamining(true, examineLabel);
			interactRef.AddDoor(this);
			playerInRange = true;
		}
	}
	
	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			pRef.SetExamining(false);
			playerInRange = false;
			interactRef.RemoveDoor(this);
		}
	}
}
