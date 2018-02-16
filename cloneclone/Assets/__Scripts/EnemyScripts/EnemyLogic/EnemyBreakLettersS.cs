using UnityEngine;
using System.Collections;

public class EnemyBreakLettersS : MonoBehaviour {

	private bool _initialized = false;
	public bool initialized { get { return _initialized; } }

	public float lerpTime = 0.25f;
	private float currentLerpTime;
	private int currentStage = 0; // 0 = lerp and fade, 1 = static, 2 = flickering, 3 = off
	public float showTime = 1.25f;
	private float currentShowTime;
	public float flickerOfftime = 0.08f;
	public float flickerOnTime = 0.1f;
	private float currentFlickerTime = 0;
	public int numFlickersMax = 5;
	private int currentNumFlickers = 0;

	private float lerpT = 0f;

	private TextMesh _myMesh;
	private Color myCol;
	private Color startFadeCol;

	private Vector3 targetPos;
	private Vector3 startPos;
	public float lerpDistance = 0.3f;

	private string currentLetter = "";

	
	// Update is called once per frame
	void Update () {

		if (currentStage == 0){
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime >= lerpTime){
				currentLerpTime = lerpTime;
				currentStage++;
			}
			lerpT = currentLerpTime/lerpTime;
			lerpT = Mathf.Sin(lerpT * Mathf.PI * 0.5f);
			transform.localPosition = Vector3.Lerp(startPos, targetPos, lerpT);
			_myMesh.color = Color.Lerp(startFadeCol, myCol, lerpT);

		}else if (currentStage == 1){

			currentShowTime += Time.deltaTime;
			if (currentShowTime >= showTime){
				currentStage++;
				_myMesh.text = "";
			}
			
		}else if (currentStage == 2){
			currentFlickerTime += Time.deltaTime;
			if (_myMesh.text == ""){
				if (currentFlickerTime >= flickerOfftime){
					currentNumFlickers++;
					_myMesh.text = currentLetter;
					currentFlickerTime = 0f;
				}
			}else{
				if (currentFlickerTime >= flickerOnTime){
					currentNumFlickers++;
					_myMesh.text = "";
					currentFlickerTime = 0f;
				}
			}
			if (currentNumFlickers >= numFlickersMax){
				currentStage++;
			}
		}else{
			gameObject.SetActive(false);
		}
	
	}

	public void Activate(string newLetter){
		if (!_initialized){
			_myMesh = GetComponent<TextMesh>();
			myCol = _myMesh.color;
			startFadeCol = myCol;
			startFadeCol.a = 0f;
			targetPos = transform.localPosition;
			startPos = targetPos;
			startPos.y -= lerpDistance;
		}
		_myMesh.text = currentLetter = newLetter;
		currentStage = currentNumFlickers = 0;
		_myMesh.color = startFadeCol;
		transform.localPosition = startPos;
		currentLerpTime = currentShowTime = currentFlickerTime = 0f;

		gameObject.SetActive(true);
	}
}
