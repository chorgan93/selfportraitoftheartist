using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class MainMenuNavigationS : MonoBehaviour {

	private bool ALLOW_RECORD_MODE = false; // TODO COLIN TURN OFF FOR FINAL BUILDS!!

	private const string currentVer = "— v. 1.0.5 —";
	private static bool hasSeenMainMenu = false;

	[Header("Demo Properties")]
	public bool publicDemoVersion = false;
	public string demoOneScene;
	public string demoTwoScene;
	
	private ControlManagerS myController;
    public ControlManagerS controlRef { get { return myController; } }

	[Header("Instance Properties")]
	public Text versionText;
	public float allowStartTime = 3f;
	private bool started = false;
	private bool quitting = false;

    private bool waitingForInput = true;
    [HideInInspector]
    public bool triggerSecondScreen = false;
	
	public SpriteRenderer fadeOnZoom;
	public float fadeTime = 1f;
	public float zoomInRate = 3f;
	private float minZoom = 0.3f;

    [Header("MENU ZERO STUFF")]
    public SpriteRenderer cursorForMenuZero;
    public TextMesh[] cursorMenuZeroPositions;
    private int currentMenuZeroPosition = 0;
    public GameObject menuZeroObject;

    public TextMesh[] pressAnyText;
	public GameObject[] textTurnOff;
	public GameObject firstScreenTurnOff;
	private bool onNewScreen = false;
	
	public GameObject secondScreenObject;

	private Blur blurEffect;
	private float blurEffectTimeMax = 5f;
	private float blurEffectTime;
	private float blurT;
	private bool blurEnabled = true;
	
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

    private string startNewGameScene = "TutorialIntro";
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
	private bool allowCheats = false; // COLIN TODO TURN OFF FOR FINAL BUILDS!

	public static bool inMain = false;
	
	private bool startedLoading;
	AsyncOperation async;

	private bool canContinue = false;
	
	public InfiniteBGM startMusic;

	[Header("Sound Properties")]
	public GameObject newGameSound;

    private int lastUsedFile = 0;
    private int numSaveFiles = 0;
    public int NumSaveFiles { get { return numSaveFiles; }}
    private int saveToLoad;

    [Header("Options Properties")]
    public GameMenuS optionsMenu;
    public LoadFileMenu loadFileMenu;
    [HideInInspector]
    public bool inLoadMenu = false;
    private bool inOptionsMenu = false;
    public GameObject[] additionalInstructions;

	void Awake(){
		versionText.text = currentVer;
		inMain = true;

#if UNITY_WEBGL
		attractEnabled = false;
		menuSelections[menuSelections.Length-1].gameObject.SetActive(false);
		newGameScene = webStartScene;
#endif

        // load basic info
        numSaveFiles = SaveLoadS.NumSavesOnDisk();;
        lastUsedFile = SaveLoadS.LastUsedSave();
        saveToLoad = lastUsedFile;
	}
	
	// Use this for initialization
	void Start () {

		PlayerStatsS.godMode = false;
		OverrideDemoLoadS.beenToMainMenu = true;

		blurEffect = GetComponent<Blur>();
		
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
        for (int i = 0; i < pressAnyText.Length; i++){
            if (myController.ControllerAttached())
            {
                pressAnyText[i].text = "Press Any Button";
        
            }else{
                pressAnyText[i].text = "Press Any Key";
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

            currentSelection = 0;
		if (SaveLoadS.SaveFileExists()){
            //canContinue = true;
            currentMenuZeroPosition = 0;
		}else{
            currentMenuZeroPosition = 1;
			continueDisableColor.a = 0f;
            cursorMenuZeroPositions[0].color = continueDisableColor;
		}
        // force cancontinue true for now
        canContinue = true;
		hasSeenMainMenu = true;
		
	}
	
	// Update is called once per frame
	void Update () {
		CheckCheats();

		if (!startedLoading){

            if (attractEnabled && !inOptionsMenu && !inLoadMenu){
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

            if (allowStartTime <= 0)
            {
                if (waitingForInput)
                {
                    // show "press any" message


                    if (myController.ControllerAttached())
                    {
                        if (myController.CheckForButtonPress(true) > -1)
                        {
                            waitingForInput = false;
                            menuZeroObject.SetActive(true);
                            stickReset = false;
                            selectReset = false;
                            if (myController.DetermineControllerType() == 1)
                            {
                                ControlManagerS.controlProfile = 3;
                            }
                            else
                            {
                                ControlManagerS.controlProfile = 0;
                            }
                            if (SaveLoadS.SaveFileExists())
                            {
                                currentMenuZeroPosition = 0;
                            }
                            else
                            {
                                currentMenuZeroPosition = 1;
                            }
                            for (int i = 0; i < pressAnyText.Length; i++)
                            {
                                pressAnyText[i].gameObject.SetActive(false);
                            }
                            cursorForMenuZero.transform.position = cursorMenuZeroPositions[currentMenuZeroPosition].transform.position;
                            SetAdditionalInstruction(true);

                        }else if (myController.CheckForKeyPress(true) > -1)
                            {
                                waitingForInput = false;
                                menuZeroObject.SetActive(true);
                                ControlManagerS.controlProfile = 1;
                                stickReset = false;
                                selectReset = false;
                                for (int i = 0; i < pressAnyText.Length; i++)
                                {
                                    pressAnyText[i].gameObject.SetActive(false);
                                }
                                if (SaveLoadS.SaveFileExists())
                                {
                                    currentMenuZeroPosition = 0;
                                }
                                else
                                {
                                    currentMenuZeroPosition = 1;
                                }
                            cursorForMenuZero.transform.position = cursorMenuZeroPositions[currentMenuZeroPosition].transform.position;
                            SetAdditionalInstruction(true);
                            }

                    }
                    else if (myController.CheckForKeyPress(true) > -1)
                    {
                        waitingForInput = false;
                        menuZeroObject.SetActive(true);
                        ControlManagerS.controlProfile = 1;

                        stickReset = false;
                        selectReset = false;
                        for (int i = 0; i < pressAnyText.Length; i++)
                        {
                            pressAnyText[i].gameObject.SetActive(false);
                        }
                        if (SaveLoadS.SaveFileExists())
                        {
                            currentMenuZeroPosition = 0;
                        }
                        else
                        {
                            currentMenuZeroPosition = 1;
                        }
                        cursorForMenuZero.transform.position = cursorMenuZeroPositions[currentMenuZeroPosition].transform.position;
                        SetAdditionalInstruction(true);
                    }
                }
                else if (triggerSecondScreen)
                {
                    // load and go
                    started = true;
                    selectReset = false;
                    CancelBlur();
                    attractCountdown = attractCountdownMax;
                    foreach (GameObject t in textTurnOff)
                    {
                        t.SetActive(false);
                    }
                    fadeOnZoom.gameObject.SetActive(true);
                }
                else
                {
                    if (!inLoadMenu)
                    {
                        // show rpgmaker-type menu
                        if (!stickReset)
                        {
                            if (Mathf.Abs(myController.HorizontalMenu()) < 0.1f && Mathf.Abs(myController.VerticalMenu()) < 0.1f)
                            {
                                stickReset = true;
                            }
                        }
                        else
                        {
                            if (myController.VerticalMenu() < -0.1f)
                            {
                                currentMenuZeroPosition++;
                                if (currentMenuZeroPosition > 2)
                                {
                                    currentMenuZeroPosition = 2;
                                }
                                cursorForMenuZero.transform.position = cursorMenuZeroPositions[currentMenuZeroPosition].transform.position;
                                stickReset = false;
                            }
                            else if (myController.VerticalMenu() > 0.1f)
                            {
                                currentMenuZeroPosition--;
                                if (currentMenuZeroPosition < 1 && numSaveFiles < 1)
                                {
                                    currentMenuZeroPosition = 1;
                                }
                                if (currentMenuZeroPosition < 0)
                                {
                                    currentMenuZeroPosition = 0;
                                }
                                cursorForMenuZero.transform.position = cursorMenuZeroPositions[currentMenuZeroPosition].transform.position;
                                stickReset = false;
                            }

                        }
                        if (selectReset)
                        {
                            if (myController.GetCustomInput(12))
                            {
                                if (currentMenuZeroPosition == 0)
                                {
                                    // load game. if more than 1 file available, open load screen
                                    if (numSaveFiles > 1)
                                    {
                                        // open load screen
                                        OpenLoadUI();
                                    }
                                    else
                                    {

                                        SaveLoadS.Load(saveToLoad);
                                        triggerSecondScreen = true;
                                    }
                                }
                                else if (currentMenuZeroPosition == 1)
                                {
                                    // new game
                                    // if less than 3 saves, go ahead. otherwise open load screen
                                    if (numSaveFiles < 3)
                                    {
                                        if (GameDataS.current != null)
                                        {
                                            GameDataS.current.RemoveCurrent();
                                        }
                                        SaveLoadS.currentSaveSlot = numSaveFiles;
                                        triggerSecondScreen = true;
                                    }
                                    else
                                    {
                                        OpenLoadUI(true);
                                    }
                                }
                                else
                                {
                                    Application.Quit();
                                }
                                selectReset = false;
                            }
                        }
                        else
                        {
                            selectReset |= !myController.GetCustomInput(12);
                        }
                    }
                
                }
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
                if (GameDataS.current != null){
                    if (GameDataS.current.storyProgression != null){
                        if (GameDataS.current.storyProgression.Contains(666)){
                            menuSelectionsText[0].text = menuSelectionsText[0].text.Insert(0, "† ");
                            menuSelectionsText[0].text += " †";
                        }
                    }
                }
				secondScreenObject.SetActive(true);
				fadeOnZoom.gameObject.SetActive(false);
			}
        }else if (!loading && !inOptionsMenu){
			if (!secondScreenLoop.activeSelf){
				if (secondScreenIntro == null){
					secondScreenLoop.SetActive(true);
					SetSelection();
					selectOrb.SetActive(true);
					credits.SetActive(true);
				}
			}else{

                if (!myController.GetCustomInput(12)){
					selectReset = true;
				}

				if (Mathf.Abs(myController.VerticalMenu()) < 0.1f && Mathf.Abs(myController.HorizontalMenu()) < 0.1f){
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
						
                        // for now, only allow two options (start game), options
							if (currentSelection > 2){
								currentSelection = 0;
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
                        // for now, only allow two options (start game), options
                        if (currentSelection < 0)
                        {
                            currentSelection = 2;
                        }
						#endif
					SetSelection();
				}

				// control settings
					/*if (currentSelection == 2){
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
				}**/
				
                    if (selectReset && myController.GetCustomInput(12) 
						&& !loading && !quitting){
						attractCountdown = attractCountdownMax;
                        if (currentSelection == 0)
                        {
                            // start
                            startMusic.FadeOut();
                            loadBlackScreen.gameObject.SetActive(true);
                            loading = true;
                            selectReset = false;
                            selectOrb.SetActive(false);
                            Cursor.visible = false;
                            hideOnOverride.gameObject.SetActive(false);
                            showOnOverride.gameObject.SetActive(false);
                            newGameScene = startNewGameScene;
                            SetAdditionalInstruction(false);
                            if (GameDataS.current == null)
                            {
                                StoryProgressionS.NewGame(); // reset for new game progress
                                                             /*if (currentSelection == 0){
                                                                 newGameScene = demoOneScene;
                                                             }else{
                                                                 newGameScene = demoTwoScene;
                                                             }**/
                            }
                            else
                            {
                                newGameScene = GameOverS.reviveScene;
                                PlayerController.doWakeUp = true;
                            }
                            PlayerStatsS.healOnStart = true;
                            StartNextLoad();
                        }

                        else if (currentSelection == 1)
                        {
                            attractCountdown = attractCountdownMax;
                            SetAdditionalInstruction(false);
                            OpenOptionsScreen();
                        }
                        else if (currentSelection == 2)
                        {
                            // reset menu scene
                            attractCountdown = attractCountdownMax;
                           
                                // start reset
                                loadBlackScreen.gameObject.SetActive(true);
                            startMusic.FadeOut();
                                loading = true;
                                selectReset = false;
                                selectOrb.SetActive(false);
                                Cursor.visible = false;
                                hideOnOverride.gameObject.SetActive(false);
                                showOnOverride.gameObject.SetActive(false);
                                newGameScene = Application.loadedLevelName;
                            StartNextLoad(false);
                            SetAdditionalInstruction(false);

                        }
					}
				}
			}
		}else if (loading){
			if (loadBlackScreen.color.a >= 1f){
				if (async.progress >= 0.9f){
					//inMain = false;
					async.allowSceneActivation = true;
				}
			}
		}
		
	}

    void OpenLoadUI(bool forOver = false){
        loadFileMenu.TurnOn(this, lastUsedFile, forOver);
        inLoadMenu = true;

                attractCountdown = attractCountdownMax;
    }

    void OpenOptionsScreen(){
        inOptionsMenu = true;
        optionsMenu.TurnOn(this);
    }
    public void TurnOffOptions(){
        inOptionsMenu = false;
        selectReset = false;
        SetAdditionalInstruction(true);
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
		
		/*if (Input.GetKeyDown(KeyCode.Escape) && selectReset){
			//Application.Quit();
			currentSelection = 3;
			SetSelection();
		}**/
		
		if (allowCheats){
			if (Input.GetKeyDown(KeyCode.G)){
				cheatString += "G";
				if (cheatString == "GGGG"){
					PlayerStatsS.godMode = true;
					cheatString = "";
					Debug.Log("god mode on");
					Instantiate(newGameSound);
				}
			}
			if (Input.GetKeyDown(KeyCode.R)){
				PlayerStatDisplayS.RECORD_MODE = !PlayerStatDisplayS.RECORD_MODE;
				if (PlayerStatDisplayS.RECORD_MODE){
					Debug.Log("record mode on");
					Instantiate(newGameSound);
				}
			}
			
			if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)){
				cheatString = "";
				PlayerStatsS.godMode = false;
				PlayerStatDisplayS.RECORD_MODE = false;
				Debug.Log("god mode off");
			}
		}
		
	}

    private void StartNextLoad(bool playSound = true){
		StartCoroutine(LoadNextScene());
		startedLoading = true;	
        if (newGameSound && playSound){
			Instantiate(newGameSound);
		}
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
                blurEffect.blurSpread = Mathf.Lerp(1, 0, blurT);
                blurEffect.iterations = Mathf.FloorToInt(Mathf.Lerp(10, 0, blurT));



			}else{
				blurEffect.enabled = false;
			}
		}
	}

	void CancelBlur(){
		blurEffect.enabled = false;
	}

    public void SetAdditionalInstruction(bool newOn){
        for (int i = 0; i < additionalInstructions.Length; i++){
            additionalInstructions[i].SetActive(newOn);
        }
    }
}
