using UnityEngine;
using System.Collections;

public class ChangeSceneTriggerS : MonoBehaviour {

	public string nextSceneString = "";
	public bool requireExamine = false;
	public int whereToSpawn = 0;
	public int doorNum = -1;
	public string examineString = "";
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
				pRef.SetExamining(true, examineString);
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
					pRef.SetExamining(true, examineString);
				}
				examining = true;
			}
		}

	}

	void OnTriggerExit(Collider other){
		if (requireExamine){
			if (other.gameObject.tag == "Player"){
				pRef.SetExamining(false);
				examining = false;
			}
		}
	}

	void StartNextScene(){

		PlayerInventoryS.I.SaveLoadout(pRef.equippedWeapons, pRef.subWeapons, pRef.equippedBuddies);

		if (setProgressOnActivate > -1 && setProgressOnActivate > StoryProgressionS.storyProgress){
			StoryProgressionS.storyProgress = setProgressOnActivate;
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
