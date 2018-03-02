using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class MainMenuNavigationS : MonoBehaviour {

	private const string currentVer = "— v. 0.7.0 —";
	private static bool hasSeenMainMenu = false;

	[Header("Demo Properties")]
	public bool publicDemoVersion = false;
	public string demoOneScene;
	public string demoTwoScene;
	
	private ControlManagerS myController;

	[Header("Instance Properties")]
	public Text versionText;
	public float allowStartTime = 3f;
	private bool started = false;
	private bool quitting = false;
	
	public SpriteRenderer fadeOnZoom;
	public float fadeTime = 1f;
	public float zoomInRate = 3f;
	private float minZoom = 0.3f;
	
	public GameObject[] textTurnOff;
	public GameObject firstScreenTurnOff;
	private bool onNewScreen = false;
	
	public GameObject secondScreenObject;

	private BlurOptimized blurEffect;
	private float blurEffectTimeMax = 5f;
	private float blurEffectTime;
	private float blurT;
	private bool blurEnabled = true;
	private float blurSizeMax = 9.99f;
	
	private Camera myCam;
	private float startOrtho;
	
	public GameObject secondScreenIntro;
	public GameObject secondScreenLoop;
	
	public Transform[] menuSelections;
	public TextMesh[] menuSelectionsText;
	public TextMesh[] controlTexts;
	private int currentSelection = 0;
	public GameObject selectOrb;
	public GameObject credits;
	public Color continueDisableColor;

	public Transform[] newGameSelections;
	public TextMesh[] newGameOverrideText;
	private bool selectingOverride = false;
	public GameObject showOnOverride;
	public GameObject hideOnOverride;
	
	private Vector3 selectionScale;
	private Color selectionStartColor;
	
	private bool stickReset = false;
	private bool selectReset = false;
	
	public SpriteRenderer loadBlackScreen;
	private bool loading = false;
	
	private string newGameScene = "TutorialIntro"; // (put back in when making full version)
	//private string newGameScene = "Dream00a_IntroCutsceneSHORT";
	private string webStartScene = "Dream00a_IntroCutsceneSHORT";
	//private string newGameScene = "InfiniteScene";
	private string twitterLink = "http://www.twitter.com/melessthanthree";
	private string twitterLinkII = "http://twitter.com/NicoloDTelesca";
	private string facebookLink = "http://www.facebook.com/lucahgame/";

	private bool attractEnabled = true;
	private string attractScene = "AttractMode_00";
	private float attractCountdownMax = 30f;
	private float attractCountdown;
	
	private string cheatString = "";
	private bool allowCheats = false; // TURN OFF FOR DEMO

	public static bool inMain = false;
	
	private bool startedLoading;
	AsyncOperation async;

	private bool canContinue = false;
	
	public InfiniteBGM startMusic;

	void Awake(){
		versionText.text = currentVer;
		inMain = true;

		#if UNITY_WEBGL
		attractEnabled = false;
		menuSelections[menuSelections.Length-1].gameObject.SetActive(false);
		newGameScene = webStartScene;
		#endif
	}
	
	// Use this for initialization
	void Start () {

		PlayerStatsS.godMode = false;
		OverrideDemoLoadS.beenToMainMenu = true;

		blurEffect = GetComponent<BlurOptimized>();
		
		fadeOnZoom.gameObject.SetActive(false);
		firstScreenTurnOff.SetActive(true);

		attractCountdown = attractCountdownMax;
		
		foreach (GameObject t in textTurnOff){
			t.SetActive(true);
		}
		
		myController = GetComponent<ControlManagerS>();
		if (!hasSeenMainMenu){
			if (myController.ControllerAttached()){
				if (myController.DetermineControllerType() == 1){
				ControlManagerS.controlProfile = 3;
				}else{
					ControlManagerS.controlProfile = 0;
				}
				Cursor.visible = false;
			}else{
				ControlManagerS.controlProfile = 1;
				Cursor.visible = true;
			}
		}else{
			if (ControlManagerS.controlProfile == 1){
				Cursor.visible = true;
			}else{
				Cursor.visible = false;
			}
		}
		SetControlSelection();

		myCam = GetComponent<Camera>();
		startOrtho = myCam.orthographicSize;
		
		secondScreenObject.gameObject.SetActive(false);
		secondScreenLoop.SetActive(false);
		loadBlackScreen.gameObject.SetActive(false);
		selectOrb.SetActive(false);
		
		selectionScale = menuSelections[0].localScale;
		selectionStartColor = menuSelectionsText[0].color;
		
		startMusic.FadeIn();

		if (SaveLoadS.SaveFileExists()){
			if (!publicDemoVersion){
			currentSelection = 1;
			}
			canContinue = true;
		}else{
			continueDisableColor.a = 0f;
			menuSelectionsText[1].color = continueDisableColor;
		}

		hasSeenMainMenu = true;
		
	}
	
	// Update is called once per frame
	void Update () {
		CheckCheats();

		if (!startedLoading){

			if (attractEnabled){
				attractCountdown -= Time.deltaTime;
				//Debug.Log(attractCountdown);
				if (attractCountdown <= 0){
					loading = true;
					newGameScene = attractScene;
					startMusic.FadeOut();
					loadBlackScreen.gameObject.SetActive(true);
					loading = true;
					selectOrb.SetActive(false);
					Cursor.visible = false;
					hideOnOverride.gameObject.SetActive(false);
					showOnOverride.gameObject.SetActive(false);
					StartNextLoad();
				}
			}
		}

		if (!started && !loading){

			allowStartTime -= Time.deltaTime;

			HandleBlur();

			if (allowStartTime <= 0 && (myController.GetCustomInput(3) || Input.GetKeyDown(KeyCode.Return))){
				started = true;
				selectReset = false;
				CancelBlur();
				attractCountdown = attractCountdownMax;
				foreach (GameObject t in textTurnOff){
					t.SetActive(false);
				}
				fadeOnZoom.gameObject.SetActive(true);
			}
		}else if (!onNewScreen && !loading){
			if (myCam.orthographicSize > minZoom){
				myCam.orthographicSize -= Time.deltaTime*zoomInRate;
			}else{
				myCam.orthographicSize = minZoom;
			}
			if (fadeOnZoom.color.a >= 1f){
				onNewScreen = true;
				myCam.orthographicSize = startOrtho;
				firstScreenTurnOff.SetActive(false);
				secondScreenObject.SetActive(true);
				fadeOnZoom.gameObject.SetActive(false);
			}
		}else if (!loading){
			if (!secondScreenLoop.activeSelf){
				if (secondScreenIntro == null){
					secondScreenLoop.SetActive(true);
					SetSelection();
					selectOrb.SetActive(true);
					credits.SetActive(true);
				}
			}else{

				if (!Input.GetKeyDown(KeyCode.Return) && !myController.GetCustomInput(3) && !Input.GetKeyDown(KeyCode.Q) && !Input.GetKeyDown(KeyCode.Backspace)
					&& !Input.GetKeyDown(KeyCode.Delete) && !Input.GetKeyDown(KeyCode.Escape) && !myController.GetCustomInput(1) && !Input.GetKeyDown(KeyCode.E)){
					selectReset = true;
				}

				if (Mathf.Abs(myController.VerticalMenu()) < 0.05f && Mathf.Abs(myController.HorizontalMenu()) < 0.05f){
					stickReset = true;
				}

				if (selectingOverride){

					if (selectReset && ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Backspace)
						|| Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Escape) || myController.GetCustomInput(1)))){
						selectReset = false;
						selectingOverride = false;
						currentSelection = 0;
						showOnOverride.gameObject.SetActive(false);
						hideOnOverride.gameObject.SetActive(true);
						SetSelection();
						attractCountdown = attractCountdownMax;
					}

					if (selectReset && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E) || myController.GetCustomInput(3))){
						attractCountdown = attractCountdownMax;
						if (currentSelection == 0){
							startMusic.FadeOut();
							loadBlackScreen.gameObject.SetActive(true);
							loading = true;
							selectOrb.SetActive(false);
							Cursor.visible = false;
							hideOnOverride.gameObject.SetActive(false);
							showOnOverride.gameObject.SetActive(false);
							selectReset = false;
							StoryProgressionS.NewGame(); // reset for new game progress
							
							PlayerStatsS.healOnStart = true;
							StartNextLoad();
						}else{
							selectReset = false;
							selectingOverride = false;
							currentSelection = 0;
							showOnOverride.gameObject.SetActive(false);
							hideOnOverride.gameObject.SetActive(true);
							SetSelection();
						}
					}
						
					if (myController.HorizontalMenu() > 0.1f && stickReset){
						attractCountdown = attractCountdownMax;
						currentSelection++;
						if (currentSelection > 1){
							currentSelection = 0;
						}
						stickReset = false;
						SetSelection();
					}
					if (myController.HorizontalMenu() < -0.1f && stickReset){
						attractCountdown = attractCountdownMax;
						currentSelection--;
						if (currentSelection < 0){
							currentSelection = 1;
						}
						stickReset = false;
						SetSelection();
					}
				}else{

				// go down
					if (myController.VerticalMenu() < -0.1f && stickReset){
						attractCountdown = attractCountdownMax;
					
					stickReset = false;
					
					currentSelection ++;
						#if UNITY_WEBGL
						if (currentSelection == 1 && !canContinue){
							currentSelection = 2;
						}
						if (currentSelection > menuSelections.Length-2){
							currentSelection = 0;
						}
						#else
						if (!publicDemoVersion){
					if (currentSelection == 1 && !canContinue){
						currentSelection = 2;
					}
					if (currentSelection > menuSelections.Length-1){
						currentSelection = 0;
					}
						}else{
							if (currentSelection > 1){
								currentSelection = 0;
							}
						}
						#endif
					
					SetSelection();
				}
				
				// go up
					if (myController.VerticalMenu() > 0.1f && stickReset){
						attractCountdown = attractCountdownMax;
						
					stickReset = false;
					
					currentSelection --;
						#if UNITY_WEBGL
						if (currentSelection == 1 && !canContinue){
							currentSelection = 0;
						}
						if (currentSelection < 0){
							currentSelection = menuSelections.Length-2;
						}
						#else
						if (!publicDemoVersion){
					if (currentSelection == 1 && !canContinue){
						currentSelection = 0;
					}
					if (currentSelection < 0){
						currentSelection = menuSelections.Length-1;
					}
						}else{
							if (currentSelection < 0){
								currentSelection = 1;
							}
						}
						#endif
					SetSelection();
				}

				// control settings
					if (currentSelection == 2){
						if ((myController.HorizontalMenu() > 0.1f && stickReset) || 
							(selectReset && (myController.GetCustomInput(3) || Input.GetKeyDown(KeyCode.KeypadEnter) 
								|| Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E)))){
							attractCountdown = attractCountdownMax;
						myController.ChangeControlProfile(1);
						SetControlSelection();
						stickReset = false;
							selectReset = false;
					}
						if (myController.HorizontalMenu() < -0.1f && stickReset){
							attractCountdown = attractCountdownMax;
						myController.ChangeControlProfile(-1);
						SetControlSelection();
						stickReset = false;
					}
				}
				
					if (selectReset && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E) || myController.GetCustomInput(3)) 
						&& !loading && !quitting){
						attractCountdown = attractCountdownMax;
						if (currentSelection == 1 || (currentSelection == 0 && (!canContinue || publicDemoVersion))){
						startMusic.FadeOut();
						loadBlackScreen.gameObject.SetActive(true);
						loading = true;
							selectReset = false;
						selectOrb.SetActive(false);
						Cursor.visible = false;
							hideOnOverride.gameObject.SetActive(false);
							showOnOverride.gameObject.SetActive(false);
							if (currentSelection == 0 || publicDemoVersion){
							StoryProgressionS.NewGame(); // reset for new game progress
								/*if (currentSelection == 0){
									newGameScene = demoOneScene;
								}else{
									newGameScene = demoTwoScene;
								}**/
						}else{
							SaveLoadS.Load();
							newGameScene = GameOverS.reviveScene;
							PlayerController.doWakeUp = true;
						}
						PlayerStatsS.healOnStart = true;
						StartNextLoad();
					}
						else if (currentSelection == 0 && canContinue && !publicDemoVersion){
							selectingOverride = true;
							currentSelection = 1;
							selectReset = false;
							showOnOverride.gameObject.SetActive(true);
							hideOnOverride.gameObject.SetActive(false);
							SetSelection();
						}
						else if (currentSelection == 3 && !publicDemoVersion){
							attractCountdown = attractCountdownMax;
							Application.Quit();
							quitting = true;
						}
					}
				}
			}
		}else{
			if (loadBlackScreen.color.a >= 1f){
				if (async.progress >= 0.9f){
					//inMain = false;
					async.allowSceneActivation = true;
				}
			}
		}
		
	}
	
	void SetSelection(){
		
		Color correctCol = Color.white;
		if (selectingOverride){
			for (int i = 0; i < newGameSelections.Length; i++){
				if (i == currentSelection){
					//menuSelections[i].localScale = selectionScale*1.2f;
					selectOrb.transform.position = newGameSelections[i].position;
					correctCol = Color.white;
					correctCol.a = newGameOverrideText[i].color.a;
					newGameOverrideText[i].color = correctCol;;
				}else{
						correctCol = selectionStartColor;
					correctCol.a = newGameOverrideText[i].color.a;
					newGameOverrideText[i].color = correctCol;
				}
			}
		}else{
			for (int i = 0; i < menuSelections.Length; i++){
				if (i == currentSelection){
					//menuSelections[i].localScale = selectionScale*1.2f;
					selectOrb.transform.position = menuSelections[i].position;
					if (i == 1 && !canContinue){
						correctCol = continueDisableColor;
						correctCol.a = menuSelectionsText[i].color.a;
						menuSelectionsText[i].color = correctCol;
					}else{
						correctCol = Color.white;
						correctCol.a = menuSelectionsText[i].color.a;
						menuSelectionsText[i].color = correctCol;
					}
				}else{
					//menuSelections[i].localScale = selectionScale;
					if (i == 1 && !canContinue){
						correctCol = continueDisableColor;
						correctCol.a = menuSelectionsText[i].color.a;
						menuSelectionsText[i].color = correctCol;
					}else{
						correctCol = selectionStartColor;
						correctCol.a = menuSelectionsText[i].color.a;
						menuSelectionsText[i].color = correctCol;
					}
				}
			}
		}
		
	}

	void SetControlSelection(){

		string controlType = "Gamepad";

		Cursor.visible = false;

		if (ControlManagerS.controlProfile == 1){
			controlType = "Keyboard & Mouse";

			Cursor.visible = true;
		}
		if (ControlManagerS.controlProfile == 2){
			controlType = "Keyboard (No Mouse)";

			Cursor.visible = false;
		}
		if (ControlManagerS.controlProfile == 3){
			controlType = "Gamepad (PS4)";

			Cursor.visible = false;
		}
		for (int i = 0; i < controlTexts.Length; i++){
			controlTexts[i].text = controlType;
		}

	}
	
	private void CheckCheats(){
		
		if (Input.GetKeyDown(KeyCode.Escape) && selectReset){
			//Application.Quit();
			currentSelection = 3;
			SetSelection();
		}
		
		if (allowCheats){
			if (Input.GetKeyDown(KeyCode.G)){
				cheatString += "G";
				if (cheatString == "GGGG"){
					PlayerStatsS.godMode = true;
					Debug.Log("god mode on");
				}
			}
			
			if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)){
				cheatString = "";
				PlayerStatsS.godMode = false;
				Debug.Log("god mode off");
			}
		}
		
	}
	
	private void StartNextLoad(){
		StartCoroutine(LoadNextScene());
		startedLoading = true;
	}
	
	private IEnumerator LoadNextScene(){
		async = Application.LoadLevelAsync(newGameScene);
		async.allowSceneActivation = false;
		yield return async;
	}

	void HandleBlur(){
		if (blurEffect.enabled){
			if (blurEffectTime < blurEffectTimeMax){
				blurEffectTime += Time.deltaTime;
				blurT = blurEffectTime / blurEffectTimeMax;
				//blurT = Mathf.Sin(blurT * Mathf.PI * 0.5f);
				blurT = 1f - Mathf.Cos(blurT * Mathf.PI * 0.5f);
				blurEffect.blurSize = Mathf.Lerp(blurSizeMax, 0, blurT);
				blurEffect.blurIterations = Mathf.FloorToInt(Mathf.Lerp(4, 0, blurT));
				blurEffect.downsample = Mathf.FloorToInt(Mathf.Lerp(2, 0, blurT));



			}else{
				blurEffect.enabled = false;
			}
		}
	}

	void CancelBlur(){
		blurEffect.enabled = false;
	}
}
