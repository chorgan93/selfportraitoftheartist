using UnityEngine;
using System.Collections;

public class NPCS : MonoBehaviour {

	public enum DialogueStructure {Loop, RepeatLast};
	public DialogueStructure dialogueType = DialogueStructure.Loop;

	[Header("Talk Properties")]
	public string talkLabel = "";
	public string talkLabelNoController = "";
	public Vector3 examinePos = new Vector3(0, 1f, 0);
	public NPCDialogueSet[] dialogues;
	private int currentDialogue = 0;
	private int currentDialogueIndex = 0;
	public GameObject talkSound;
	public float talkX = 0;

	public GameObject newPoi;
	
	private bool playerInRange = false;
	private PlayerController pRef;
	private bool talkButtonDown = false;
	private bool talking = false;

	private Rigidbody _myRigid;
	private Animator _myAnimator;

	
	[Header("Walk Properties")]
	public GameObject[] wayPoints;
	private int currentWayPoint = 0;
	public float walkSpeed = 200f;
	public float walkTimeMin;
	public float walkTimeMax;
	private float currentWalkTime;
	public float changeDestinationTimeMin;
	public float changeDestinationTimeMax;
	private float currentDestinationTime;
	public float waitTimeMin;
	public float waitTimeMax;
	private float currentWaitTime;

	private bool isWaiting = false;

	[Header("Animation Properties")]
	public string talkKey;
	public string walkKey;
	public string idleKey;

	private Vector3 _currentDestination = Vector3.zero;
	private Vector3 startScale;
	private Vector3 flipScale;
	private Vector3 talkScale;
	private Vector3 flipTalkScale;

	[Header("Sound Properties")]
	public GameObject mixOnTalk;
	public GameObject mixOnEnd;


    [Header("Special Properties")]
    public GameObject activateOnTalkEnd;


	
	void Start () {

		_myRigid = GetComponent<Rigidbody>();
		_myAnimator = GetComponent<Animator>();

		startScale = flipScale = transform.localScale;
		flipScale.x *= -1f;
		if (talkX == 0){
			talkScale = startScale;
			flipTalkScale = startScale;
		}else{
			talkScale = startScale;
			talkScale.x *= talkX;
			flipTalkScale = flipScale;
			flipTalkScale.x *= talkX;
		}

		float whereToBegin = Random.Range(0,100);
		if (whereToBegin > 50){
			TriggerWait();
		}else{
			SetDestination();
			TriggerWalk();
		}
		
	}
	
	// Update is called once per frame
	void Update () {

		
        if ((playerInRange && pRef.myDetect.closestEnemy == null) || talking){
			
			if (!pRef.myControl.GetCustomInput(3)){
				talkButtonDown = false;
			}
			
			if (pRef.myControl.GetCustomInput(3) && !talkButtonDown){
				talkButtonDown = true;
				
				if (!talking){
						if (!pRef.talking){
						pRef.SetTalking(true);
						if (newPoi){
							CameraFollowS.F.SetNewPOI(newPoi);
						}
					currentDialogueIndex = 0;
						if (mixOnTalk){
							GameObject newMix = Instantiate(mixOnTalk) as GameObject;
							newMix.SetActive(true);
						}
						
					DialogueManagerS.D.SetDisplayText(dialogues[currentDialogue].dialogueStrings[currentDialogueIndex]);

						if (talkSound){
							Instantiate(talkSound);
						}
					talking = true;
					_myRigid.velocity = Vector3.zero;
					_myAnimator.ResetTrigger(idleKey);
					_myAnimator.ResetTrigger(walkKey);
					_myAnimator.SetTrigger(talkKey);

					if (pRef.transform.position.x > transform.position.x){
						transform.localScale = talkScale;
					}else{
						transform.localScale = flipTalkScale;
					}


					}
				}else{

						if (talkSound != null){
							Instantiate(talkSound);
						}

						if (DialogueManagerS.D.doneScrolling){
						currentDialogueIndex++;
						if (currentDialogueIndex >= dialogues[currentDialogue].dialogueStrings.Length){
							pRef.SetTalking(false);
							CameraFollowS.F.ResetPOI();
							DialogueManagerS.D.EndText();
							talking = false;
							currentDialogue++;
							currentDialogueIndex = 0;
							if (currentDialogue > dialogues.Length-1){
								if (dialogueType == DialogueStructure.Loop){
									currentDialogue = 0;
								}else{
									currentDialogue = dialogues.Length-1;
								}
							}
							if (mixOnEnd){
								GameObject newMix = Instantiate(mixOnEnd) as GameObject;
								newMix.SetActive(true);
							}
                            if (activateOnTalkEnd){
                                activateOnTalkEnd.SetActive(true);
                            }
							if (isWaiting){
								_myAnimator.ResetTrigger(talkKey);
								_myAnimator.SetTrigger(walkKey);
							}else{
								_myAnimator.ResetTrigger(talkKey);
								_myAnimator.SetTrigger(idleKey);
							}
						}else{
							DialogueManagerS.D.SetDisplayText(dialogues[currentDialogue].dialogueStrings[currentDialogueIndex]);
						}
							


						}else{
							DialogueManagerS.D.CompleteText();
						}

				}
			}
		}
		
	}

