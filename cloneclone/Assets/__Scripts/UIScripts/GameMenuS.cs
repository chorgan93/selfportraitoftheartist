using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameMenuS : MonoBehaviour
{

    private InGameMenuManagerS myManager;

    public static bool unlockedChallenge;
    public static bool unlockedTurbo;

    private int currentResolutionInt = 0;

    private Color textDefaultColor;
    private Color textSelectColor = Color.white;

    private int currentSelection = 0;
    public RectTransform selector;

    public Text[] selectTexts;
    public RectTransform[] selectPositions;

    private bool inConfirmation = false;
    public GameObject confirmObject;
    public Text confirmText;
    public string confirmCheckpointString;
    public string confirmMenuString;

    private int fontSizeStart = -1;
    private int fontSizeSelected;

    public Text[] confirmTexts;
    public RectTransform[] confirmPositions;

    private ControlManagerS myControl;
    public ControlManagerS MyControl { get { return myControl; } }

    private bool stickReset = false;
    private bool selectButtonUp = false;
    private bool cancelButtonUp = false;

    private bool inOptionsMenu = false;
    public GameObject optionsMenuProper;
    public RectTransform[] optionsPositions;
    public Text[] optionsTexts;
    public Text controlText;
    public Text speedText;
    public Text sinText;
    public Text punishText;
    public Text musicText;
    public Text sfxText;
    public Text shakeText;
    public Text resolutionText;
    public Text fullscreenText;
    public Text postProcessingText;
    public RectTransform optionsSelector;

    private bool quitRightFromOptions = false;

    public CustomizableControlsUIS customControlRef;
    private bool inCustomControlMenu = false;


    private int fontSizeOptionStart = -1;
    private int fontSizeOptionSelected;

    private GameOverS respawnManager;
    private bool initialized = false;

    [Header("Build Debug")]
    public Text debugStick;
    public Text debugActive;
    private bool showDebug = false;
    public bool overrideToMenu = true;

    private MainMenuNavigationS mainMenuRef;
    public bool mainMenuUpdate = false;

    private void Start()
    {
        string checkResolution = "";
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            checkResolution += Screen.resolutions[i].ToString() + "\n";
        }
        // Debug.LogError(checkResolution);
    }

    // Update is called once per frame
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha4)){
            unlockedTurbo = !unlockedTurbo;
            Debug.Log("Turbo enabled set to " + unlockedTurbo);
        }
