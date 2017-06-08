using UnityEngine;
using System.Collections;

public class InGameCinemaTextS : MonoBehaviour {

	private InGameCinematicS _myHandler;
	private ControlManagerS _myControl;

	public int myCinemaStep = 0;

	public string[] textStrings;
	private int currentString = 0;

	private bool advanceButtonDown = true;
	private bool dialogueComplete = false;

	public bool textZoom = false;

	private bool textAfter = false;

	// Use this for initialization
	void Start () {

		_myHandler = GetComponentInParent<InGameCinematicS>();
		_myControl = _myHandler.pRef.myControl;

		AddNewlines();

		DialogueManagerS.D.SetDisplayText(textStrings[currentString], false, textZoom);

		CheckForDialogueAfter();
	}
	
	// Update is called once per frame
	void Update () {

		if (!dialogueComplete){
		if (!advanceButtonDown && _myControl.TalkButton()){
			if (DialogueManagerS.D.doneScrolling){
				currentString++;
				if (currentString > textStrings.Length-1){
						if (!textAfter){
							DialogueManagerS.D.EndText();
						}
					_myHandler.AdvanceCinematic();
						dialogueComplete = true;
					}else{
						DialogueManagerS.D.SetDisplayText(textStrings[currentString], false, textZoom);
					}
				}else{
					DialogueManagerS.D.CompleteText();
				}
		}

		if (!_myControl.TalkButton()){
			advanceButtonDown = false;
			}else{
				advanceButtonDown = true;
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
		}
	}
}
