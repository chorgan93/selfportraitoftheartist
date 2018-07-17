using UnityEngine;
using System.Collections;

public class BuddyNPCS : MonoBehaviour {

	public int buddyNum = -1;

	private float nudgeInTime = 0.6f;
	private bool nudgingIn = false;
	private float nudgeOutTime = 1.2f;
	private bool nudgingOut = false;
	private float nudgeCount;
	private float nudgeT;

	private float embraceTime = 1f;
	private float embraceCountdown;
	private bool embracing = false;

	[Header("Examine Properties")]
	public string examineString;
	public string examineStringNoController;
	public Vector3 examinePos = new Vector3(0, 1f, 0);

	[Header("Movement Properties")]
	public float moveTime;
	private float moveCount;
	private float moveT;
	public float moveTargetRadius;
	private bool isMoving = false;

	public float waitTimeMin = 1f;
	public float waitTimeMax = 3f;
	private float waitTime;

	private Vector3 centerPos;
	private Vector3 startMovePos;
	private Vector3 endMovePos;

	public Vector3 nudgeOffset;
	private Vector3 currentOffset;

	public float flipXMult = -1f;
	private Vector3 startScale;
	private Vector3 flipScale;

	private PlayerDetectS myDetect;
	private bool talkButtonDown = false;


	[Header("Effect Properties")]
	public GameObject petEffectPrefab;
	public float spawnEffectWaitTime = 0.3f;
	private float spawnEffectCount;
	private bool spawnedEffect = false;
	private Color effectCol;

	// Use this for initialization
	void Start () {
	
		myDetect = GetComponent<PlayerDetectS>();
		myDetect.examineString = examineString;
		myDetect.examineStringNoController = examineStringNoController;
		centerPos = transform.position;
		waitTime = Random.Range(waitTimeMin, waitTimeMax);
		startScale = transform.localScale;
		flipScale = startScale;
		flipScale.x*=flipXMult;

		effectCol = GetComponent<SpriteRenderer>().color;

		CheckAvailable();

	}
	
	// Update is called once per frame
	void Update () {

		if (nudgingIn){
			nudgeCount += Time.deltaTime;
			nudgeT = nudgeCount/nudgeInTime;
			if (nudgeT >= 1){
				nudgeT = 1f;
				nudgingIn = false;
				embracing = true;
				embraceCountdown = embraceTime;
			}
			nudgeT = Mathf.Sin(nudgeT * Mathf.PI * 0.5f);
			transform.position = Vector3.Lerp(startMovePos,endMovePos,nudgeT);
		}
		else if (embracing){
			if (!spawnedEffect){
				spawnEffectCount -= Time.deltaTime;
				if (spawnEffectCount <= 0){
					Vector3 spawnPos = transform.position;
					spawnPos.z+=1f;
					spawnPos.x = (myDetect.player.transform.position.x + transform.position.x)/2f;
					spawnPos.y += 0.3f;
					GameObject newEffect = Instantiate(petEffectPrefab, spawnPos, Quaternion.identity) as GameObject;
					newEffect.GetComponent<BuddyPetEffectS>().FireEffect(effectCol, myDetect.player.myRenderer.transform.localScale.x<0f);
					spawnedEffect = true;
				}
			}
			embraceCountdown -= Time.deltaTime;
			if (embraceCountdown <= 0){
				EndEmbrace();
			}
		}
		else if (nudgingOut){
			nudgeCount += Time.deltaTime;
			nudgeT = nudgeCount/nudgeOutTime;
			if (nudgeT >= 1){
				nudgeT = 1f;
				nudgingOut = false;
				waitTime = Random.Range(waitTimeMin, waitTimeMax);
				SetPlayerExamine(true);
			}
			nudgeT = Mathf.Sin(nudgeT * Mathf.PI * 0.5f);
			transform.position = Vector3.Lerp(startMovePos,endMovePos,nudgeT);
		}else{
			if (myDetect.PlayerInRange()){
				if (talkButtonDown && !myDetect.player.myControl.GetCustomInput(3)){
					talkButtonDown = false;
				}
				if (!myDetect.player.talking && myDetect.player.myControl.GetCustomInput(3) && !talkButtonDown){
					StartNudge();
				}
				if (!talkButtonDown && myDetect.player.myControl.GetCustomInput(3)){
					talkButtonDown = true;
				}
			}
			if (isMoving){
				moveCount += Time.deltaTime;
				moveT = moveCount/moveTime;
				if (moveT >= 1){
					moveT = 1f;
					isMoving = false;
					SetPlayerExamine(true);
					waitTime = Random.Range(waitTimeMin, waitTimeMax);
				}
				moveT = Mathf.Sin(moveT * Mathf.PI * 0.5f);
				transform.position = Vector3.Lerp(startMovePos,endMovePos,moveT);
				
			}else{
				waitTime -= Time.deltaTime;
				if (waitTime <= 0){
					StartMove();
				}
			}
		}
	}

