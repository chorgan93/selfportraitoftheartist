using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KeypadS : MonoBehaviour {

	public GameObject keypadDisplay;
	public KeypadKeys[] keypadInputs;
	private int currentSelection = 0;
	private bool keypadOn = false;
	public Text currentCodeDisplay;
	private string currentCode = "";
	private string codeToMatch;

	private LockedDoorS myDoor;
	private ControlManagerS myControl;

	private bool stickReset = false;
	private bool selectButtonDown = false;
	private bool backButtonDown = false;
	private bool startButtonDown = false;

	private float delayInput;

	// Use this for initialization
	void Start () {
	
		codeToMatch = PlayerInventoryS.I.tvNum.ToString();
		keypadDisplay.SetActive(false);
		keypadOn = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (delayInput > 0 && keypadOn){
			delayInput -= Time.deltaTime;
		}
		else if (keypadOn){
#if UNITY_SWITCH
            if (stickReset && Mathf.Abs(myControl.HorizontalMenu()) > 0.5f)
#else
            if (stickReset && Mathf.Abs(myControl. HorizontalMenu()) > 0.1f)
#endif
            {
				stickReset = false;
				if (myControl.HorizontalMenu() < 0){
					currentSelection--;
					if (currentSelection < 0){
						currentSelection = keypadInputs.Length-1;
					}
					RefreshKeys();
				}else{
					currentSelection++;
					if (currentSelection > keypadInputs.Length-1){
						currentSelection = 0;
					}
					RefreshKeys();
				}
			}
#if UNITY_SWITCH
            if (stickReset && Mathf.Abs(myControl.VerticalMenu()) > 0.5f)
#else
            if (stickReset && Mathf.Abs(myControl.VerticalMenu()) > 0.1f)
#endif
            {
				stickReset = false;
				if (myControl.VerticalMenu() < 0){
					currentSelection+=3;
					if (currentSelection > keypadInputs.Length-1){
						currentSelection -= 12;
					}
					RefreshKeys();
				}else{
					currentSelection-=3;
					if (currentSelection < 0){
						currentSelection += 12;
					}
					RefreshKeys();
				}
			}
			if (!stickReset && Mathf.Abs(myControl.HorizontalMenu()) < 0.1f && Mathf.Abs(myControl.VerticalMenu()) < 0.1f)
            {
				stickReset = true;
			}

			if (!selectButtonDown && myControl.GetCustomInput(3)){
				selectButtonDown = true;
				SelectKey();
			}
            if (selectButtonDown && !myControl.GetCustomInput(3)){
				selectButtonDown = false;
			}

			if (!backButtonDown && myControl.GetCustomInput(11)){
				backButtonDown = true;
				PressDel(true);
			}
			if (backButtonDown && !myControl.GetCustomInput(11)){
				backButtonDown = false;
			}

			if (!startButtonDown && myControl.GetCustomInput(10)){
				startButtonDown = true;
				PressEnter();
			}
			if (startButtonDown && !myControl.GetCustomInput(10)){
				startButtonDown = false;
			}
		}

	}

	public void TurnOn(LockedDoorS newDoor, ControlManagerS cRef){
		myDoor = newDoor;
		delayInput = 0.3f;
		currentSelection = 0;
		currentCode = "";
		currentCodeDisplay.text = currentCode;
		RefreshKeys();
		keypadDisplay.gameObject.SetActive(true);
		keypadOn = true;
		myControl = cRef;
	}

	void RefreshKeys(){
		for (int i = 0; i < keypadInputs.Length; i++){
			if (i == currentSelection){
				keypadInputs[i].SetSelect(true);
			}else{
				keypadInputs[i].SetSelect(false);
			}
		}
	}

	void SelectKey(){
		keypadInputs[currentSelection].Press();
		if (currentSelection <= 8){
			currentCode += (currentSelection+1).ToString();
			if (currentCode.Length > 3){
				currentCode = currentCode.Remove(0,1);
			}
			currentCodeDisplay.text = currentCode.ToString();
		}else if (currentSelection == 10){
			currentCode += 0;
			if (currentCode.Length > 3){
				currentCode = currentCode.Remove(0,1);
			}
			currentCodeDisplay.text = currentCode.ToString();
		}else if (currentSelection == 9){
			PressDel();
		}else{
			PressEnter();
		}
	}

	void PressDel(bool playSound = false){

		if (playSound){
		keypadInputs[9].Press();
		}
		if (currentCode.Length > 0){
			currentCode = currentCode.Remove(currentCode.Length-1,1);
		}
		currentCodeDisplay.text = currentCode.ToString();
	}

	void PressEnter(){
		if (currentCode == codeToMatch){
			myDoor.EndKeypad(true);
		}else{
			myDoor.EndKeypad(false);
		}
		currentCode = "";
		currentSelection = 0;
		keypadDisplay.SetActive(false);
		keypadOn = false;
	}
}
