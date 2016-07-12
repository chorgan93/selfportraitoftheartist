using UnityEngine;
using System.Collections;

public class CameraFollowS : MonoBehaviour {

	//_________________________________________________CLASS PROPERTIES

	private float _camEasing = 0.1f;
	private Vector3 _camPosOffset = new Vector3(0,0,-10);

	private Vector3 _currentPos;

	//__________________________________________________INSTANCE PROPERTIES

	private GameObject _poi;

	private float startOrthoSize;
	private float focusMult = 0.95f;
	private float punchInMult = 0.9f;
	private float punchInMultDeath = 0.5f;
	private Camera myCam;
	
	//_________________________________________________GETTERS AND SETTERS
	
	public float camEasing	{ get { return _camEasing; } }
	public GameObject poi	{ get { return _poi; } }
	public Vector3	currentPos {get { return _currentPos; }}

	//__________________________________________________UNITY METHODS

	// Use this for initialization
	void Start () {

		_poi = GameObject.Find("CameraPOI");

		myCam = GetComponent<Camera>();
		startOrthoSize = myCam.orthographicSize;


			
			Vector3 camPos = poi.transform.position+_camPosOffset;
			_currentPos = transform.position;
			transform.position = camPos;


	
	}
	
	// Update is called once per frame
	void Update(){

		if (Input.GetKeyDown(KeyCode.R)){
			Application.LoadLevel(Application.loadedLevel);
		}

	}

	void FixedUpdate () {

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
			
			transform.position = _currentPos;

		



		if (!CameraShakeS.C.isSleeping){

				myCam.orthographicSize = (1-_camEasing)*myCam.orthographicSize + _camEasing*startOrthoSize*focusMult;
			

		}

	}

	//__________________________________________________PUBLIC METHODS
	public void PunchIn(){

		myCam.orthographicSize =  startOrthoSize * punchInMult;

	}

	public void PunchInBig(){

		myCam.orthographicSize =  startOrthoSize * punchInMultDeath;

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


}