	void FaceMove(){
		if (startMovePos.x < endMovePos.x){
			transform.localScale = startScale;
		}else{
			transform.localScale = flipScale;
		}
	}
	void FacePlayer(){
		if (myDetect.PlayerInRange()){
			if (endMovePos.x < myDetect.player.transform.position.x){
				transform.localScale = startScale;
			}else{
				transform.localScale = flipScale;
			}
		}else{
			if (startMovePos.x < endMovePos.x){
				transform.localScale = startScale;
			}else{
				transform.localScale = flipScale;
			}
		}
	}

	void StartNudge(){
		talkButtonDown = true;
		SetPlayerExamine(false);
		myDetect.player.StartEmbrace();
		nudgeCount = 0f;
		isMoving = false;
		moveCount = 0f;
		waitTime = Random.Range(waitTimeMin, waitTimeMax);
		nudgingIn = true;
		CameraFollowS.F.SetZoomIn(true);
		startMovePos = transform.position;
		endMovePos = myDetect.player.transform.position;
		currentOffset = nudgeOffset;
		spawnedEffect = false;
		spawnEffectCount = spawnEffectWaitTime;

		if (myDetect.player.myRenderer.transform.localScale.x < 0){
			
			currentOffset.x *= -1f; 
		}

		endMovePos += currentOffset;
		endMovePos.z = transform.position.z;
		FacePlayer();
	}

	void StartMove(){
		isMoving = true;
		moveCount = 0f;
		startMovePos = transform.position;
		endMovePos = centerPos+moveTargetRadius*Random.insideUnitSphere;
		endMovePos.z = transform.position.z;
		FaceMove();
	}

	void EndEmbrace(){
		myDetect.player.EndEmbrace();
		embracing = false;
		CameraFollowS.F.SetZoomIn(false);
		nudgingOut = true;
		nudgeCount = 0f;
		startMovePos = transform.position;
		endMovePos = centerPos+moveTargetRadius*Random.insideUnitSphere;
		endMovePos.z = transform.position.z;
		FaceMove();
	}

	void SetPlayerExamine(bool messageOn){
		if (myDetect.PlayerInRange()){
			if (messageOn){
				if (myDetect.player.myControl.ControllerAttached() || examineStringNoController == ""){
					myDetect.player.SetExamining(true, examinePos, examineString);
				}else{
					myDetect.player.SetExamining(true, examinePos, examineStringNoController);
				}
			}else{
				myDetect.player.SetExamining(false, examinePos);
			}
		}
	}

	void CheckAvailable(){
		bool isAvail = false;
        if (PlayerInventoryS.I != null && !PlayerController.killedFamiliar){
			for (int i = 0; i < PlayerInventoryS.I.unlockedBuddies.Count; i++){
				if (PlayerInventoryS.I.unlockedBuddies[i].buddyNum == buddyNum){
					isAvail = true;
				}	
			}
		}
		gameObject.SetActive(isAvail);
	}
}
