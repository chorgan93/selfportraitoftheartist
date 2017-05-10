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

	public Text[] confirmTexts;
	public RectTransform[] confirmPositions;

	private ControlManagerS myControl;

	private bool stickReset = false;
	private bool selectButtonUp = false;
	private bool cancelButtonUp = false;

	private bool inOptionsMenu = false;

	private GameOverS respawnManager;

	// Use this for initialization
	void Start () {

		textDefaultColor = selectTexts[0].color;
		SetSelection(0);

	}

	// Update is called once per frame
	void Update () {

		if (Mathf.Abs(myControl.HorizontalMenu()) < 0.1f && Mathf.Abs(myControl.VerticalMenu()) < 0.1f){
			stickReset = true;
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

			if (selectButtonUp && myControl.MenuSelectButton() && currentSelection == 1 && InGameMenuManagerS.allowFastTravel){
				RespawnAtLastCheckpoint();
			}

			if (selectButtonUp && myControl.MenuSelectButton() && currentSelection == 3){
				RespawnAtLastCheckpoint(true);
			}
		}

	}

	void SetSelection(int newSelection){
		currentSelection = newSelection;
		for (int i = 0; i < selectTexts.Length; i++){
			if (i == currentSelection){
				selectTexts[i].color = textSelectColor;
				selector.anchoredPosition = selectPositions[i].anchoredPosition;
			}else{
				selectTexts[i].color = textDefaultColor;
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
		inOptionsMenu = false;
		inConfirmation = false;
		cancelButtonUp = false;
		SetSelection(0);
		gameObject.SetActive(true);
		
	}
	public void TurnOff(){
		inOptionsMenu = false;
		inConfirmation = false;
		gameObject.SetActive(false);
		
	}

	public void SetManager(InGameMenuManagerS mm){
		myManager = mm;
				myControl = myManager.pRef.myControl;
		respawnManager = myManager.pRef.GetComponent<GameOverS>();
	}
}