#endif
        if (mainMenuUpdate)
        {
            GameMenuUpdate();
        }
    }
    public void GameMenuUpdate()
    {

        if (Mathf.Abs(myControl.HorizontalMenu()) < 0.1f && Mathf.Abs(myControl.VerticalMenu()) < 0.1f)
        {
            stickReset = true;
        }
        if (showDebug)
        {
            debugStick.text = "Stick Reset? " + stickReset
                + "\nH:" + Mathf.Abs(myControl.HorizontalMenu()) + " V:" + Mathf.Abs(myControl.VerticalMenu());
            debugActive.text = "Menu active? " + myManager.gMenuActive;
        }

        if (!myControl.GetCustomInput(3))
        {
            selectButtonUp = true;
        }

        if (!myControl.GetCustomInput(1))
        {
            cancelButtonUp = true;
        }

        if (!inOptionsMenu)
        {

#if UNITY_SWITCH
            if (myControl.VerticalMenu() > 0.5f && stickReset)
#else
            if (myControl.VerticalMenu() > 0.1f && stickReset)
#endif
            {
                stickReset = false;
                currentSelection--;
                myManager.pRef.ResetTimeMax();
                if (currentSelection < 0)
                {
                    currentSelection = selectTexts.Length - 1;
                }
                SetSelection(currentSelection);
            }
#if UNITY_SWITCH
            if (myControl.VerticalMenu() < -0.5f && stickReset)
#else
            if (myControl.VerticalMenu() < -0.1f && stickReset)
#endif
            {
                stickReset = false;
                currentSelection++;
                myManager.pRef.ResetTimeMax();
                if (currentSelection > selectTexts.Length - 1)
                {
                    currentSelection = 0;
                }
                SetSelection(currentSelection);
            }

            if ((cancelButtonUp && myControl.GetCustomInput(1) && !customControlRef.InReplaceMode) || (selectButtonUp && myControl.GetCustomInput(3) && currentSelection == 0))
            {
                TurnOff();
                myManager.pRef.ResetTimeMax();
                selectButtonUp = false;
                cancelButtonUp = false;
                myManager.TurnOffFromGameMenu();
            }

            if (selectButtonUp && myControl.GetCustomInput(3) && currentSelection == 2)
            {
                myManager.pRef.ResetTimeMax();
                selectButtonUp = false;
                inOptionsMenu = true;
                MatchOptionsText();
                SetSelection(0);
                optionsMenuProper.gameObject.SetActive(true);
            }

            if (selectButtonUp && myControl.GetCustomInput(3) && currentSelection == 1 && InGameMenuManagerS.allowFastTravel && PlayerInventoryS.I.CheckpointsReached() > 0)
            {
                RespawnAtLastCheckpoint();
                myManager.pRef.ResetTimeMax();
            }

            if (selectButtonUp && myControl.GetCustomInput(3) && currentSelection == 3 && InGameMenuManagerS.allowFastTravel &&
                (PlayerInventoryS.I.CheckpointsReached() > 0 || overrideToMenu))
            {
                RespawnAtLastCheckpoint(true);
                myManager.pRef.ResetTimeMax();
            }
        }
        else if (!inCustomControlMenu)
        {
            if (myControl.VerticalMenu() > 0.1f && stickReset)
            {
                stickReset = false;
                currentSelection--;
                // on switch, make sure to skip 6 and 7
#if UNITY_SWITCH
                if (currentSelection == 6 || currentSelection == 7){
                    currentSelection = 5;
                }
#endif
                if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
				if (currentSelection < 0){
					currentSelection = optionsTexts.Length-1;
				}
				SetSelection(currentSelection);
			}
			if (myControl.VerticalMenu() < -0.1f && stickReset){
				stickReset = false;
                currentSelection++;
                // on switch, make sure to skip 6 and 7
#if UNITY_SWITCH
                if (currentSelection == 6 || currentSelection == 7)
                {
                    currentSelection = 8;
                }
#endif
                if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
				if (currentSelection > optionsTexts.Length-1){
					currentSelection = 0;
				}
				SetSelection(currentSelection);
			}

			// exit options
			if ((cancelButtonUp && myControl.GetCustomInput(1)) || (selectButtonUp && myControl.GetCustomInput(3) && currentSelection == 10
			)){

				selectButtonUp = false;
				cancelButtonUp = false;
                inOptionsMenu = false;if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
				SetSelection(2);
				optionsMenuProper.gameObject.SetActive(false);
                if (quitRightFromOptions){
                    TurnOff();
                }

			}

			// control type set
			if (currentSelection == 0){
				HandleControlOption();
                if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
			}
			// speed type set
			if (currentSelection == 1){
                HandleSpeedOption();if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
			}
			// sin difficulty set
			if (currentSelection == 2){
                HandleSinOption();if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
			}
			// punishment difficulty set
			if (currentSelection == 3){
                HandlePunishOption();if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
			}
			// music volume set
			if (currentSelection == 4){
                HandleMusicOption();if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
			}
			// sfx volume set
			if (currentSelection == 5){
                HandleSfxOption();if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
			}
			// screenshake set 
			if (currentSelection == 8){
                HandleShakeOption();if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
			}

			// resolution set
			if (currentSelection == 7){
                //HandleZoomOption();
                HandleResolutionOption();
                if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
			}

			// fullscreen set
			if (currentSelection == 6){
                //HandleAliasOption();
                HandleFullscreenOption();
                if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
			}
            // post-processing set
            if (currentSelection == 9)
            {
                HandlePostOption();
                if (!mainMenuUpdate)
                {
                    myManager.pRef.ResetTimeMax();
                }
            }
		}

	}

	void SetSelection(int newSelection){
		currentSelection = newSelection;
		if (inOptionsMenu){
			for (int i = 0; i < optionsTexts.Length; i++){
				if (i == currentSelection){
					optionsTexts[i].color = textSelectColor;
					optionsTexts[i].fontSize = fontSizeOptionSelected;
					optionsSelector.anchoredPosition = optionsPositions[i].anchoredPosition;
				}else{
					optionsTexts[i].fontSize = fontSizeOptionStart;
					optionsTexts[i].color = textDefaultColor;
				}
			}
		}else{
			for (int i = 0; i < selectTexts.Length; i++){
				if (i == currentSelection){
					selector.anchoredPosition = selectPositions[i].anchoredPosition;
				}
				if (i == currentSelection && (((i != 1 || (i == 1 && InGameMenuManagerS.allowFastTravel && PlayerInventoryS.I.CheckpointsReached() > 0)))
					&& (i != 3 || (i == 3 && InGameMenuManagerS.allowFastTravel && (PlayerInventoryS.I.CheckpointsReached() > 0 || overrideToMenu))))){
				selectTexts[i].color = textSelectColor;
				selectTexts[i].fontSize = fontSizeSelected;
			}else{
				selectTexts[i].fontSize = fontSizeStart;
				selectTexts[i].color = textDefaultColor;
			}
		}
		}
	}

	void ReturnToGame(){
		TurnOff();
		myManager.TurnOffFromGameMenu();
	}

	void RespawnAtLastCheckpoint(bool toMenu = false){
		TurnOff();
		selectButtonUp = false;
		cancelButtonUp = false;
		myManager.TurnOffFromGameMenu();
		myManager.pRef.SetExamining(false, Vector3.zero, "");
		myManager.pRef.SetTalking(true);
		respawnManager.FakeDeath(toMenu);
		if (!toMenu){
		myManager.pRef.myStats.DeathCountUp(true);
		}
	}

    public void TurnOn(MainMenuNavigationS goToOptions){
		if (!initialized){

			textDefaultColor = selectTexts[0].color;
			SetSelection(0);
			initialized = true;

			if (showDebug){
				debugStick.text = "Stick Reset? " + stickReset 
					+"\nH:" + Mathf.Abs(myControl.HorizontalMenu()) + " V:"+Mathf.Abs(myControl.VerticalMenu());
				debugActive.text = "Menu active? " + myManager.gMenuActive;
			}else{
				debugStick.enabled = false;
				debugActive.enabled = false;
			}
		}

		InGameMenuManagerS.menuInUse = true;
		inOptionsMenu = false;
		optionsMenuProper.gameObject.SetActive(false);
		inConfirmation = false;
		cancelButtonUp = false;
		if (fontSizeStart <= 0){
			fontSizeStart = 60;
			fontSizeSelected = Mathf.CeilToInt(fontSizeStart*1.05f);
			fontSizeOptionStart = 50;
			fontSizeOptionSelected = Mathf.CeilToInt(fontSizeOptionStart*1.05f);
		}
		SetSelection(0);
		gameObject.SetActive(true);
        inCustomControlMenu = false;
        quitRightFromOptions = false;
        if (goToOptions != null){
            myControl = goToOptions.controlRef;
            //Debug.Log(myControl);
            quitRightFromOptions = true;
                selectButtonUp = false;
            inOptionsMenu = true;
            //Debug.Log(inOptionsMenu);
            MatchOptionsText();
            SetSelection(0);
            optionsMenuProper.gameObject.SetActive(true);
            mainMenuRef = goToOptions;
        }
		//Debug.Log("game menu turn ON");
		
	}
	public void TurnOff(){
		inOptionsMenu = false;
		inConfirmation = false;
		InGameMenuManagerS.menuInUse = false;
		optionsMenuProper.gameObject.SetActive(false);
        customControlRef.TurnOff(this);
        gameObject.SetActive(false);
        inCustomControlMenu = false;
        if (mainMenuRef){
            mainMenuRef.TurnOffOptions();
        }
		//Debug.LogError("game menu turn OFF");
		
	}

	public void SetManager(InGameMenuManagerS mm){
		myManager = mm;
		myControl = GetComponent<ControlManagerS>();
		respawnManager = myManager.pRef.GetComponent<GameOverS>();
	}

	//________________________________OPTIONS HANDLERS
	void HandleControlOption(){

		// moving to its own screen. select to turn on
        if (selectButtonUp && myControl.GetCustomInput(3)){
            customControlRef.TurnOn(this);
            inCustomControlMenu = true;
            selectButtonUp = false;
        }

	}

	void UpdateControlSettingText(){
        /*if (ControlManagerS.controlProfile == 0){
			controlText.text = "Gamepad";
			Cursor.visible = false;
		}
		else if (ControlManagerS.controlProfile == 3){
			controlText.text = "Gamepad (PS4)";
			Cursor.visible = false;
		}
		else if (ControlManagerS.controlProfile == 1){
			controlText.text = "Keyboard & Mouse";
			Cursor.visible = true;
		}
		else if (ControlManagerS.controlProfile == 2){
			controlText.text = "Keyboard (No Mouse)";
			Cursor.visible = false;
		}*/
        controlText.enabled = false;
	}

	void HandleSinOption(){

#if UNITY_SWITCH
        if (myControl.HorizontalMenu() > 0.5f && stickReset)
#else
		if (myControl.HorizontalMenu() > 0.1f && stickReset)
#endif
            {
			stickReset = false;
			int difficultySelect = DifficultyS.GetSinInt();
			difficultySelect++;
            if (difficultySelect > 2 && !SaveLoadS.challengeUnlocked)
            {
                difficultySelect=2;
            }
			if (difficultySelect > 3){
				difficultySelect = 3;
			}
			DifficultyS.SetDifficultiesFromInt(difficultySelect, DifficultyS.GetPunishInt());
			UpdateSinSettingText();
		}
#if UNITY_SWITCH
        if (myControl.HorizontalMenu() < -0.5f && stickReset)
#else
        if (myControl.HorizontalMenu() < -0.1f && stickReset)
#endif
        {
            stickReset = false;
			int difficultySelect = DifficultyS.GetSinInt();
			difficultySelect--;
			if (difficultySelect < 0){
				difficultySelect = 0;
			}
			DifficultyS.SetDifficultiesFromInt(difficultySelect, DifficultyS.GetPunishInt());
			UpdateSinSettingText();
		}

		if (selectButtonUp && myControl.GetCustomInput(3)){

			selectButtonUp = false;
			int difficultySelect = DifficultyS.GetSinInt();
			difficultySelect++;
            if (difficultySelect > 2 && !SaveLoadS.challengeUnlocked)
            {
                difficultySelect = 2;
            }
			if (difficultySelect > 3){
				difficultySelect = 3;
			}
			DifficultyS.SetDifficultiesFromInt(difficultySelect, DifficultyS.GetPunishInt());
			UpdateSinSettingText();

		}

	}

	void UpdateSinSettingText(){
		if (DifficultyS.GetSinInt() == 0){
			sinText.text = "Easy";
		}
		else if (DifficultyS.GetSinInt() == 1){
			sinText.text = "Normal";
		}
		else if (DifficultyS.GetSinInt() == 2){
			sinText.text = "Hard";
		}
		else if (DifficultyS.GetSinInt() == 3){
			sinText.text = "Challenge";
		}
	}

	void HandlePunishOption(){

#if UNITY_SWITCH
        if (myControl.HorizontalMenu() > 0.5f && stickReset)
#else
		if (myControl.HorizontalMenu() > 0.1f && stickReset)
#endif
        {
			stickReset = false;
			int difficultySelect = DifficultyS.GetPunishInt();
			difficultySelect++;
            if (difficultySelect > 2 && !SaveLoadS.challengeUnlocked)
            {
                difficultySelect = 2;
            }
			if (difficultySelect > 3){
				difficultySelect = 3;
			}
			DifficultyS.SetDifficultiesFromInt( DifficultyS.GetSinInt(), difficultySelect);
			UpdatePunishSettingText();
		}
#if UNITY_SWITCH
        if (myControl.HorizontalMenu() < -0.5f && stickReset)
#else
        if (myControl.HorizontalMenu() < -0.1f && stickReset)
#endif
        {
            stickReset = false;
			int difficultySelect = DifficultyS.GetPunishInt();
			difficultySelect--;
			if (difficultySelect < 0){
				difficultySelect = 0;
			}
			DifficultyS.SetDifficultiesFromInt( DifficultyS.GetSinInt(), difficultySelect);
			UpdatePunishSettingText();
		}

		if (selectButtonUp && myControl.GetCustomInput(3)){

			selectButtonUp = false;
			int difficultySelect = DifficultyS.GetPunishInt();
			difficultySelect++;
            if (difficultySelect > 2 && !SaveLoadS.challengeUnlocked)
            {
                difficultySelect = 2;
            }
			if (difficultySelect > 3){
				difficultySelect = 3;
			}
			DifficultyS.SetDifficultiesFromInt( DifficultyS.GetSinInt(), difficultySelect);
			UpdatePunishSettingText();

		}

	}

	void UpdatePunishSettingText(){
		if (DifficultyS.GetPunishInt() == 0){
			punishText.text = "Easy";
		}
		else if (DifficultyS.GetPunishInt() == 1){
			punishText.text = "Normal";
		}
		else if (DifficultyS.GetPunishInt() == 2){
			punishText.text = "Hard";
		}
		else if (DifficultyS.GetPunishInt() == 3){
			punishText.text = "Challenge";
		}
	}

	void HandleAliasOption(){
#if UNITY_SWITCH
        if ((myControl.HorizontalMenu() > 0.5f || myControl.HorizontalMenu() < -0.5f) && stickReset)
#else
            if ((myControl.HorizontalMenu() > 0.1f || myControl.HorizontalMenu() < -0.1f) && stickReset)
#endif
            {
			stickReset = false;
			CameraEffectsS.aliasOn = !CameraEffectsS.aliasOn;
			if (CameraEffectsS.E){
				CameraEffectsS.E.MatchAlias();
			}
		}

		if (selectButtonUp && myControl.GetCustomInput(3)){
			CameraEffectsS.aliasOn = !CameraEffectsS.aliasOn;
			if (CameraEffectsS.E){
				CameraEffectsS.E.MatchAlias();
			}
			selectButtonUp = false;

		}

		if (CameraEffectsS.aliasOn){
			fullscreenText.text = "ON";
		}else{
            fullscreenText.text = "OFF";
		}
	}

    void HandlePostOption()
    {
#if UNITY_SWITCH
        if ((myControl.HorizontalMenu() > 0.1f || myControl.HorizontalMenu() < -0.1f) && stickReset)
#else
        if ((myControl.HorizontalMenu() > 0.1f || myControl.HorizontalMenu() < -0.1f) && stickReset)
#endif
        {
            stickReset = false;
            CameraEffectsS.ChangeEffectSetting(!CameraEffectsS.cameraEffectsEnabled);
        }

        if (selectButtonUp && myControl.GetCustomInput(3))
        {

            CameraEffectsS.ChangeEffectSetting(!CameraEffectsS.cameraEffectsEnabled);
            selectButtonUp = false;

        }

        if (CameraEffectsS.cameraEffectsEnabled)
        {
            postProcessingText.text = "ON";
        }
        else
        {
            postProcessingText.text = "OFF";
        }
    }

    void HandleFullscreenOption(){
#if !UNITY_SWITCH
        if ((myControl.HorizontalMenu() > 0.1f || myControl.HorizontalMenu() < -0.1f) && stickReset)
        {
            stickReset = false;
            if (Screen.fullScreen)
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.fullScreen = false;
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Screen.fullScreen = true;
            }
            StartCoroutine(UpdateFullscreenSettingCoroutine());
        }

        if (selectButtonUp && myControl.GetCustomInput(3))
        {

            selectButtonUp = false;
            if (Screen.fullScreen)
            {
                Screen.fullScreen = false;
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
            else
            {
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            StartCoroutine(UpdateFullscreenSettingCoroutine());

        }
#endif

    }

	void HandleShakeOption(){

#if UNITY_SWITCH
        if ((myControl.HorizontalMenu() > 0.5f || myControl.HorizontalMenu() < -0.5f) && stickReset)
#else
        if ((myControl.HorizontalMenu() > 0.1f || myControl.HorizontalMenu() < -0.1f) && stickReset)
#endif
        {
            stickReset = false;
            if (myControl.HorizontalMenu() > 0) { 
            if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER >= 1f)
            {
                CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 0f;
                shakeText.text = "OFF";
                }else if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER > 0){
                    CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 1f;
                    shakeText.text = "ON";  
                }
            else
            {
                CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 0.5f;
                shakeText.text = "LOW";
            }
        }else{
                if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER >= 1f)
                {
                    CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 0.5f;
                    shakeText.text = "LOW";
                }
                else if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER > 0)
                {
                    CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 0f;
                    shakeText.text = "OFF";
                }
                else
                {
                    CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 1f;
                    shakeText.text = "ON";
                }
        }
		}

		if (selectButtonUp && myControl.GetCustomInput(3)){
            if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER >= 1f)
            {
                CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 0f;
                shakeText.text = "OFF";
            }
            else if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER > 0)
            {
                CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 1f;
                shakeText.text = "ON";
            }
            else
            {
                CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 0.5f;
                shakeText.text = "LOW";
            }
			selectButtonUp = false;

		}

	}

	void HandleMusicOption(){

#if UNITY_SWITCH
        if (myControl.HorizontalMenu() > 0.5f && stickReset)
#else
		if (myControl.HorizontalMenu() > 0.1f && stickReset)
#endif
        {
            stickReset = false;
			if (BGMHolderS.BG != null){
				BGMHolderS.BG.UpdateVolumeSetting(1);
			}else{
				BGMHolderS.SetVolumeSetting(1);
			}
			musicText.text = BGMHolderS.volumeMult*100f + "%";
        }
#if UNITY_SWITCH
        if (myControl.HorizontalMenu() < -0.5f && stickReset)
#else
        if (myControl.HorizontalMenu() < -0.1f && stickReset)
#endif
        {
            stickReset = false;
			if (BGMHolderS.BG != null){
				BGMHolderS.BG.UpdateVolumeSetting(-1);
			}else{
				BGMHolderS.SetVolumeSetting(-1);
			}
			musicText.text = BGMHolderS.volumeMult*100f + "%";
		}

		// exit options
		if (selectButtonUp && myControl.GetCustomInput(3)){
			if (BGMHolderS.BG != null){
				BGMHolderS.BG.UpdateVolumeSetting(1);
			}else{
				BGMHolderS.SetVolumeSetting(1);
			}
			musicText.text = BGMHolderS.volumeMult*100f + "%";
			selectButtonUp = false;

		}

	}

	void HandleSfxOption(){

#if UNITY_SWITCH
        if (myControl.HorizontalMenu() > 0.5f && stickReset)
#else
        if (myControl.HorizontalMenu() > 0.1f && stickReset)
#endif
        {
            stickReset = false;
			SFXObjS.SetVolumeSetting(1);
			sfxText.text = SFXObjS.volumeSetting*100f + "%";
        }
#if UNITY_SWITCH
        if (myControl.HorizontalMenu() < -0.5f && stickReset)
#else
        if (myControl.HorizontalMenu() < -0.1f && stickReset)
#endif
        {
            stickReset = false;
			SFXObjS.SetVolumeSetting(-1);
			sfxText.text = SFXObjS.volumeSetting*100f + "%";
		}

		// exit options
		if (selectButtonUp && myControl.GetCustomInput(3)){

			selectButtonUp = false;
			SFXObjS.SetVolumeSetting(1);
			sfxText.text = SFXObjS.volumeSetting*100f + "%";

		}

	}

	void HandleZoomOption(){

#if UNITY_SWITCH
        if (myControl.HorizontalMenu() > 0.5f && stickReset)
#else
        if (myControl.HorizontalMenu() > 0.1f && stickReset)
#endif
        {
            stickReset = false;
			CameraFollowS.ChangeZoomLevel(1);
			UpdateCameraZoomSettingText();
        }
#if UNITY_SWITCH
        if (myControl.HorizontalMenu() < -0.5f && stickReset)
#else
        if (myControl.HorizontalMenu() < -0.1f && stickReset)
#endif
        {
            stickReset = false;
			CameraFollowS.ChangeZoomLevel(-1);
			UpdateCameraZoomSettingText();
		}

		// exit options
		if (selectButtonUp && myControl.GetCustomInput(3)){
			selectButtonUp = false;
			CameraFollowS.ChangeZoomLevel(1);
			UpdateCameraZoomSettingText();

		}

	}

    void HandleResolutionOption()
    {
#if !UNITY_SWITCH
        if (myControl.HorizontalMenu() > 0.1f && stickReset)
        {
            stickReset = false;
            GetCurrentResolutionIndex();
            currentResolutionInt++;
            if (currentResolutionInt > Screen.resolutions.Length-1){
                currentResolutionInt = Screen.resolutions.Length-1;
            }
            Screen.SetResolution(Screen.resolutions[currentResolutionInt].width, Screen.resolutions[currentResolutionInt].height,
                                 Screen.fullScreen);
            //Debug.LogError("Trying to set resolution " + Screen.resolutions[currentResolutionInt].ToString());
            UpdateResolutionSettingText();
        }
        else if (myControl.HorizontalMenu() < -0.1f && stickReset)
        {
            stickReset = false;
            GetCurrentResolutionIndex(true);
            currentResolutionInt--;
            if (currentResolutionInt < 0)
            {
                currentResolutionInt = 0;
            }
            Screen.SetResolution(Screen.resolutions[currentResolutionInt].width, Screen.resolutions[currentResolutionInt].height,
                                 Screen.fullScreen);
            //Debug.LogError("Trying to set resolution " + Screen.resolutions[currentResolutionInt].ToString());
            UpdateResolutionSettingText();
        }

        else if (selectButtonUp && myControl.GetCustomInput(3))
        {
            selectButtonUp = false;
            GetCurrentResolutionIndex();
            currentResolutionInt++;
            if (currentResolutionInt > Screen.resolutions.Length-1)
            {
                currentResolutionInt = Screen.resolutions.Length-1;
            }
            Screen.SetResolution(Screen.resolutions[currentResolutionInt].width, Screen.resolutions[currentResolutionInt].height,
                                 Screen.fullScreen);
            //Debug.LogError("Trying to set resolution " + Screen.resolutions[currentResolutionInt].ToString());
            UpdateResolutionSettingText();
            

        }
#endif

    }

    void GetCurrentResolutionIndex(bool reverse = false)
    {
        float currentResolutionX = Screen.width;
        float currentResolutionY = Screen.height;
        if (reverse)
        {
            for (int i = Screen.resolutions.Length - 1; i >= 0; i--)
            {
                if (ApproximateDimension(currentResolutionX, Screen.resolutions[i].width)
                    && ApproximateDimension(currentResolutionY, Screen.resolutions[i].height))
                {
                    currentResolutionInt = i;
                    //Debug.Log("Resolutions match at index " + currentResolutionInt + ": " + Screen.resolutions[i].width + "x" + Screen.resolutions[i].height);

                }
            }
        }
        else
        {
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                if (ApproximateDimension(currentResolutionX, Screen.resolutions[i].width)
                    && ApproximateDimension(currentResolutionY, Screen.resolutions[i].height))
                {
                    currentResolutionInt = i;
                    //Debug.Log("Resolutions match at index " + currentResolutionInt + ": " + Screen.resolutions[i].width + "x" + Screen.resolutions[i].height);

                }
            }
        }

        //Debug.Log("Currently at possible resolution of " + currentResolutionInt + " of " + Screen.resolutions.Length);
    }

    private bool ApproximateDimension(float a, float b, float maxDifference = 1f)
    {
        if (Mathf.Abs(a - b) <= maxDifference)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	void HandleSpeedOption()
    {
#if UNITY_SWITCH
        if (myControl.HorizontalMenu() > 0.5f && stickReset)
#else
        if (myControl.HorizontalMenu() > 0.1f && stickReset)
#endif
        {
            stickReset = false;
			CameraShakeS.ChangeTurbo(1);
		}


		if (selectButtonUp && myControl.GetCustomInput(3)){

			selectButtonUp = false;
			CameraShakeS.ChangeTurbo(1);

		}

#if UNITY_SWITCH
        if (myControl.HorizontalMenu() < -0.5f && stickReset)
#else
        if (myControl.HorizontalMenu() < -0.1f && stickReset)
#endif
        {
            stickReset = false;
			CameraShakeS.ChangeTurbo(-1);
		}

		speedText.text = CameraShakeS.GetTurboString();
	}

	void UpdateCameraZoomSettingText(){
		if (CameraFollowS.zoomInt == 0){
			resolutionText.text = "+/- " + CameraFollowS.zoomInt.ToString();
		}else if (CameraFollowS.zoomInt > 0){
			resolutionText.text = "+ " + CameraFollowS.zoomInt.ToString();
		}else{
			resolutionText.text = CameraFollowS.zoomInt.ToString();
		}
	}

    void UpdateResolutionSettingText(){
        StartCoroutine(UpdateResoultionSettingTextCoroutine());
    }
    IEnumerator UpdateResoultionSettingTextCoroutine(){

        //Debug.Log("Possible resolutions: " + Screen.resolutions.Length);
        yield return null;

        resolutionText.text = Screen.width.ToString() + " x " + Screen.height.ToString();
    }
    IEnumerator UpdateFullscreenSettingCoroutine(){
        //Debug.Log("Fullscreen: " + Screen.fullScreen);
        yield return null;
        if (Screen.fullScreen)
        {
            fullscreenText.text = "ON";
        }else{
            fullscreenText.text = "OFF";
        }
    }


	void MatchOptionsText(){

		UpdateControlSettingText();
		UpdateSinSettingText();
		UpdatePunishSettingText();
		if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER >= 1f){
			shakeText.text = "ON";
        }else if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER > 0){
            shakeText.text = "LOW";
        }else{
			shakeText.text = "OFF";
		}
        //UpdateCameraZoomSettingText();
        UpdateResolutionSettingText();
        if (Screen.fullScreen)
        {
            fullscreenText.text = "ON";
        }else{
            fullscreenText.text = "OFF";
        }
        if (CameraEffectsS.cameraEffectsEnabled){
            postProcessingText.text = "ON";
        }else{
            postProcessingText.text = "OFF";
        }
		speedText.text = CameraShakeS.GetTurboString();
		musicText.text = BGMHolderS.volumeMult*100f + "%";
		sfxText.text = SFXObjS.volumeSetting*100f + "%";
	}

	public static void ResetOptions(){
		DifficultyS.SetDifficultiesFromInt(1,1);
		//BGMHolderS.volumeMult = 1f;
		//SFXObjS.volumeSetting = 1f;
		//CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 1f;
		//CameraFollowS.ResetZoomLevel();
	}

    public void BackFromControlMenu(){
        cancelButtonUp = selectButtonUp = false;
        inCustomControlMenu = false;
    }
}
