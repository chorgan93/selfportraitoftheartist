using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollowS : MonoBehaviour {

	//_________________________________________________CLASS PROPERTIES

	private float _camEasing = 0.1f;
	private Vector3 _camPosOffset = new Vector3(0,0,-10);

	private Vector3 _currentPos;

	//__________________________________________________INSTANCE PROPERTIES

	private GameObject _poi;
	private GameObject defaultPoi;
	private GameObject overrideResetPoi;

	private float startOrthoSize;
	private float focusMult = 0.9f;
	private float punchInMult = 0.7f;
	private float punchInMultDeath = 0.4f;
	private Camera myCam;
	private float _punchHangTime = 0f;

	private float minX;
	private float maxX;
	private float minY;
	private float maxY;
	private bool useLimits = false;

	private List<GameObject> poiQueue;
	private List<float> poiDelayTimes;
	private float delayMoveTime;
	private bool queueOver = true;

	private bool zoomingIn = false;
	private bool dialogueZoom = false;
	private float zoomMult = 0.5f;
	private float dialogueZoomMult = 0.75f;

	private PlayerController playerRef;

	public static CameraFollowS F;
	
	//_________________________________________________GETTERS AND SETTERS
	
	public float camEasing	{ get { return _camEasing; } }
	public GameObject poi	{ get { return _poi; } }
	public Vector3	currentPos {get { return _currentPos; }}

	//__________________________________________________UNITY METHODS

	void Awake(){
		F = this;
	}

	// Use this for initialization
	void Start () {

		defaultPoi = _poi = GameObject.Find("CameraPOI");

		myCam = GetComponent<Camera>();
		startOrthoSize = myCam.orthographicSize;

		poiQueue = new List<GameObject>();
		poiDelayTimes = new List<float>();

		if (GameObject.Find("Player") != null){
			playerRef = GameObject.Find("Player").GetComponent<PlayerController>();
		}

			
			Vector3 camPos = poi.transform.position+_camPosOffset;
			_currentPos = transform.position;
			transform.position = camPos;


	
	}
	
	// Update is called once per frame
	void Update(){

		if (_punchHangTime > 0f){
			_punchHangTime -= Time.deltaTime;
		}

	}

	void FixedUpdate () {
		
		SetPOIS();
		FollowPOI();
	
	}

	//____________________________________________________PRIVATE METHODS

	private void FollowPOI(){


		Vector3 camPos = poi.transform.position+_camPosOffset;

	
			_currentPos = transform.position;
			_currentPos.x = (1-_camEasing)*_currentPos.x + _camEasing*camPos.x;
			_currentPos.y = (1-_camEasing)*_currentPos.y + _camEasing*camPos.y;
	
			if (CameraShakeS.C.isShaking){
				_currentPos += CameraShakeS.C.shakeOffset;
			}

		if (useLimits){
		if (_currentPos.x > maxX){
			_currentPos.x = maxX;
		}

		if (_currentPos.x < minX){
			_currentPos.x = minX;
		}

		if (_currentPos.y > maxY){
			_currentPos.y = maxY;
		}
		
		if (_currentPos.y < minY){
			_currentPos.y = minY;
		}
		}
			
			transform.position = _currentPos;

		
		if (_punchHangTime <= 0f){
			if (zoomingIn){
				if (dialogueZoom){
					myCam.orthographicSize = (1-_camEasing)*myCam.orthographicSize + _camEasing*startOrthoSize*dialogueZoomMult;
				}else{
					myCam.orthographicSize = (1-_camEasing)*myCam.orthographicSize + _camEasing*startOrthoSize*zoomMult;
				}
			}
			else{
				if (!CameraShakeS.C.isSleeping){
	
					myCam.orthographicSize = (1-_camEasing)*myCam.orthographicSize + _camEasing*startOrthoSize*focusMult;
				
	
				}
			}
		}

	}

	private void SetPOIS(){

		if (!queueOver){
			delayMoveTime -= Time.deltaTime;

			if (delayMoveTime <= 0){
			
				poiQueue.RemoveAt(0);
				poiDelayTimes.RemoveAt(0);

				if (poiQueue.Count > 0){
					SetNewPOI(poiQueue[0]);
					delayMoveTime = poiDelayTimes[0];
				}else{
					if (overrideResetPoi){
						SetNewPOI(overrideResetPoi);
						playerRef.SetTalking(true);
						overrideResetPoi = null;
						queueOver = true;
					}else{
						ResetPOI();
					}
				}

			}
		}

	}

	//__________________________________________________PUBLIC METHODS
	public void PunchIn(){

		myCam.orthographicSize =  startOrthoSize * punchInMult;

	}

	public void PunchInCustom(float punchMult, float punchHangTime){
		
		myCam.orthographicSize =  startOrthoSize * punchMult;
		_punchHangTime = punchHangTime;
		
	}

	public void PunchInBig(){

		myCam.orthographicSize =  startOrthoSize * punchInMultDeath;

	}

	public void SetZoomIn(bool zoom){
		zoomingIn = zoom;
		dialogueZoom = false;
	}
	public void SetDialogueZoomIn(bool zoom){
		zoomingIn = zoom;
		dialogueZoom = zoom;
	}
	public void EndZoom(){
		zoomingIn = dialogueZoom = false;
	}

	public void PunchCombatEnd(Vector3 targetPos){


			CameraShakeS.C.TimeSleep(0.2f);
	
			Vector3 camPos = targetPos+_camPosOffset;
			
			_currentPos = transform.position;
			_currentPos.x = camPos.x;
			_currentPos.y = camPos.y;
		
			transform.position = _currentPos;

			myCam.orthographicSize =  startOrthoSize * punchInMultDeath;



	}

	public void SetLimits(float mX, float mxX, float mY, float mxY){

		minX = mX;
		minY = mY;
		maxX = mxX;
		maxY = mxY;

		useLimits = true;

	}

	public void RemoveLimits(){

		useLimits = false;

	}

	public void SetNewPOI(GameObject newPoi){
		_poi = newPoi;
		if (playerRef){
			playerRef.SetTalking(true);
		}
	}

	public void ResetPOI(bool isCinematic = false){
		_poi = defaultPoi;
		queueOver = true;
		if (playerRef){
			playerRef.SetTalking(isCinematic);
		}
	}

	public void AddToQueue(GameObject newPoi, float poiTime){
		poiQueue.Add(newPoi);
		poiDelayTimes.Add(poiTime);

		if (poiQueue.Count == 1){
			SetNewPOI(poiQueue[0]);
			delayMoveTime = poiDelayTimes[0];
		}

		queueOver = false;
	}
	public void SetOverrideResetPOI(GameObject newOverride){
		overrideResetPoi = newOverride;
	}


}
