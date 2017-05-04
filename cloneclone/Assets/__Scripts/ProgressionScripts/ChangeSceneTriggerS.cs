using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeSceneTriggerS : MonoBehaviour {

	public string nextSceneString = "";
	public bool requireExamine = false;
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
	public int setProgressOnActivate = -1;

	void Start(){
		if (openedSprite != null){
			_myRender=GetComponentInChildren<SpriteRenderer>();
			if (PlayerInventoryS.I.openedDoors.Contains(doorNum)){
				_myRender.sprite = openedSprite;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (requireExamine && examining && !loading){

			if (pRef.myDetect.allEnemiesInRange.Count <= 0){
				if (pRef.myControl.ControllerAttached() || examineStringNoController == ""){
					pRef.SetExamining(true, examinePos, examineString);
				}else{
					pRef.SetExamining(true, examinePos, examineStringNoController);
				}
			}

			if (pRef.myControl.TalkButton()){
				if (!talkButtonDown && pRef.examining){
					StartNextScene();
					pRef.SetTalking(true);
				}
				talkButtonDown = true;
			}else{
				talkButtonDown = false;
			}
		}
	
	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Player" && nextSceneString!="" && !loading){

			if (!pRef){
				pRef = other.GetComponent<PlayerController>();
			}

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

	void OnTriggerExit(Collider other){
		if (requireExamine){
			if (other.gameObject.tag == "Player"){
				pRef.SetExamining(false, examinePos);
				examining = false;
			}
		}
	}

	void StartNextScene(){

		List<int> saveBuddyList = new List<int>();
		saveBuddyList.Add(pRef.ParadigmIBuddy().buddyNum);
		if (pRef.ParadigmIIBuddy() != null){
			saveBuddyList.Add(pRef.ParadigmIIBuddy().buddyNum);
		}
		PlayerInventoryS.I.SaveLoadout(pRef.equippedWeapons, pRef.subWeapons, saveBuddyList);

		if (setProgressOnActivate > -1){
			StoryProgressionS.SetStory(setProgressOnActivate);
		}

		CameraEffectsS.E.SetNextScene(nextSceneString);
		CameraEffectsS.E.FadeIn();
		loading = true;

		VerseDisplayS.V.EndVerse();

		SpawnPosManager.whereToSpawn = whereToSpawn;

		if (doWakeUp){
			PlayerController.doWakeUp = true;
		}

		if (requireExamine && doorNum > -1){
			PlayerInventoryS.I.AddOpenDoor(doorNum);
		}
	}
}
