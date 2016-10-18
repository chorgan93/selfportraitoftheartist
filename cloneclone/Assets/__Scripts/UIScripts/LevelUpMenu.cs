﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelUpMenu : MonoBehaviour {

	public RectTransform cursorObj;
	private int currentPos = 0;

	private PlayerController pRef;
	private ControlManagerS myControl;

	[Header("Main Menu Selections")]
	public RectTransform[] mainMenuSelectPositions;
	public Text[] mainMenuTextObjs;
	private Color textStartColor;
	private int textStartSize;
	public float textSelectSizeMult = 1.2f;
	public GameObject mainMenuObj;

	public GameObject levelMenuProper;
	private bool onLevelMenu = false;
	private bool onTravelMenu = false;

	private bool _canBeExited = false;
	public bool canBeExited { get { return _canBeExited; } }

	private bool _controlStickMoved = false;
	private bool _selectButtonDown = false;
	private bool _exitButtonDown = false;

	private bool _initialized = false;

	[HideInInspector]
	public bool sendExitMessage = false;

	// Use this for initialization
	void Start () {
	
		levelMenuProper.gameObject.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {

		if (!onLevelMenu && !onTravelMenu){

			_canBeExited = true;

			if (!_controlStickMoved && (Mathf.Abs(myControl.Horizontal()) > 0.1f ||
			                            Mathf.Abs(myControl.Vertical()) > 0.1f)){
				_controlStickMoved = true;

				mainMenuTextObjs[currentPos].fontSize = textStartSize;
				mainMenuTextObjs[currentPos].color = textStartColor;

				if (myControl.Horizontal() > 0f ||
				    myControl.Vertical() < 0f){
					currentPos++;
					if (currentPos > mainMenuSelectPositions.Length-1){
						currentPos = 0;
					}
				}else{
					currentPos--;
					if (currentPos < 0){
						currentPos = mainMenuSelectPositions.Length-1;
					}
				}

				
				mainMenuTextObjs[currentPos].fontSize = Mathf.RoundToInt(textStartSize*textSelectSizeMult);
				mainMenuTextObjs[currentPos].color = Color.white;
			}

			cursorObj.anchoredPosition = mainMenuSelectPositions[currentPos].anchoredPosition;

			if (!_selectButtonDown && myControl.MenuSelectButton()){
				_selectButtonDown = true;
				if (currentPos == 0){
					TurnOnLevelUpMenu();
				}
				if (currentPos == 2){
					TurnOff();
					sendExitMessage = true;
				}
			}

		}

		if (onLevelMenu){
			if (!_exitButtonDown && myControl.ExitButton()){
				TurnOffLevelUpMenu();
			}
		}

		if (myControl.ExitButtonUp()){
			_exitButtonDown = false;
		}
		if (myControl.MenuSelectUp()){
			_selectButtonDown = false;
		}
		if (Mathf.Abs(myControl.Horizontal()) < 0.1f && Mathf.Abs(myControl.Vertical()) < 0.1f){
			_controlStickMoved = false;
		}
	
	}

	private void TurnOnLevelUpMenu(){
		levelMenuProper.gameObject.SetActive(true);
		mainMenuObj.SetActive(false);
		_canBeExited = false;
		currentPos = 0;
		onLevelMenu = true;
	}

	private void TurnOffLevelUpMenu(){
		levelMenuProper.gameObject.SetActive(false);
		mainMenuObj.SetActive(true);
		_canBeExited = true;
		currentPos = 0;
		onLevelMenu = false;
	}

	public void TurnOn(){

		if (!_initialized){
			
			pRef = GetComponentInParent<InGameMenuManagerS>().pRef;
			myControl = pRef.myControl;
			textStartColor = mainMenuTextObjs[0].color;
			textStartSize = mainMenuTextObjs[0].fontSize;
			_initialized = true;
		}

		gameObject.SetActive(true);
		mainMenuObj.SetActive(true);
		_canBeExited = true;
		onLevelMenu = false;
		onTravelMenu = false;
		currentPos = 0;

		_selectButtonDown = true;
		_exitButtonDown = true;
		
		cursorObj.anchoredPosition = mainMenuSelectPositions[currentPos].anchoredPosition;
		mainMenuTextObjs[currentPos].fontSize = Mathf.RoundToInt(textStartSize*textSelectSizeMult);
		mainMenuTextObjs[currentPos].color = Color.white;
	}

	private void TurnOff(){
		foreach(Text t in mainMenuTextObjs){
			t.fontSize = textStartSize;
			t.color = textStartColor;
		}
		gameObject.SetActive(false);
		levelMenuProper.gameObject.SetActive(false);
	}
}
