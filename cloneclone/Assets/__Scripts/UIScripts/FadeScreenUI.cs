using UnityEngine;
using System.Collections;

public class FadeScreenUI : MonoBehaviour {

	private bool _fadingOut = false;
	public bool fadingOut { get { return _fadingOut; } }
	
	private bool _fadingIn = false;
	public bool fadingIn { get { return _fadingIn; } }

	public float fadeRate = 1f;

	private SpriteRenderer _myRenderer;
	private Color _myColor;
	private Color _textColor;

	private string destinationScene = "HellScene";
	private int destinationSceneIndex = -1;
	AsyncOperation async;
	private bool startedLoading = false;

	public TextMesh loadingText;
	private string loadingString = "L O A D I N G   ";
	private string currentLoadingString = "";
	private float addLetterRate = 0.1f;
	private float addLetterCountdown;

	private DarknessPercentUIS darknessTracker;

	// Use this for initialization
	void Start () {

		_myRenderer = GetComponent<SpriteRenderer>();
		_myColor = _myRenderer.color; 
		_textColor = loadingText.color;
		_textColor.a = 0f;
		loadingText.color = _textColor;
		loadingText.text = loadingString;

		darknessTracker = GameObject.Find("In Game UI").GetComponentInChildren<DarknessPercentUIS>();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_fadingOut){
			
			_myColor = _myRenderer.color;
			_myColor.a -= fadeRate*Time.deltaTime;

			if (_myColor.a <= 0){
				_myColor.a = 0;
				_fadingOut = false;
			}
			_myRenderer.color = _myColor;
		}

		if (_fadingIn){
			
			_myColor = _myRenderer.color;
			_myColor.a += fadeRate*Time.deltaTime;
			
			if (_myColor.a >= 1){
				_myColor.a = 1;
				_fadingIn = false;
				StartLoading();
			}
			_myRenderer.color = _myColor;

			_textColor.a = _myColor.a;
			loadingText.color = _textColor;
		
		}

		if (startedLoading){

			/*addLetterCountdown -= Time.deltaTime;
			if (addLetterCountdown <= 0){
				addLetterCountdown = addLetterRate;
				if (currentLoadingString.Length >= loadingString.Length){
					currentLoadingString = "";
				}else{
					currentLoadingString += loadingString[currentLoadingString.Length];
				}
				loadingText.text = currentLoadingString;
			}*/

			if (_myRenderer.color.a >= 1f && async.progress >= 0.9f && darknessTracker.allowAdvance){	
				if (destinationScene == GameOverS.reviveScene){
					if (PlayerInventoryS.I != null){
						// this is reviving from game over, reset inventory
						PlayerInventoryS.I.RefreshRechargeables();
					}
				}
				InGameCinematicS.inGameCinematic = false;
				async.allowSceneActivation = true;
			}
		}
	
	}

	public void FadeOut(float newRate = 0){

		if (_myRenderer == null){
			_myRenderer = GetComponent<SpriteRenderer>();
			_myColor = _myRenderer.color; 
		}

		if (newRate > 0){
			fadeRate = newRate;
		}

		_myColor = _myRenderer.color; 
		_myColor.a = 1;
		_myRenderer.color = _myColor;

		_fadingOut = true;

	}
	public void ChangeColor(Color newCol){
		Color changeCol = newCol;
		changeCol.a = _myRenderer.color.a;
		_myRenderer.color = changeCol;
	}

	public void FadeIn(string nextScene, float newRate = 0){

		if (nextScene != ""){
			destinationScene = nextScene;
		}
		
		if (_myRenderer == null){
			_myRenderer = GetComponent<SpriteRenderer>();
			_myColor = _myRenderer.color; 
		}
		
		if (newRate > 0){
			fadeRate = newRate;
		}
		
		_myColor = _myRenderer.color; 
		_myColor.a = 0;
		_myRenderer.color = _myColor;
		
		_fadingIn = true;

	}

	public void SetNewDestination(string newScene){
		destinationScene = newScene;
	}
	public void SetNewDestination(int newSceneIndex){
		destinationSceneIndex = newSceneIndex;
	}

	private void StartLoading(){
		StartCoroutine(LoadNextScene());
		startedLoading = true;
	}

	private IEnumerator LoadNextScene(){
		if (destinationSceneIndex > -1){
			async = Application.LoadLevelAsync(destinationSceneIndex);
		}else{
			async = Application.LoadLevelAsync(destinationScene);
		}
		async.allowSceneActivation = false;
		yield return async;
	}
}
