using UnityEngine;
using System.Collections;

public class DialogueResponseS : MonoBehaviour {

	private int currentPos = 0;
	public GameObject[] choiceIndicators;
	private int _choiceMade = -1;
	public int choiceMade { get { return _choiceMade; } }

	private ControlManagerS myControl;

	private bool stickReset = false;
	private bool selectButtonDown = false;
	private bool cancelButtonDown = false;

	private bool choiceActive = false;
	public bool getChoiceActive { get { return choiceActive; } }

	private float delayChoice = 0f;
	
	// Update is called once per frame
	void Update () {
	
		if (choiceActive && myControl){
			if (delayChoice > 0f){
				delayChoice -= Time.deltaTime;
			}else{

			if (myControl.GetCustomInput(3)){
				if (!selectButtonDown){
					_choiceMade = currentPos;
					TurnOff();
					selectButtonDown = true;
				}
			}else{
				selectButtonDown = false;
			}

			if (myControl.GetCustomInput(13)){
				if (!cancelButtonDown){
					_choiceMade = 1;
					TurnOff();
				}
				cancelButtonDown = true;
			}else{
				cancelButtonDown = false;
			}

			if (Mathf.Abs(myControl.HorizontalMenu()) > 0.1f || Mathf.Abs(myControl.VerticalMenu()) > 0.1f){
				if (stickReset){
					if (myControl.HorizontalMenu() > 0.1f || myControl.VerticalMenu() > 0.1f){
						currentPos--;
					}else{
						currentPos++;
					}
				}
				if (currentPos >= choiceIndicators.Length){
					currentPos = 0;
				}
				if (currentPos < 0){
					currentPos = choiceIndicators.Length-1;
				}
				SetPos();
				stickReset = false;
			}else{
				stickReset = true;
			}

		}
		}

	}

	void SetPos(){

		for (int i = 0 ; i < choiceIndicators.Length; i++){
			if (i == currentPos){
				choiceIndicators[i].SetActive(true);
			}else{
				choiceIndicators[i].SetActive(false);
			}
		}
		
	}

	public void TurnOn(ControlManagerS controlRef){
		if (!myControl){
			myControl = controlRef;
		}
		delayChoice = 0.3f;
		currentPos = 0;
		choiceActive = true;
		gameObject.SetActive(true);
		SetPos();
	}

	public void TurnOff(){
		gameObject.SetActive(false);
		choiceActive = false;
	}

	public int GetChoiceMade(){
		int choice = -1;
		choice = _choiceMade;
		_choiceMade = -1;
		return choice;
	}
}
