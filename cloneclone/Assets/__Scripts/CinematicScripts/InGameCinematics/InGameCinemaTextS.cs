using UnityEngine;
using System.Collections;

public class InGameCinemaTextS : MonoBehaviour {

	private InGameCinematicS _myHandler;
	public InGameCinematicS myHandler { get { return _myHandler; } }
	private ControlManagerS _myControl;
	public ControlManagerS myControl { get { return _myControl; } }

	public int myCinemaStep = 0;

	public string[] textStrings;
	private int currentString = 0;

	private bool advanceButtonDown = true;
	private bool dialogueComplete = false;

	public float skipAfterTime = 2f;
	public bool waitForSkip = false;
	public bool skipAfterTimePasses = false;

	public bool textZoom = false;
	public bool hideTextOnEnd = false;

	private bool textAfter = false;

	[Header ("First Scene Info")]
	public bool dontTurnOnStats = false;
	public bool waitForInput = false;
	private bool waitForDialogueOption = false;
	public TextInputUIS textInputRef;
	private bool awaitingInput = false;
	public InGameCinemaDialogueOptionS[] dialogueOptions;

	// Use this for initialization
	void Start () {

		_myHandler = GetComponentInParent<InGameCinematicS>();
		_myControl = _myHandler.pRef.myControl;

		AddNewlines();

		DialogueManagerS.D.SetDisplayText(textStrings[currentString], false, textZoom);

		CheckForDialogueAfter();
		advanceButtonDown = true;

		if (dialogueOptions != null){
			if (dialogueOptions.Length > 0){
				waitForDialogueOption = true;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (!dialogueComplete && !awaitingInput){
			if (skipAfterTimePasses){
				skipAfterTime -= Time.deltaTime;
				if (skipAfterTime <= 0){
					if (!textInputRef){
						if (!textAfter || hideTextOnEnd){
							DialogueManagerS.D.EndText(!dontTurnOnStats);
						}
						_myHandler.dialogueDone = true;
						_myHandler.TurnOnTime();
						dialogueComplete = true;
					}else{
						textInputRef.Activate(this);
						awaitingInput = true;
					}
				}
			}
		if (!advanceButtonDown && _myControl.GetCustomInput(3)){
				advanceButtonDown = true;
				if (!waitForSkip){
			if (DialogueManagerS.D.doneScrolling){
				currentString++;
				if (currentString > textStrings.Length-1){
							if (!textInputRef && !waitForDialogueOption){
						if (!textAfter){
								DialogueManagerS.D.EndText(!dontTurnOnStats);
						}
						_myHandler.dialogueDone = true;
						_myHandler.TurnOnTime();
						dialogueComplete = true;
							}else if (waitForDialogueOption){
								awaitingInput = true;
								for (int i = 0; i < dialogueOptions.Length; i++){
									dialogueOptions[i].Initialize(this);
								}
							}
							else{
							textInputRef.Activate(this);
							awaitingInput = true;
						}
					}else{
						DialogueManagerS.D.SetDisplayText(textStrings[currentString], false, textZoom);
					}
					
				}else{
					DialogueManagerS.D.CompleteText();
				}
				}
		}

			if (!_myControl.GetCustomInput(3) && advanceButtonDown){
			advanceButtonDown = false;
			}
		}
	
	}

	private void CheckForDialogueAfter(){
		for (int i = 0; i < _myHandler.cinemaDialogues.Length; i++){
			if (_myHandler.cinemaDialogues[i].myCinemaStep == myCinemaStep+1){
				textAfter = true;
			}
		}
	}

	void AddNewlines(){
		for (int i = 0; i < textStrings.Length; i++){
			textStrings[i] = textStrings[i].Replace("NEWLINE","\n");
			textStrings[i] = textStrings[i].Replace("PLAYERNAME", TextInputUIS.playerName);
		}
	}

	public void SetInputComplete(){
		awaitingInput = false;
		if (!textAfter){
			DialogueManagerS.D.EndText();
		}
		_myHandler.dialogueDone = true;
		_myHandler.TurnOnTime();
		dialogueComplete = true;
	}
	public void SelectDialogueOption(InGameCinemaDialogueOptionS selectedOption){
		awaitingInput = false;
		if (!textAfter){
			DialogueManagerS.D.EndText();
		}
		_myHandler.dialogueDone = true;
		_myHandler.TurnOnTime();
		dialogueComplete = true;

		for (int i = 0; i < dialogueOptions.Length; i++){
			if (dialogueOptions[i] != selectedOption){
				dialogueOptions[i].gameObject.SetActive(false);
			}
		}
	}
}
