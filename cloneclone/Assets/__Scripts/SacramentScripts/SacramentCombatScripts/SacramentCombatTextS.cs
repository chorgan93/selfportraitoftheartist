using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SacramentCombatTextS : MonoBehaviour {
	
	private SacramentCombatS _myCombat;

	private Text myText;
	private bool _initialized;
	public float delayCombatStartTime = 0.6f;

	[Header("Appear Properties")]
	public float delayAppearance = 0f;
	private float delayAppearanceCountdown;
	public float autoAdvanceTime = -1f;
	private float autoAdvanceCount;
	private bool useAutoAdvance = false;

	[Header("Fade Properties")]
	public float fadeRate;
	public float maxAlpha = 1f;
	private bool fadingIn = false;
	private Color myCol;

	private int maxLines = 1;
	private int currentLines = 0;
	private List<string> setStrings = new List<string>();

	[Header("Scroll Properties")]
	public float scrollRate;
	private string fullText;
	private string currentText;
	private int currentChar = 0;
	private float scrollCountdown;
	private bool isScrolling = false;
	public bool disableAdvance=  false;

	private bool _readyToAdvance = false;
	public bool readyToAdvance { get { return _readyToAdvance; }}
	private bool textActive = false;
	private SacramentCombatActionS actionWaitingToAdvance;

	[Header ("Sound Properties")]
	public GameObject appearSound;
	private bool playedAppearSound = false;
	public GameObject scrollSound;
	public int scrollSoundTiming = 3;
	private int scrollSoundCountdown;

	private bool awaitingInput = false;
	private bool inputGiven = false;
	private bool selectingAlly = false;
	private bool beginCombatOnEnd = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (textActive){
			if (delayAppearanceCountdown > 0){
				delayAppearanceCountdown -= Time.deltaTime;
			}else{

				if (useAutoAdvance && autoAdvanceCount > 0f){
					autoAdvanceCount -= Time.deltaTime;
				}
				if (!playedAppearSound){
					if (appearSound){
						Instantiate(appearSound);
					}
					playedAppearSound = true;
				}
		if (fadingIn){
				myCol = myText.color;
				myCol.a += fadeRate*Time.deltaTime;
					if (myCol.a >= maxAlpha || ((Input.GetMouseButtonDown(0) && _myCombat.myStep.myHandler.usingMouse) 
						|| (!_myCombat.myStep.myHandler.usingMouse && _myCombat.myStep.myHandler.TalkButton()))){
					myCol.a = maxAlpha;
					fadingIn = false;
					_readyToAdvance = true;
				}
				myText.color = myCol;
		}
		if (isScrolling){
				scrollCountdown -= Time.deltaTime;
				if (scrollCountdown <= 0){
					scrollCountdown = scrollRate;
					currentChar++;
					if (currentChar >= fullText.Length){
						isScrolling = false;
						_readyToAdvance = true;
							if (beginCombatOnEnd){
								StartCoroutine(SendBeginMessage());
								beginCombatOnEnd = false;
							}
							if (awaitingInput){
								AddToString("", null, true, false, selectingAlly);
							
								textActive = false;
								
							}
					}else{
						currentText += fullText[currentChar];
							scrollSoundCountdown--;
							if (scrollSoundCountdown <= 0){
								scrollSoundCountdown = scrollSoundTiming;
								if (scrollSound){
									Instantiate(scrollSound);
								}
							}
						myText.text = currentText;
					}
				}

				
		}
			}
			if (((Input.GetMouseButtonDown(0) && _myCombat.myStep.myHandler.usingMouse) ||
				(!_myCombat.myStep.myHandler.usingMouse && _myCombat.myStep.myHandler.TalkButton()))
				&& inputGiven){
				if (awaitingInput){
					if (isScrolling){

						myText.text = currentText = fullText;
						isScrolling = false;

						AddToString("", null, true, false, selectingAlly);
						textActive = false;

					}
				}
				else if (!_readyToAdvance){
					myText.text = currentText = fullText;
					isScrolling = false;if (beginCombatOnEnd){
						StartCoroutine(SendBeginMessage());
						beginCombatOnEnd = false;
					}
					_readyToAdvance = true;
				}else if (actionWaitingToAdvance){
					// first, check if action is interrupted by overwatch
					if (_myCombat.CheckOverwatchAction(actionWaitingToAdvance)){
					actionWaitingToAdvance.AdvanceAction(true);
						_myCombat.StartOverwatchAction();
					}else{
						actionWaitingToAdvance.AdvanceAction();
					}
				}else{
					_myCombat.AdvanceTurn();
				}
			}
		}
	
	}

	public void ActivateText(SacramentCombatS myCom, string startCombatString){
		if (!_initialized){
			myText = GetComponent<Text>();
			fullText = myText.text = "";
			myCol = myText.color;
			maxAlpha = myCol.a;
			if (autoAdvanceTime > 0){
				useAutoAdvance = true;
			}
			currentLines = 0;
			_myCombat = myCom;
			_initialized = true;
		}
		if (useAutoAdvance){
			autoAdvanceCount = autoAdvanceTime;
		}
		playedAppearSound = false;
		delayAppearanceCountdown = delayAppearance;
	
			myText.text = "";
			currentText = "";
			currentChar = -1;
			scrollCountdown = scrollRate;
			isScrolling = true;

		setStrings = new List<string>();
		textActive = true;
		gameObject.SetActive(true);

		AddToString(startCombatString, null);
		beginCombatOnEnd = true;
		
	}
	public void DeactivateText(){
					textActive = false;
		_readyToAdvance = false;
	}

	public IEnumerator SendBeginMessage(){
		yield return new WaitForSeconds(delayCombatStartTime);
		_myCombat.Begin();
	}

	public void AddToString(string newString, SacramentCombatActionS advanceAct, bool awaitInput = false, bool givingInput = false, bool allyTarget = false){
		
		if (currentLines > maxLines){
			if (setStrings[0].Length+1 > currentText.Length){
				myText.text = currentText = "";
			}else{
			myText.text = currentText = currentText.Remove(0, setStrings[0].Length+1);
			}
			fullText = currentText+"\n"+newString;
			setStrings.RemoveAt(0);
		}else{
			if (currentLines == 0){

				fullText = newString;
			}else{

				fullText = currentText+"\n"+newString;
			}
			currentLines++;
		}
		setStrings.Add(newString);
		_readyToAdvance = false;
		actionWaitingToAdvance = advanceAct;
		selectingAlly = allyTarget;
		if (advanceAct != null){
			if (advanceAct.actionType == SacramentCombatActionS.SacramentActionType.Overwatch ||
				advanceAct.actionType == SacramentCombatActionS.SacramentActionType.FirstAid){
				selectingAlly = true;
			}
		}
		scrollCountdown = scrollRate;
		currentChar = currentText.Length-1;
		isScrolling = true;
		

		awaitingInput = awaitInput;
		inputGiven = givingInput;
		if (!awaitingInput){
			inputGiven = true;
		}
		if (inputGiven){
			_myCombat.HideChoices();
			textActive = true;
		}

		if (newString == "" && !selectingAlly){
			_myCombat.ShowChoices();
		}
	}
}
