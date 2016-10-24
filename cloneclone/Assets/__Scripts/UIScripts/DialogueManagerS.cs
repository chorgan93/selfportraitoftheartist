using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueManagerS : MonoBehaviour {

	public Image dialogueBox;
	public Text dialogueText;

	public Image memoBG;
	public Text memoText;

	private string currentDisplayString;
	private string targetDisplayString;
	private int currentChar = 0;

	private float scrollRate = 0.02f;
	private float scrollCountdown;

	private bool _doneScrolling = true;
	public bool doneScrolling { get { return _doneScrolling; } }

	public static DialogueManagerS D;

	void Awake(){

		D = this;

	}

	// Use this for initialization
	void Start () {

		dialogueBox.enabled = false;
		dialogueText.enabled = false;
		memoBG.enabled = false;
		memoText.enabled = false;
		_doneScrolling = true;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!_doneScrolling){

			scrollCountdown -= Time.deltaTime;
			if (scrollCountdown <= 0){
				scrollCountdown = scrollRate;
				currentDisplayString += targetDisplayString[currentChar];
				currentChar++;
				dialogueText.text = currentDisplayString;
				if (currentChar >= targetDisplayString.Length){
					_doneScrolling = true;
				}
			}

		}
	
	}

	public void SetDisplayText(string newText, bool isMemo = false){

		if (!isMemo){
			memoBG.enabled = false;
			memoText.enabled = false;
		dialogueBox.enabled = true;
		dialogueText.enabled = true;

		dialogueText.text = currentDisplayString = "";
		targetDisplayString = newText;
			
			scrollCountdown = 0f;
			
			_doneScrolling = false;
			currentChar = 0;
		}else{
			
			memoBG.enabled = true;
			memoText.enabled = true;
			dialogueBox.enabled = false;
			dialogueText.enabled = false;

			memoText.text = newText;
			_doneScrolling = true;
		}


	}

	public void CompleteText(){
		dialogueText.text = currentDisplayString = targetDisplayString;
		_doneScrolling = true;
	}

	public void EndText(){

		dialogueBox.enabled = false;
		dialogueText.enabled = false;
		memoBG.enabled = false;
		memoText.enabled = false;
		
		dialogueText.text = currentDisplayString = targetDisplayString = "";
		
		scrollCountdown = 0f;
		currentChar = 0;

	}
}
