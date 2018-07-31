using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

	private Vector3 loadingTextPos;
	private float loadingTextSize;

	public static bool dontAllowReset = false;

	private DarknessPercentUIS darknessTracker;
	public static bool NoFade = false;
    public static bool PostDarkScene = false;

	private float _delayFadeIn = 0f;
	private bool _delayWakeUp = false;
	private bool _triggeredWakeUp = false;
	private PlayerController pRef;

	private Image uiFadeAssist;
	private Color fadeAssistColor;
	private bool doNotUseAssist = false;

    [HideInInspector]
    public bool skipPercentScene = false;

    public bool neverGoToDarkness = false;

	// Use this for initialization
	void Start () {

		_myRenderer = GetComponent<SpriteRenderer>();
		_myColor = _myRenderer.color; 
		_textColor = loadingText.color;
		_textColor.a = 0f;
		loadingText.color = _textColor;
		loadingText.text = loadingString;
		loadingTextPos = loadingText.transform.position;
		loadingTextSize = loadingText.fontSize;

		if (NoFade){
			_myColor.a = 0f;
			_myRenderer.color = _myColor;
			_fadingOut = false;
			NoFade = false;
		}

		if (GameObject.Find("In Game UI")){
			darknessTracker = GameObject.Find("In Game UI").GetComponentInChildren<DarknessPercentUIS>();
			uiFadeAssist = GameObject.Find("FadeUIBlack").GetComponent<Image>();
			fadeAssistColor = uiFadeAssist.color;
			fadeAssistColor.a = _myColor.a;
			uiFadeAssist.color = fadeAssistColor;
		}

		if (_delayWakeUp){
			pRef = GameObject.Find("Player").GetComponent<PlayerController>();
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_fadingOut){

			if (_delayFadeIn > 0){
				_delayFadeIn -= Time.deltaTime;
			}else{
				if (_delayWakeUp && !_triggeredWakeUp){
					pRef.TriggerWakeUp();
					_triggeredWakeUp = true;
				}


				if (!doNotUseAssist && uiFadeAssist != null){
					fadeAssistColor.a = _myColor.a;
					uiFadeAssist.color = fadeAssistColor;
				}
			
				_myColor = _myRenderer.color;
				_myColor.a -= fadeRate*Time.deltaTime;
	
				if (_myColor.a <= 0){
					_myColor.a = 0;
					_fadingOut = false;
				}
				_myRenderer.color = _myColor;
			}
		}

		if (_fadingIn){
			
			_myColor = _myRenderer.color;
			_myColor.a += fadeRate*Time.deltaTime;
			
			if (_myColor.a >= 1){
				_myColor.a = 1;
				_fadingIn = false;
				if (!RetryFightUI.allowRetry){
					StartLoading();
                }else if (skipPercentScene){
                    darknessTracker.allowRetryUI.TurnOn();
                }
			}
			_myRenderer.color = _myColor;

			if (!doNotUseAssist && uiFadeAssist != null){
				fadeAssistColor.a = _myColor.a;
				uiFadeAssist.color = fadeAssistColor;
			}

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

			if (RankManagerS.R != null){
			if (darknessTracker){
				if (_myRenderer.color.a >= 1f && async.progress >= 0.9f && darknessTracker.allowAdvance && !RankManagerS.R.delayLoad){	
						if (destinationScene == GameOverS.reviveScene || destinationScene == GameOverS.tempReviveScene){
					if (PlayerInventoryS.I != null){
						// this is reviving from game over, reset inventory
						PlayerInventoryS.I.RefreshRechargeables();
					}
				}
				InGameCinematicS.inGameCinematic = false;
				dontAllowReset = false;
				async.allowSceneActivation = true;
			}
			}else{
					//Debug.Log(async.progress);
				if (_myRenderer.color.a >= 1f && async.progress >= 0.9f && !RankManagerS.R.delayLoad){	
						if (destinationScene == GameOverS.reviveScene || destinationScene == GameOverS.tempReviveScene){
						if (PlayerInventoryS.I != null){
							// this is reviving from game over, reset inventory
							PlayerInventoryS.I.RefreshRechargeables();
						}
					}
					InGameCinematicS.inGameCinematic = false;
					dontAllowReset = false;
					async.allowSceneActivation = true;
				}
			}
			}else{
				if (darknessTracker){
				if (_myRenderer.color.a >= 1f && async.progress >= 0.9f && darknessTracker.allowAdvance){	
						if (destinationScene == GameOverS.reviveScene || destinationScene == GameOverS.tempReviveScene){
						if (PlayerInventoryS.I != null){
							// this is reviving from game over, reset inventory
							PlayerInventoryS.I.RefreshRechargeables();
						}
					}
					InGameCinematicS.inGameCinematic = false;
					dontAllowReset = false;
					async.allowSceneActivation = true;
				}
			}else{
				if (_myRenderer.color.a >= 1f && async.progress >= 0.9f){	
						if (destinationScene == GameOverS.reviveScene || destinationScene == GameOverS.tempReviveScene){
						if (PlayerInventoryS.I != null){
							// this is reviving from game over, reset inventory
							PlayerInventoryS.I.RefreshRechargeables();
						}
					}
					InGameCinematicS.inGameCinematic = false;
					dontAllowReset = false;
					async.allowSceneActivation = true;
				}
			}
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
		doNotUseAssist = true;
	}

	public void FadeIn(string nextScene, float newRate = 0){

        if (DarknessPercentUIS.DPERCENT.pStatRef.currentDarkness >= 100f && !DarknessPercentUIS.demoMode && !PostDarkScene
            && !DarknessPercentUIS.DPERCENT.pStatRef.pRef.isNatalie && !neverGoToDarkness){
			BGMHolderS.BG.FadeOutAll();
			GameOverS.tempReviveScene = destinationScene;
			GameOverS.tempRevivePosition = SpawnPosManager.whereToSpawn;
			nextScene = DarknessPercentUIS.DPERCENT.Return100Scene();
			if (!DarknessPercentUIS.DPERCENT.pStatRef.PlayerIsDead()){
				DarknessPercentUIS.DPERCENT.ActivateDeathCountUp();
			}
		}
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
		DeathCountdownS.DC.FadeOutCountdown();

	}

	public void SetNewDestination(string newScene){
		destinationScene = newScene;
	}
	public void SetNewDestination(int newSceneIndex){
		destinationSceneIndex = newSceneIndex;
	}

	public void StartLoading(){
		StartCoroutine(LoadNextScene());
		dontAllowReset = true;
		startedLoading = true;
		SetText();
	}

	void SetText(){
		loadingText.fontSize = Mathf.RoundToInt(loadingTextSize*CameraFollowS.ZOOM_LEVEL);
		loadingTextPos.x*=CameraFollowS.ZOOM_LEVEL;
		loadingTextPos.y*=CameraFollowS.ZOOM_LEVEL;
		loadingText.transform.position = loadingTextPos;
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

	public void ChangeFadeTime(float delayFadeTime, bool delayWakeUp){
		_delayWakeUp = delayWakeUp;
		_delayFadeIn = delayFadeTime;
	}
}