	void FixedUpdate(){
		HandleMovement();
	}

	private void HandleMovement(){

		if (!talking){

			if (isWaiting){
				currentWaitTime -= Time.deltaTime;
				if (currentWaitTime <= 0){
					SetDestination();
					TriggerWalk();
				}
			}else{
	
				currentDestinationTime -= Time.deltaTime;
				if (currentDestinationTime <= 0){
					SetDestination();
				}

				_myRigid.AddForce((_currentDestination-transform.position).normalized*Time.deltaTime*walkSpeed);

				if (_currentDestination.x < transform.position.x){
					transform.localScale = flipScale;
				}
				if (_currentDestination.x > transform.position.x){
					transform.localScale = startScale;
				}

				currentWalkTime -= Time.deltaTime;
				if (currentWalkTime <= 0){
					TriggerWait();
				}
			}
		}
	}

	private void TriggerWalk(){

		if (walkTimeMax > 0){
			if (isWaiting){
				_myAnimator.ResetTrigger(idleKey);
				_myAnimator.ResetTrigger(talkKey);
				_myAnimator.SetTrigger(walkKey);
			}
			isWaiting = false;
			SetWalkTime();
			SetDestinationTime();
		}else{
			TriggerWait();
		}

	}

	private void TriggerWait(){

		if (waitTimeMax > 0){
			_myRigid.velocity = Vector3.zero;
			if (!isWaiting){
				_myAnimator.ResetTrigger(talkKey);
				_myAnimator.ResetTrigger(walkKey);
				_myAnimator.SetTrigger(idleKey);
			}
			isWaiting = true;
			SetWaitTime();
		}
		else{
			TriggerWalk();
		}

	}

	private void SetWaitTime(){
		
		currentWaitTime = Random.Range(waitTimeMin, waitTimeMax);
		
	}

	private void SetWalkTime(){
		
		currentWalkTime = Random.Range(walkTimeMin, walkTimeMax);
		
	}

	private void SetDestination(){

		int newWayPoint = Mathf.RoundToInt(Random.Range(0, wayPoints.Length-1));

		if (newWayPoint != currentWayPoint){
			currentWayPoint = newWayPoint;
		}else{
			currentWayPoint ++;
			if (currentWayPoint > wayPoints.Length-1){
				currentWayPoint = 0;
			}
		}
		
		_currentDestination = wayPoints[currentWayPoint].transform.position;

		SetDestinationTime();

	}

	private void SetDestinationTime(){

		currentDestinationTime = Random.Range(changeDestinationTimeMin, changeDestinationTimeMax);

	}
	

	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			if (!pRef){
				pRef = other.gameObject.GetComponent<PlayerController>();
			}
			if (pRef.myControl.ControllerAttached() || talkLabelNoController == ""){
				pRef.SetExamining(true, examinePos, talkLabel);
			}else{
				pRef.SetExamining(true, examinePos, talkLabelNoController);
			}
			playerInRange = true;
		}

		if (other.gameObject == wayPoints[currentWayPoint]){
			TriggerWait();
		}
	}
	
	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			pRef.SetExamining(false, examinePos);
			playerInRange = false;
		}
	}
}
