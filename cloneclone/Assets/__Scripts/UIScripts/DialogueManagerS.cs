using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueManagerS : MonoBehaviour {

	public Image dialogueBox;
	public Image dialogueBoxTop;
	public Text dialogueText;
	private Vector2 boxBottomStartPos;
	private Vector2 boxBottomHidePos;
	private Vector2 boxTopStartPos;
	private Vector2 boxTopHidePos;

	private float showTimeMax = 0.3f;
	private float showTime;
	private float showT;

	private bool isLerping = false;
	private bool isLerpingOut = false;
	private PlayerStatDisplayS hideStats;
	private bool statsOn;

	public Image memoBG;
	public Text memoText;

	private string currentDisplayString;
	private string targetDisplayString;
	private int currentChar = 0;

	private float scrollRate = 0.02f;
	private float scrollCountdown;

	private bool _doneScrolling = true;
	public bool doneScrolling { get { return _doneScrolling; } }

	private bool _freezeText = false;

	public static DialogueManagerS D;

	void Awake(){

		D = this;

	}

	// Use this for initialization
	void Start () {

		dialogueBox.enabled = dialogueBoxTop.enabled = false;
		dialogueText.enabled = false;
		memoBG.enabled = false;
		memoText.enabled = false;
		_doneScrolling = true;

		boxBottomStartPos = boxBottomHidePos = dialogueBox.rectTransform.anchoredPosition;
		boxBottomHidePos.y -= dialogueBox.rectTransform.sizeDelta.y;
		
		boxTopStartPos = boxTopHidePos = dialogueBoxTop.rectTransform.anchoredPosition;
		boxTopHidePos.y += dialogueBox.rectTransform.sizeDelta.y;

		hideStats = GetComponentInChildren<PlayerStatDisplayS>();
		if (hideStats.gameObject.activeSelf){
			statsOn = true;
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!_doneScrolling && !_freezeText){

			scrollCountdown -= Time.deltaTime;
			if (scrollCountdown <= 0){
				scrollCountdown = scrollRate;
				currentDisplayString += targetDisplayString[currentChar];
				currentChar++;
				dialogueText.text = currentDisplayString+ " ";
				if (currentChar >= targetDisplayString.Length){
					_doneScrolling = true;
				}
			}

		}

		if (isLerping){
			showTime += Time.deltaTime;
			if (showTime >= showTimeMax){
				showTime = showTimeMax;
				isLerping = false;
			}
			showT = showTime/showTimeMax;
			showT = Mathf.Sin(showT * Mathf.PI * 0.5f);

			dialogueBox.rectTransform.anchoredPosition = Vector2.Lerp(boxBottomHidePos, boxBottomStartPos, showT);
			dialogueBoxTop.rectTransform.anchoredPosition = Vector2.Lerp(boxTopHidePos, boxTopStartPos, showT);
		}
		if (isLerpingOut){
			showTime -= Time.deltaTime;
			if (showTime <= 0){
				showTime = 0;
				isLerpingOut = false;
				dialogueBox.enabled = dialogueBoxTop.enabled = false;
			}
			showT = showTime/(showTimeMax/2f);
			showT = Mathf.Sin(showT * Mathf.PI * 0.5f);
			
			dialogueBox.rectTransform.anchoredPosition = Vector2.Lerp(boxBottomHidePos, boxBottomStartPos, showT);
			dialogueBoxTop.rectTransform.anchoredPosition = Vector2.Lerp(boxTopHidePos, boxTopStartPos, showT);
		}
	
	}

	public void SetDisplayText(string newText, bool isMemo = false, bool doZoom = true){

		if (!isMemo){
			memoBG.enabled = false;
			memoText.enabled = false;

			if (!dialogueBox.enabled){
				dialogueBox.rectTransform.anchoredPosition = boxBottomHidePos;
				dialogueBoxTop.rectTransform.anchoredPosition = boxTopHidePos;
				dialogueBox.enabled = dialogueBoxTop.enabled = true;
				dialogueText.enabled = true;

					showTime = showT = 0f;
					isLerping = true;
						isLerpingOut = false;

				hideStats.gameObject.SetActive(false);
			}

			if (doZoom){
				CameraFollowS.F.SetDialogueZoomIn(true);
			}

		dialogueText.text = currentDisplayString = "";
		targetDisplayString = newText;
			
			scrollCountdown = 0f;
			
			_doneScrolling = false;
			currentChar = 0;
		}else{
			
			memoBG.enabled = true;
			memoText.enabled = true;
			dialogueBox.enabled = dialogueBoxTop.enabled = false;
			dialogueText.enabled = false;

			memoText.text = newText;
			_doneScrolling = true;
		}


	}

	public void CompleteText(){
		dialogueText.text = currentDisplayString = targetDisplayString+" ";
		_doneScrolling = true;
	}

	public void EndText(){

		dialogueText.enabled = false;
		memoBG.enabled = false;
		memoText.enabled = false;

				isLerpingOut = true;
				isLerping = false;
				showTime = showTimeMax/2f;

		CameraFollowS.F.EndZoom();
		if (statsOn){
			hideStats.gameObject.SetActive(true);
		}
		
		dialogueText.text = currentDisplayString = targetDisplayString = "";
		
		scrollCountdown = 0f;
		currentChar = 0;

	}

	public void FreezeDialogue(){
		_freezeText = true;
	}
}
