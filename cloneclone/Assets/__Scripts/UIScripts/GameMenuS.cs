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

	private bool stickReset = false;
	private bool selectButtonUp = false;
	private bool cancelButtonUp = false;

	private bool inOptionsMenu = false;
	public GameObject optionsMenuProper;
	public RectTransform[] optionsPositions;
	public Text[] optionsTexts;
	public Text controlText;
	public Text sinText;
	public Text punishText;
	public Text musicText;
	public Text sfxText;
	public Text shakeText;
	public RectTransform optionsSelector;


	private int fontSizeOptionStart = -1;
	private int fontSizeOptionSelected;

	private GameOverS respawnManager;
	private bool initialized = false;

	[Header("Build Debug")]
	public Text debugStick;
	public Text debugActive;
	private bool showDebug = false;


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
				if (currentSelection < 0){
					currentSelection = selectTexts.Length-1;
				}
				SetSelection(currentSelection);
			}
			if (myControl.VerticalMenu() < -0.2f && stickReset){
				stickReset = false;
				currentSelection++;
				if (currentSelection > selectTexts.Length-1){
					currentSelection = 0;
				}
				SetSelection(currentSelection);
			}

			if ((cancelButtonUp && myControl.ExitButton()) || (selectButtonUp && myControl.MenuSelectButton() && currentSelection == 0)){
				TurnOff();
				selectButtonUp = false;
				cancelButtonUp = false;
				myManager.TurnOffFromGameMenu();
			}

			if (selectButtonUp && myControl.MenuSelectButton() && currentSelection == 2){
				selectButtonUp = false;
				inOptionsMenu = true;
				MatchOptionsText();
				SetSelection(0);
				optionsMenuProper.gameObject.SetActive(true);
			}

			if (selectButtonUp && myControl.MenuSelectButton() && currentSelection == 1 && InGameMenuManagerS.allowFastTravel && PlayerInventoryS.I.CheckpointsReached() > 0){
				RespawnAtLastCheckpoint();
			}

			if (selectButtonUp && myControl.MenuSelectButton() && currentSelection == 3 && InGameMenuManagerS.allowFastTravel && PlayerInventoryS.I.CheckpointsReached() > 0){
				RespawnAtLastCheckpoint(true);
			}
		}else{
			if (myControl.VerticalMenu() > 0.2f && stickReset){
				stickReset = false;
				currentSelection--;
				if (currentSelection < 0){
					currentSelection = optionsTexts.Length-1;
				}
				SetSelection(currentSelection);
			}
			if (myControl.VerticalMenu() < -0.2f && stickReset){
				stickReset = false;
				currentSelection++;
				if (currentSelection > optionsTexts.Length-1){
					currentSelection = 0;
				}
				SetSelection(currentSelection);
			}

			// exit options
			if ((cancelButtonUp && myControl.ExitButton()) || (selectButtonUp && myControl.MenuSelectButton() && currentSelection == 6)){

				selectButtonUp = false;
				cancelButtonUp = false;
				inOptionsMenu = false;
				SetSelection(2);
				optionsMenuProper.gameObject.SetActive(false);

			}

			// control type set
			if (currentSelection == 0){
				HandleControlOption();
			}
			// sin difficulty set
			if (currentSelection == 1){
				HandleSinOption();
			}
			// punishment difficulty set
			if (currentSelection == 2){
				HandlePunishOption();
			}
			// music volume set
			if (currentSelection == 3){
				HandleMusicOption();
			}
			// sfx volume set
			if (currentSelection == 4){
				HandleSfxOption();
			}
			// screenshake set
			if (currentSelection == 5){
				HandleShakeOption();
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
					&& (i != 3 || (i == 3 && InGameMenuManagerS.allowFastTravel && PlayerInventoryS.I.CheckpointsReached() > 0)))){
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
		//Debug.Log("game menu turn ON");
		
	}
	public void TurnOff(){
		inOptionsMenu = false;
		inConfirmation = false;
		optionsMenuProper.gameObject.SetActive(false);
		gameObject.SetActive(false);
		//Debug.LogError("game menu turn OFF");
		
	}

	public void SetManager(InGameMenuManagerS mm){
		myManager = mm;
		myControl = GetComponent<ControlManagerS>();
		respawnManager = myManager.pRef.GetComponent<GameOverS>();
	}

	//________________________________OPTIONS HANDLERS
	void HandleControlOption(){

		if (myControl.HorizontalMenu() > 0.2f && stickReset){
			stickReset = false;
			ControlManagerS.controlProfile++;
			if (ControlManagerS.controlProfile > 2){
				if (myControl.ControllerAttached()){
					ControlManagerS.controlProfile = 0;
				}else{
					ControlManagerS.controlProfile = 1;
				}
			}
			UpdateControlSettingText();
		}
		if (myControl.HorizontalMenu() < -0.2f && stickReset){
			stickReset = false;
			ControlManagerS.controlProfile--;
			if ((ControlManagerS.controlProfile < 0 && myControl.ControllerAttached()) 
				|| (ControlManagerS.controlProfile < 1 && !myControl.ControllerAttached())){
				ControlManagerS.controlProfile = 2;
			}
			UpdateControlSettingText();
		}

		if (selectButtonUp && myControl.MenuSelectButton()){
			
			ControlManagerS.controlProfile++;
			if (ControlManagerS.controlProfile > 2){
				if (myControl.ControllerAttached()){
					ControlManagerS.controlProfile = 0;
				}else{
					ControlManagerS.controlProfile = 1;
				}
			}
			selectButtonUp = false;

			UpdateControlSettingText();
		}

	}

	void UpdateControlSettingText(){
		if (ControlManagerS.controlProfile == 0){
			controlText.text = "Gamepad";
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

		if (selectButtonUp && myControl.MenuSelectButton()){

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

		if (selectButtonUp && myControl.MenuSelectButton()){

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

		if (selectButtonUp && myControl.MenuSelectButton()){
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
		if (selectButtonUp && myControl.MenuSelectButton()){
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
		if (selectButtonUp && myControl.MenuSelectButton()){

			selectButtonUp = false;
			SFXObjS.SetVolumeSetting(1);
			sfxText.text = SFXObjS.volumeSetting*100f + "%";

		}

	}

	void MatchOptionsText(){

		UpdateControlSettingText();
		UpdateSinSettingText();
		UpdatePunishSettingText();
		if (CameraShakeS.OPTIONS_SHAKE_MULTIPLIER == 1f){
			shakeText.text = "ON";
		}else{
			shakeText.text = "OFF";
		}
		musicText.text = BGMHolderS.volumeMult*100f + "%";
		sfxText.text = SFXObjS.volumeSetting*100f + "%";
	}

	public static void ResetOptions(){
		DifficultyS.SetDifficultiesFromInt(0,0);
		BGMHolderS.volumeMult = 1f;
		SFXObjS.volumeSetting = 1f;
		CameraShakeS.OPTIONS_SHAKE_MULTIPLIER = 1f;
	}
}
