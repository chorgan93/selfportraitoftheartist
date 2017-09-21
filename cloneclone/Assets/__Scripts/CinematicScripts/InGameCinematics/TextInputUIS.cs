using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextInputUIS : MonoBehaviour {

	public static string playerName = "LUCAH";
	public GameObject inputUI;
	public RectTransform[] selectorPos;
	public RectTransform selector;
	public Text[] textFields;
	public Text[] textFieldsUnder;
	private int[] chosenLetters;
	private int currentLetter = 0;
	private int currentPos = 0;

	private string fullInput = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";

	[Header("Input Types")]
	public bool playerNameInput;

	private ControlManagerS myControl;
	private InGameCinemaTextS myCinema;


	private bool stickReset = false;
	private bool selectButtonDown = false;
	private bool backButtonDown = false;
	private bool startButtonDown = false;

	private bool canEnter = false;

	private float delayInput;


	// Use this for initialization
	void Start () {
	
		GameObject findPlayer = GameObject.Find("Player");
		if (findPlayer){
			myControl = findPlayer.GetComponent<ControlManagerS>();
		}else{
			myControl = GetComponent<ControlManagerS>();
		}

		chosenLetters = new int[8]{0,fullInput.Length-1,fullInput.Length-1,fullInput.Length-1,fullInput.Length-1,
			fullInput.Length-1,fullInput.Length-1,fullInput.Length-1};
		RefreshTexts();
		inputUI.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (delayInput > 0 && inputUI.activeSelf){
			delayInput -= Time.deltaTime;
		}
		else if (inputUI.activeSelf){
			if (stickReset && Mathf.Abs(myControl.HorizontalMenu()) > 0.4f){
				stickReset = false;
				if (myControl.HorizontalMenu() < 0){
					ChangeCurrentLetter(-1);
				}else{
					ChangeCurrentLetter(1);
				}
			}
			if (stickReset && Mathf.Abs(myControl.VerticalMenu()) > 0.4f){
				stickReset = false;
				if (myControl.VerticalMenu() < 0){
					ChangeCurrentLetter(-1);
				}else{
					ChangeCurrentLetter(1);
				}
			}
			if (!stickReset && Mathf.Abs(myControl.HorizontalMenu()) < 0.1f && Mathf.Abs(myControl.VerticalMenu()) < 0.1f){
				stickReset = true;
			}

			if (!selectButtonDown && myControl.MenuSelectButton()){
				selectButtonDown = true;
				SetLetter(true);
			}
			if (selectButtonDown && myControl.MenuSelectUp()){
				selectButtonDown = false;
			}

			if (!backButtonDown && myControl.ExitButton()){
				backButtonDown = true;
				PressDel();
			}
			if (backButtonDown && myControl.ExitButtonUp()){
				backButtonDown = false;
			}

			if (!startButtonDown && myControl.StartButton()){
				startButtonDown = true;
				if (canEnter){
					FinishInput();
				}
			}
			if (startButtonDown && !myControl.StartButton()){
				startButtonDown = false;
			}
		}
	}

	public void Activate(InGameCinemaTextS newCinema){
		canEnter = false;
		currentLetter = 0;
		currentPos = 0;
		selector.anchoredPosition = selectorPos[currentPos].anchoredPosition;
		SetLetter(false);
		delayInput = 0.3f;
		myCinema = newCinema;
		inputUI.SetActive(true);
	}

	void RefreshTexts(){
		for (int i = 0; i < textFields.Length; i++){
			textFields[i].text = textFieldsUnder[i].text = " ";
		}
	}

	void ChangeCurrentLetter(int dir){
		if (dir > 0){
			currentLetter++;
			if (currentLetter >= fullInput.Length){
				currentLetter = 0;
			}
		}else{
			currentLetter--;
			if (currentLetter < 0){
				currentLetter = fullInput.Length-1;
			}
		}
		SetLetter(false);
	}

	void SetLetter(bool advance = false){
		textFields[currentPos].text = textFieldsUnder[currentPos].text = fullInput[currentLetter].ToString();
		chosenLetters[currentPos] = currentLetter;
		if (currentLetter < fullInput.Length-1){
			canEnter = true;
		}
		if (advance){
		if (currentPos < textFields.Length-1){
				currentPos++;
				selector.anchoredPosition = selectorPos[currentPos].anchoredPosition;
				currentLetter = fullInput.Length-1;
				SetLetter(false);
			}else{
				FinishInput();
			}
		}
	}

	void PressDel(){

		if (currentPos > 0){
			textFields[currentPos].text = textFieldsUnder[currentPos].text = fullInput[fullInput.Length-1].ToString();
			currentPos--;
			selector.anchoredPosition = selectorPos[currentPos].anchoredPosition;
			currentLetter = chosenLetters[currentPos];
		}else{
			currentLetter = chosenLetters[0] = 0;
		}
		SetLetter(false);
	}

	void FinishInput(){
		if (playerNameInput){
			playerName = "";
			for (int i = 0; i < textFields.Length; i++){
				playerName += textFields[i].text;
			}
			bool foundLetter = false;
			for (int j = playerName.Length-1; j >= 0; j--){
				if (playerName[j] == ' ' && !foundLetter){
					playerName = playerName.Remove(j, 1);
				}else{
					foundLetter = true;
				}
			}
		}
		myCinema.SetInputComplete();
		inputUI.SetActive(false);
	}
}
