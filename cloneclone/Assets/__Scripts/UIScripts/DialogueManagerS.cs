using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;

public class DialogueManagerS : MonoBehaviour {

	public Image dialogueBox;
	public Image dialogueBoxTop;
	public Text dialogueText;
	public Text merchantText;
	private Vector2 boxBottomStartPos;
	private Vector2 boxBottomHidePos;
	private Vector2 boxTopStartPos;
	private Vector2 boxTopHidePos;

	private Vector2 memoTextStartPos;
	private Vector2 memoTextMoviePos;


	public DialogueResponseS dialogueResponse;

	public GameObject advanceIndicator;

	private float showTimeMax = 0.3f;
	private float showTime;
	private float showT;

	private bool isLerping = false;
	private bool isLerpingOut = false;
	private PlayerStatDisplayS hideStats;
	private bool statsOn;

	public Image memoBG;
	public VideoPlayer memoMovie;
    public RawImage memoMovieImage;
	public Text memoText;

	public Image itemPopup;
	public Image itemPopupBG;
	private float popMaxHeight;
	private Outline popUpOutline;
	private Color outlineColor;

	private string currentDisplayString;
	private string targetDisplayString;
	private int currentChar = 0;

	private float scrollRate = 0.02f;
	private float scrollCountdown;

	private bool _doneScrolling = true;
	public bool doneScrolling { get { return _doneScrolling; } }

	private bool _textActive = false;

	private bool _freezeText = false;

	private bool _usingMerchantText = false;

	private bool triggerResponse = false;

	public static DialogueManagerS D;
	public GameObject dialogueAdvanceSound;

    public bool showingText { get { return _textActive; }}

	void Awake(){

		D = this;

	}

