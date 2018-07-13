using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameMenuS : MonoBehaviour {

	private InGameMenuManagerS myManager;

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
    public ControlManagerS MyControl { get { return myControl; }}

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
	public Text zoomText;
	public Text aliasText;
	public RectTransform optionsSelector;

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


	// Update is called once per frame
	public void GameMenuUpdate () {

		if (Mathf.Abs(myControl.HorizontalMenu()) < 0.2f && Mathf.Abs(myControl.VerticalMenu()) < 0.2f){
			stickReset = true;
		}
		if (showDebug){
			debugStick.text = "Stick Reset? " + stickReset 
				+"\nH:" + Mathf.Abs(myControl.HorizontalMenu()) + " V:"+Mathf.Abs(myControl.VerticalMenu());
			debugActive.text = "Menu active? " + myManager.gMenuActive;
		}

		if (myControl.MenuSelectUp()){
			selectButtonUp = true;
		}

		if (myControl.ExitButtonUp()){
			cancelButtonUp = true;
		}

		if (!inOptionsMenu){

			if (myControl.VerticalMenu() > 0.2f && stickReset){
				stickReset = false;
				currentSelection--;
				myManager.pRef.ResetTimeMax();
				if (currentSelection < 0){
					currentSelection = selectTexts.Length-1;
				}
				SetSelection(currentSelection);
			}
			if (myControl.VerticalMenu() < -0.2f && stickReset){
				stickReset = false;
				currentSelection++;
				myManager.pRef.ResetTimeMax();
				if (currentSelection > selectTexts.Length-1){
					currentSelection = 0;
				}
				SetSelection(currentSelection);
			}

            if ((cancelButtonUp && myControl.GetCustomInput(13) && !customControlRef.InReplaceMode) || (selectButtonUp && myControl.GetCustomInput(12) && currentSelection == 0)){
				TurnOff();
				myManager.pRef.ResetTimeMax();
				selectButtonUp = false;
				cancelButtonUp = false;
				myManager.TurnOffFromGameMenu();
			}

			if (selectButtonUp && myControl.GetCustomInput(12) && currentSelection == 2){
				myManager.pRef.ResetTimeMax();
				selectButtonUp = false;
				inOptionsMenu = true;
				MatchOptionsText();
				SetSelection(0);
				optionsMenuProper.gameObject.SetActive(true);
			}

			if (selectButtonUp && myControl.GetCustomInput(12) && currentSelection == 1 && InGameMenuManagerS.allowFastTravel && PlayerInventoryS.I.CheckpointsReached() > 0){
				RespawnAtLastCheckpoint();
				myManager.pRef.ResetTimeMax();
			}

			if (selectButtonUp && myControl.GetCustomInput(12) && currentSelection == 3 && InGameMenuManagerS.allowFastTravel && 
				(PlayerInventoryS.I.CheckpointsReached() > 0 || overrideToMenu)){
				RespawnAtLastCheckpoint(true);
				myManager.pRef.ResetTimeMax();
			}
        }else if (!inCustomControlMenu){
			if (myControl.VerticalMenu() > 0.2f && stickReset){
				stickReset = false;
				currentSelection--;
				myManager.pRef.ResetTimeMax();
				if (currentSelection < 0){
					currentSelection = optionsTexts.Length-1;
				}
				SetSelection(currentSelection);
			}
			if (myControl.VerticalMenu() < -0.2f && stickReset){
				stickReset = false;
				currentSelection++;
				myManager.pRef.ResetTimeMax();
				if (currentSelection > optionsTexts.Length-1){
					currentSelection = 0;
				}
				SetSelection(currentSelection);
			}

			// exit options
			if ((cancelButtonUp && myControl.GetCustomInput(13)) || (selectButtonUp && myControl.GetCustomInput(12) && currentSelection == 9
			)){

				selectButtonUp = false;
				cancelButtonUp = false;
				inOptionsMenu = false;
				myManager.pRef.ResetTimeMax();
				SetSelection(2);
				optionsMenuProper.gameObject.SetActive(false);

			}

			// control type set
			if (currentSelection == 0){
				HandleControlOption();
				myManager.pRef.ResetTimeMax();
			}
			// speed type set
			if (currentSelection == 1){
				HandleSpeedOption();
				myManager.pRef.ResetTimeMax();
			}
			// sin difficulty set
			if (currentSelection == 2){
				HandleSinOption();
				myManager.pRef.ResetTimeMax();
			}
			// punishment difficulty set
			if (currentSelection == 3){
				HandlePunishOption();
				myManager.pRef.ResetTimeMax();
			}
			// music volume set
			if (currentSelection == 4){
				HandleMusicOption();
				myManager.pRef.ResetTimeMax();
			}
			// sfx volume set
			if (currentSelection == 5){
				HandleSfxOption();
				myManager.pRef.ResetTimeMax();
			}
			// screenshake set
			if (currentSelection == 6){
				HandleShakeOption();
				myManager.pRef.ResetTimeMax();
			}

			// zoom set
			if (currentSelection == 7){
				HandleZoomOption();
				myManager.pRef.ResetTimeMax();
			}

			// alias set
			if (currentSelection == 8){
				HandleAliasOption();
				myManager.pRef.ResetTimeMax();
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

	public void TurnOn(){
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
        if (selectButtonUp && myControl.GetCustomInput(12)){
            customControlRef.TurnOn(this);
            inCustomControlMenu = true;
            selectButtonUp = false;
        }

	}

	void UpdateControlSettingText(){
		if (ControlManagerS.controlProfile == 0){
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
		}
	}

	void HandleSinOption(){

		if (myControl.HorizontalMenu() > 0.2f && stickReset){
			stickReset = false;
			int difficultySelect = DifficultyS.GetSinInt();
			difficultySelect++;
			if (difficultySelect > 3){
				difficultySelect = 3;
			}
			DifficultyS.SetDifficultiesFromInt(difficultySelect, DifficultyS.GetPunishInt());
			UpdateSinSettingText();
		}
		if (myControl.HorizontalMenu() < -0.2f && stickReset){
			stickReset = false;
			int difficultySelect = DifficultyS.GetSinInt();
			difficultySelect--;
			if (difficultySelect < 0){
				difficultySelect = 0;
			}
			DifficultyS.SetDifficultiesFromInt(difficultySelect, DifficultyS.GetPunishInt());
			UpdateSinSettingText();
		}

		if (selectButtonUp && myControl.GetCustomInput(12)){

			selectButtonUp = false;
			int difficultySelect = DifficultyS.GetSinInt();
			difficultySelect++;
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

		if (myControl.HorizontalMenu() > 0.2f && stickReset){
			stickReset = false;
			int difficultySelect = DifficultyS.GetPunishInt();
			difficultySelect++;
			if (difficultySelect > 3){
				difficultySelect = 3;
			}
			DifficultyS.SetDifficultiesFromInt( DifficultyS.GetSinInt(), difficultySelect);
			UpdatePunishSettingText();
		}
		if (myControl.HorizontalMenu() < -0.2f && stickReset){
			stickReset = false;
			int difficultySelect = DifficultyS.GetPunishInt();
			difficultySelect--;
			if (difficultySelect < 0){
				difficultySelect = 0;
			}
			DifficultyS.SetDifficultiesFromInt( DifficultyS.GetSinInt(), difficultySelect);
			UpdatePunishSettingText();
		}

		if (selectButtonUp && myControl.GetCustomInput(12)){

			selectButtonUp = false;
			int difficultySelect = DifficultyS.GetPunishInt();
			difficultySelect++;
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
		if ((myControl.HorizontalMenu() > 0.2f || myControl.HorizontalMenu() < -0.2f) && stickReset){
			stickReset = false;
			CameraEffectsS.aliasOn = !CameraEffectsS.aliasOn;
			if (CameraEffectsS.E){
				CameraEffectsS.E.MatchAlias();
			}
		}

		if (selectButtonUp && myControl.GetCustomInput(12)){
			CameraEffectsS.aliasOn = !CameraEffectsS.aliasOn;
			if (CameraEffectsS.E){
				CameraEffectsS.E.MatchAlias();
			}
			selectButtonUp = false;

		}

		if (CameraEffectsS.aliasOn){
			aliasText.text = "ON";
		}else{
			aliasText.text = "OFF";
		}
	}

	void HandleShakeOption(){

		if ((myControl.HorizontalMenu() > 0.2f || myControl.HorizontalMenu() < -0.2f) && stickReset){
			stickReset = false;
			if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER == 1f){
				CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 0f;
				shakeText.text = "OFF";
			}else{
				CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 1f;
				shakeText.text = "ON";
			}
		}

		if (selectButtonUp && myControl.GetCustomInput(12)){
			if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER == 1f){
				CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 0f;
				shakeText.text = "OFF";
			}else{
				CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 1f;
				shakeText.text = "ON";
			}
			selectButtonUp = false;

		}

	}

	void HandleMusicOption(){

		if (myControl.HorizontalMenu() > 0.2f && stickReset){
			stickReset = false;
			if (BGMHolderS.BG != null){
				BGMHolderS.BG.UpdateVolumeSetting(1);
			}else{
				BGMHolderS.SetVolumeSetting(1);
			}
			musicText.text = BGMHolderS.volumeMult*100f + "%";
		}
		if (myControl.HorizontalMenu() < -0.2f && stickReset){
			stickReset = false;
			if (BGMHolderS.BG != null){
				BGMHolderS.BG.UpdateVolumeSetting(-1);
			}else{
				BGMHolderS.SetVolumeSetting(-1);
			}
			musicText.text = BGMHolderS.volumeMult*100f + "%";
		}

		// exit options
		if (selectButtonUp && myControl.GetCustomInput(12)){
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

		if (myControl.HorizontalMenu() > 0.2f && stickReset){
			stickReset = false;
			SFXObjS.SetVolumeSetting(1);
			sfxText.text = SFXObjS.volumeSetting*100f + "%";
		}
		if (myControl.HorizontalMenu() < -0.2f && stickReset){
			stickReset = false;
			SFXObjS.SetVolumeSetting(-1);
			sfxText.text = SFXObjS.volumeSetting*100f + "%";
		}

		// exit options
		if (selectButtonUp && myControl.GetCustomInput(12)){

			selectButtonUp = false;
			SFXObjS.SetVolumeSetting(1);
			sfxText.text = SFXObjS.volumeSetting*100f + "%";

		}

	}

	void HandleZoomOption(){

		if (myControl.HorizontalMenu() > 0.2f && stickReset){
			stickReset = false;
			CameraFollowS.ChangeZoomLevel(1);
			UpdateCameraZoomSettingText();
		}
		if (myControl.HorizontalMenu() < -0.2f && stickReset){
			stickReset = false;
			CameraFollowS.ChangeZoomLevel(-1);
			UpdateCameraZoomSettingText();
		}

		// exit options
		if (selectButtonUp && myControl.GetCustomInput(12)){
			selectButtonUp = false;
			CameraFollowS.ChangeZoomLevel(1);
			UpdateCameraZoomSettingText();

		}

	}

	void HandleSpeedOption(){
		if (myControl.HorizontalMenu() > 0.2f && stickReset){
			stickReset = false;
			CameraShakeS.ChangeTurbo(1);
		}


		if (selectButtonUp && myControl.GetCustomInput(12)){

			selectButtonUp = false;
			CameraShakeS.ChangeTurbo(1);

		}

		if (myControl.HorizontalMenu() < -0.2f && stickReset){
			stickReset = false;
			CameraShakeS.ChangeTurbo(-1);
		}

		speedText.text = CameraShakeS.GetTurboString();
	}

	void UpdateCameraZoomSettingText(){
		if (CameraFollowS.zoomInt == 0){
			zoomText.text = "+/- " + CameraFollowS.zoomInt.ToString();
		}else if (CameraFollowS.zoomInt > 0){
			zoomText.text = "+ " + CameraFollowS.zoomInt.ToString();
		}else{
			zoomText.text = CameraFollowS.zoomInt.ToString();
		}
	}

	void MatchOptionsText(){

		UpdateControlSettingText();
		UpdateSinSettingText();
		UpdatePunishSettingText();
		if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER >= 1f){
			shakeText.text = "ON";
		}else{
			shakeText.text = "OFF";
		}
		UpdateCameraZoomSettingText();
		speedText.text = CameraShakeS.GetTurboString();
		musicText.text = BGMHolderS.volumeMult*100f + "%";
		sfxText.text = SFXObjS.volumeSetting*100f + "%";
	}

	public static void ResetOptions(){
		DifficultyS.SetDifficultiesFromInt(0,0);
		BGMHolderS.volumeMult = 1f;
		SFXObjS.volumeSetting = 1f;
		CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 1f;
		CameraFollowS.ResetZoomLevel();
	}

    public void BackFromControlMenu(){
        cancelButtonUp = selectButtonUp = false;
        inCustomControlMenu = false;
    }
}
