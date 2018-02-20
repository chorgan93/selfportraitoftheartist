using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollowS : MonoBehaviour {

	//_________________________________________________CLASS PROPERTIES

	private float _camEasing = 0.1f;
	private Vector3 _camPosOffset = new Vector3(0,0,-10);

	private float slowZoomMult = 0.1f;

	private Vector3 _currentPos;

	private float RECORD_MODE_ORTHO_MULT = 0.9f;
	private const float STUN_ORTHO_MULT = 0.9f;

	//__________________________________________________INSTANCE PROPERTIES

	private GameObject _poi;
	private GameObject defaultPoi;
	private GameObject overrideResetPoi;

	public Vector3 offsetPos = Vector3.zero;
	public float followSpeedMultiplier = 1f;

	private float startOrthoSize;
	private float focusMult = 0.95f;
	private float punchInMult = 0.9f;
	private float punchInMultDeath = 0.6f;
	private Camera myCam;
	private float _punchHangTime = 0f;

	private float stunOrthoSize;

	private float minX;
	private float maxX;
	private float minY;
	private float maxY;
	private bool useLimits = false;

	private List<GameObject> poiQueue;
	private List<float> poiDelayTimes = new List<float>();
	private float delayMoveTime;
	private bool queueOver = true;

	private bool zoomingIn = false;
	private bool dialogueZoom = false;
	private bool slowerZoom = false;
	private float zoomMult = 0.5f;
	private float dialogueZoomMult = 0.75f;

	private float orthoSizeMult = 1f;
	public float orthoMultRef { get { return orthoSizeMult; } }
	private bool slowChange = false;
	private float slowChangeCount = 0f;

	private PlayerController playerRef;

	public static CameraFollowS F;
	public static float ZOOM_LEVEL = 1f;
	public static int zoomInt = 0;
	private const float zoomLevelDiff = 0.004f;
	private const int zoomLevelMin = -2;
	private const int zoomLevelMax = 2;

	private List<EnemyS> stunnedEnemies;

	
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
		if (PlayerStatDisplayS.RECORD_MODE){
			startOrthoSize*=RECORD_MODE_ORTHO_MULT;
		}
		myCam.orthographicSize = startOrthoSize*ZOOM_LEVEL*orthoSizeMult;
		stunOrthoSize = startOrthoSize*STUN_ORTHO_MULT;

		poiQueue = new List<GameObject>();
		poiDelayTimes = new List<float>();

		if (GameObject.Find("Player") != null){
			playerRef = GameObject.Find("Player").GetComponent<PlayerController>();
		}

		stunnedEnemies = new List<EnemyS>();
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


		Vector3 camPos = poi.transform.position+_camPosOffset+offsetPos;

	
			_currentPos = transform.position;

		_currentPos.x = (1-_camEasing*followSpeedMultiplier)*_currentPos.x + followSpeedMultiplier*_camEasing*camPos.x;
		_currentPos.y = (1-_camEasing*followSpeedMultiplier)*_currentPos.y + followSpeedMultiplier*_camEasing*camPos.y;
	
	
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
				if (!slowerZoom){
					if (dialogueZoom){
						myCam.orthographicSize = (1-_camEasing)*myCam.orthographicSize + _camEasing*startOrthoSize*dialogueZoomMult*ZOOM_LEVEL*orthoSizeMult;
					}else{
						myCam.orthographicSize = (1-_camEasing)*myCam.orthographicSize + _camEasing*startOrthoSize*zoomMult*ZOOM_LEVEL*orthoSizeMult;
					}
				}else{
					if (dialogueZoom){
						myCam.orthographicSize = (1-_camEasing*slowZoomMult)*myCam.orthographicSize
							+ _camEasing*slowZoomMult*startOrthoSize*dialogueZoomMult*ZOOM_LEVEL*orthoSizeMult;
					}else{
						myCam.orthographicSize = (1-_camEasing*slowZoomMult)*myCam.orthographicSize
							+ _camEasing*slowZoomMult*startOrthoSize*zoomMult*ZOOM_LEVEL*orthoSizeMult;
					}
				}
			}else if (stunnedEnemies.Count > 0){
				myCam.orthographicSize = (1-_camEasing)*myCam.orthographicSize
					+ _camEasing*stunOrthoSize*ZOOM_LEVEL*orthoSizeMult;
			}
			else{
				if (!CameraShakeS.C.isSleeping){
					if (!slowChange){
					myCam.orthographicSize = (1-_camEasing)*myCam.orthographicSize*ZOOM_LEVEL + _camEasing*startOrthoSize*ZOOM_LEVEL*orthoSizeMult;
					}else{
						myCam.orthographicSize = (1-_camEasing*0.5f)*myCam.orthographicSize*ZOOM_LEVEL + _camEasing*0.5f*startOrthoSize*ZOOM_LEVEL*orthoSizeMult;
						slowChangeCount -= Time.deltaTime;
						if (slowChangeCount <= 0){
							slowChange = false;
						}
					}
				
	
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
					if (poiDelayTimes.Count == 1){
						delayMoveTime*=1.5f;
					}
				}else{
					if (overrideResetPoi){
						SetNewPOI(overrideResetPoi);
						playerRef.SetTalking(true);
						overrideResetPoi = null;
						queueOver = true;
					}else{
						ResetPOI(InGameCinematicS.inGameCinematic);
					}
				}

			}
		}

	}

	//__________________________________________________PUBLIC METHODS
	public void CutTo(Vector3 newPos){
		newPos.z = transform.position.z;
		transform.position = newPos;
	}
	public void PunchIn(){

		myCam.orthographicSize =  (startOrthoSize * punchInMult)*ZOOM_LEVEL*orthoSizeMult;

	}

	public void PunchInCustom(float punchMult, float punchHangTime){
		
		myCam.orthographicSize =  (startOrthoSize * punchMult)*ZOOM_LEVEL*orthoSizeMult;
		_punchHangTime = punchHangTime;
		
	}

	public void PunchInBig(){

		myCam.orthographicSize =  (startOrthoSize * punchInMultDeath)*ZOOM_LEVEL*orthoSizeMult;

	}

	public void SetZoomIn(bool zoom, bool slowZoom = false){
		zoomingIn = zoom;
		dialogueZoom = false;
		slowerZoom = slowZoom;
	}
	public void SetDialogueZoomIn(bool zoom){
		zoomingIn = zoom;
		dialogueZoom = zoom;
		slowerZoom = false;
	}
	public void EndZoom(){
		zoomingIn = dialogueZoom = slowerZoom = false;
	}

	public void PunchCombatEnd(Vector3 targetPos){


			CameraShakeS.C.TimeSleep(0.2f);
	
			Vector3 camPos = targetPos+_camPosOffset;
			
			_currentPos = transform.position;
			_currentPos.x = camPos.x;
			_currentPos.y = camPos.y;
		
			transform.position = _currentPos;

		myCam.orthographicSize =  startOrthoSize * punchInMultDeath * orthoSizeMult;



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

	public void SetNewPOI(GameObject newPoi, bool ignoreTalk = false){
		_poi = newPoi;
		if (playerRef && !ignoreTalk){
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

	public void ChangeOffset(Vector3 newOffset){
	}

	//_____________________________OPTIONS METHODS
	public static void ChangeZoomLevel(int dir){
		if (dir > 0){
			if (zoomInt < zoomLevelMax){
				zoomInt++;
				ZOOM_LEVEL = 1f+zoomInt*zoomLevelDiff;
			}
		}else{
			if (zoomInt > zoomLevelMin){
				zoomInt--;
				ZOOM_LEVEL = 1f+zoomInt*zoomLevelDiff;
			}
		}
	}

	public static void ResetZoomLevel(){
		zoomInt = 0;
		ZOOM_LEVEL = 1f;
	}

	public void AddStunnedEnemy(EnemyS newEnemy){
		if (!stunnedEnemies.Contains(newEnemy)){
			stunnedEnemies.Add(newEnemy);
		}
	}
	public void RemoveStunnedEnemy(EnemyS newEnemy){
		if (stunnedEnemies.Contains(newEnemy)){
			stunnedEnemies.Remove(newEnemy);
		}
	}
	public void ClearStunnedEnemies(){
		stunnedEnemies.Clear();
	}

	public void ChangeOrthoSizeMult(float newSize = 1f, float changeTime = 1f){
		orthoSizeMult = newSize;
		slowChangeCount = changeTime;
		if (slowChangeCount > 0){
		slowChange = true;
		}
	}

	public float GetFullQueueTime(){
		float qTime = 0;
		for (int i = 0; i < poiDelayTimes.Count; i++){
			qTime += poiDelayTimes[i];	
		}
		return qTime;
	}


}