	// Use this for initialization
	void Start () {

		dialogueBox.enabled = dialogueBoxTop.enabled = false;
		dialogueText.enabled = false;
		memoBG.enabled = false;
		memoText.enabled = false;
		memoTextStartPos = memoText.rectTransform.anchoredPosition;
		memoTextMoviePos = memoTextStartPos;
		memoTextMoviePos.y+=85f;
		memoMovie.enabled = false;
        memoMovie.gameObject.SetActive(false);
        memoMovieImage.gameObject.SetActive(false);
        memoMovieImage.gameObject.SetActive(false);
        _doneScrolling = true;

		itemPopup.enabled = itemPopupBG.enabled = false;
		popMaxHeight = itemPopupBG.rectTransform.sizeDelta.y;
		popUpOutline = itemPopup.GetComponent<Outline>();
		outlineColor = popUpOutline.effectColor;
		itemPopupBG.gameObject.SetActive(false);

		advanceIndicator.SetActive(false);

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

		if (_textActive){
		if (!_doneScrolling && !_freezeText){

			scrollCountdown -= Time.deltaTime;
			if (scrollCountdown <= 0){
				scrollCountdown = scrollRate;
				currentDisplayString += targetDisplayString[currentChar];
				currentChar++;
				if (_usingMerchantText && merchantText){
					merchantText.text = currentDisplayString+ " ";
				}else{
					dialogueText.text = currentDisplayString+ " ";
				}
				if (currentChar >= targetDisplayString.Length){
					_doneScrolling = true;
						//Debug.LogError("Done scrolling by update!");
					advanceIndicator.SetActive(true);
					if (triggerResponse){
						triggerResponse = false;
						dialogueResponse.TurnOn(hideStats.pConRef.myControl);
					}
				}
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
			if (_usingMerchantText){
				merchantText.rectTransform.position = dialogueText.rectTransform.position;
			}
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
			if (_usingMerchantText){
				merchantText.rectTransform.position = dialogueText.rectTransform.position;
			}
		}
	
	}

	public void SetItemFind(Sprite newItem, Color spriteCol, Color outlineCol){
		StartCoroutine(ItemFindEffect());
		itemPopup.sprite = newItem;
		itemPopup.color = spriteCol;
        itemPopup.gameObject.SetActive(true);
		outlineColor.r = outlineCol.r;
		outlineColor.g = outlineCol.g;
		outlineColor.b = outlineCol.b;
		popUpOutline.effectColor = outlineCol;
		itemPopup.enabled = itemPopupBG.enabled = true;
		itemPopupBG.gameObject.SetActive(true);
	}

	IEnumerator ItemFindEffect(){
		float itemEffectCount = 0f;
		float itemEffectTime = 0.2f;
		Vector2 popUpSize = itemPopupBG.rectTransform.sizeDelta;
		//itemPopupBG.rectTransform.pivot = new Vector2(0.5f, 1f);
		//itemPopupBG.rectTransform.anchoredPosition = new Vector2(0f, -95);
		while (itemEffectCount < itemEffectTime){
			itemEffectCount += Time.deltaTime;
			if (itemEffectCount >= itemEffectTime){
				itemEffectCount = itemEffectTime;
			}
			popUpSize.y = Mathf.Lerp(0f, popMaxHeight, itemEffectCount/itemEffectTime);
			itemPopupBG.rectTransform.sizeDelta = popUpSize;
			yield return null;
		}
	}
	IEnumerator ItemFindDisable(){
		float itemEffectCount = 0f;
		float itemEffectTime = 0.12f;
		Vector2 popUpSize = itemPopupBG.rectTransform.sizeDelta;
		//itemPopupBG.rectTransform.pivot = new Vector2(0.5f, 0f);
		//itemPopupBG.rectTransform.anchoredPosition = new Vector2(0f, -295f);
		while (itemEffectCount < itemEffectTime){
			itemEffectCount += Time.deltaTime;
			if (itemEffectCount >= itemEffectTime){
				itemEffectCount = itemEffectTime;
			}
			popUpSize.y = Mathf.Lerp(popMaxHeight, 0f, itemEffectCount/itemEffectTime);
			itemPopupBG.rectTransform.sizeDelta = popUpSize;
			yield return null;
		}

        itemPopup.enabled = itemPopupBG.enabled = false;
        itemPopup.gameObject.SetActive(false);
		itemPopupBG.gameObject.SetActive(false);
	}

	public void SetDisplayText(string newText, bool isMemo = false, bool doZoom = true, bool fromMerchant = false, 
		bool endWithResponse = false, VideoClip movieText = null, float movieSizeMult = 1f, bool ignoreLoc = false){

		if (hideStats){
			hideStats.pConRef.ResetTimeMax();
		}
		if (_textActive){

			Instantiate(dialogueAdvanceSound);

		}
		if (!isMemo){
			memoBG.enabled = false;
			memoText.enabled = false;

			triggerResponse = endWithResponse;

			if (!dialogueBox.enabled){
				dialogueBox.rectTransform.anchoredPosition = boxBottomHidePos;
				dialogueBoxTop.rectTransform.anchoredPosition = boxTopHidePos;
				dialogueBox.enabled = dialogueBoxTop.enabled = true;
				if (fromMerchant && merchantText){
					merchantText.enabled = true;
				}else{
					dialogueText.enabled = true;
				}

					showTime = showT = 0f;
					isLerping = true;
						isLerpingOut = false;

				hideStats.gameObject.SetActive(false);
			}

			if (doZoom){
				CameraFollowS.F.SetDialogueZoomIn(true);
			}else{
				CameraFollowS.F.SetDialogueZoomIn(false);
			}

			if (fromMerchant && merchantText){
				_usingMerchantText = true;
				merchantText.text = currentDisplayString = "";
			}else{
				dialogueText.text = currentDisplayString = "";
			}
            if (!ignoreLoc)
            {
                targetDisplayString = 
                    LocalizationManager.instance.GetLocalizedValue(newText).Replace("PLAYERNAME", TextInputUIS.playerName).Replace("\n", System.Environment.NewLine);
            }else{
                targetDisplayString = newText.Replace("PLAYERNAME", TextInputUIS.playerName);
            }
			
			scrollCountdown = 0f;
			
			_doneScrolling = false;
			currentChar = 0;
		}else{
			
			memoBG.enabled = true;
			memoText.enabled = true;
			dialogueBox.enabled = dialogueBoxTop.enabled = false;
			dialogueText.enabled = false;

			if (movieText){
			if (!memoMovie.enabled){
                    // will not work on switch; disable until solution is found
#if !UNITY_SWITCH
                    memoText.rectTransform.anchoredPosition = memoTextMoviePos;
                    memoMovieImage.enabled = false;
				memoMovie.enabled = true;
                    memoMovie.gameObject.SetActive(true);
                    memoMovieImage.gameObject.SetActive(true);
				memoMovie.clip = movieText;
                    StartCoroutine(PlayTutorialMovie());
#else
                    memoText.rectTransform.anchoredPosition = memoTextStartPos;
#endif
                }
			}else{

				memoText.rectTransform.anchoredPosition = memoTextStartPos;
			}

			memoText.text = LocalizationManager.instance.GetLocalizedValue(newText).Replace("\n", System.Environment.NewLine);
            _doneScrolling = true;
			//Debug.LogError("Done scrolling bc memo!");
		}
		advanceIndicator.SetActive(false);
		_textActive = true;


	}

    IEnumerator PlayTutorialMovie(){
        memoMovie.Prepare();
        while (!memoMovie.isPrepared){
            yield return null;
        }
        //memoMovie.SetNativeSize();
        //memoMovie.rectTransform.sizeDelta *= movieSizeMult;
        //MovieTexture playMovie = (MovieTexture)memoMovie.mainTexture;
        //memoMovie.loop = true;
        memoMovieImage.texture = memoMovie.texture;
        memoMovie.Play();
        memoMovieImage.rectTransform.sizeDelta = new Vector2(425f, memoMovie.texture.height*425f/memoMovie.texture.width);
        memoMovieImage.enabled = true;
        //memoMovieImage.SetNativeSize();
        Debug.Log("Play movie!");
    }

	public void CompleteText(){
		if (hideStats){
			hideStats.pConRef.ResetTimeMax();
		}
		if (_usingMerchantText){
			merchantText.text = currentDisplayString = targetDisplayString+" ";
		}else{
			dialogueText.text = currentDisplayString = targetDisplayString+" ";
		}
		_doneScrolling = true;
		//Debug.LogError("Done scrolling by complete text function!");
		advanceIndicator.SetActive(true);
		if (triggerResponse){
			triggerResponse = false;
			dialogueResponse.TurnOn(hideStats.pConRef.myControl);
		}
	}

	public void EndItemFind(){
		if (itemPopupBG.gameObject.activeSelf){
			StartCoroutine(ItemFindDisable());
		}
	}

	public void EndText(bool newStatOn = true, bool playSound = true){
		if (hideStats){
			hideStats.pConRef.ResetTimeMax();
		}
		//if (!waitingForResponse || (waitingForResponse && !dialogueResponse.getChoiceActive)){
		dialogueText.enabled = false;
		memoBG.enabled = false;
		memoText.enabled = false;
		_textActive = false;
		memoMovie.enabled = false;
        memoMovie.gameObject.SetActive(false);
        memoMovieImage.gameObject.SetActive(false);

		if (itemPopupBG.gameObject.activeSelf){
			StartCoroutine(ItemFindDisable());
		}else{
		itemPopup.enabled = itemPopupBG.enabled = false;
		itemPopupBG.gameObject.SetActive(false);
		}

		//CompleteText();

				isLerpingOut = true;
				isLerping = false;
				showTime = showTimeMax/2f;

		CameraFollowS.F.EndZoom();
		if (statsOn && newStatOn){
			hideStats.gameObject.SetActive(true);
		}
		if (_usingMerchantText){
			_usingMerchantText = false;
			merchantText.text = currentDisplayString = targetDisplayString = "";
			merchantText.enabled = false;
		}else{
		
		dialogueText.text = currentDisplayString = targetDisplayString = "";
		}
        if (playSound)
        {
            Instantiate(dialogueAdvanceSound);
        }
		
		scrollCountdown = 0f;
		currentChar = 0;
		advanceIndicator.SetActive(false);
		_doneScrolling = false;

		//}

	}

	public void FreezeDialogue(){
		_freezeText = true;
		//Debug.LogError("Dialogue freezing");
	}
}
