using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SacramentTextS : MonoBehaviour {
	

	public enum SacramentTextType {Scrolling, FadeIn, Instant};
	public SacramentTextType textType = SacramentTextType.Scrolling;
    public bool ignoreLocalization = false;

	private Text myText;
	private bool _initialized;

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

	[Header("Legibility Properties")]
	public SacramentTextBGS textBGFadeIn;
	public SacramentTextBGS textBGFadeOut;
	public float delayBGTime;


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

	private SacramentStepS _stepRef;

	[Header ("Sound Properties")]
	public GameObject appearSound;
	private bool playedAppearSound = false;
	public GameObject scrollSound;
	public int scrollSoundTiming = 3;
	private int scrollSoundCountdown;

	// Use this for initialization
	void Start () {
        if (LocalizationManager.currentLanguage == 0) {
            return;
        } 
        RectTransform myrect = GetComponent<RectTransform>();
        float newXSize = myrect.parent.parent.GetComponent<RectTransform>().sizeDelta.x / 2f - myrect.anchoredPosition.x;
        myrect.sizeDelta = new Vector2(newXSize, myrect.sizeDelta.y);
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
				if (_readyToAdvance && (((Input.GetMouseButtonDown(0) || _stepRef.myHandler.TalkButton()) && !disableAdvance) || (useAutoAdvance && autoAdvanceCount <= 0f))){
				_stepRef.AdvanceText();
			}
		if (fadingIn){
				myCol = myText.color;
				myCol.a += fadeRate*Time.deltaTime;
					if (myCol.a >= maxAlpha || Input.GetMouseButtonDown(0) || _stepRef.myHandler.TalkButton()){
					myCol.a = maxAlpha;
					fadingIn = false;
					_readyToAdvance = true;
						if (!disableAdvance){
					_stepRef.myHandler.ActivateWait();
						}
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
							if (!disableAdvance){
						_stepRef.myHandler.ActivateWait();
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

					if (Input.GetMouseButtonDown(0) || _stepRef.myHandler.TalkButton()){
					myText.text = currentText = fullText;
					isScrolling = false;
					_readyToAdvance = true;
						if (!disableAdvance){
					_stepRef.myHandler.ActivateWait();
						}
				}
		}
			}
		}
	
	}

	public void ActivateText(SacramentStepS myStep){
		if (!_initialized){
			_stepRef = myStep;
			myText = GetComponent<Text>();
            if (!ignoreLocalization)
            {
                fullText = LocalizationManager.instance.GetLocalizedValue(myText.text).Replace("\\n", System.Environment.NewLine)
                                              .Replace("PLAYERNAME", TextInputUIS.playerName);
            }else{
                fullText = myText.text;
            }
			myCol = myText.color;
			maxAlpha = myCol.a;
			if (autoAdvanceTime > 0){
				useAutoAdvance = true;
			}
			_initialized = true;
		}
		if (useAutoAdvance){
			autoAdvanceCount = autoAdvanceTime;
		}
		if (textBGFadeIn){
			textBGFadeIn.FadeIn(delayBGTime);
		}
		playedAppearSound = false;
		delayAppearanceCountdown = delayAppearance;
		if (textType == SacramentTextType.Instant){
			myText.text = fullText;
			_readyToAdvance = true;
			if (!disableAdvance){
			_stepRef.myHandler.ActivateWait();
			}
		}else if (textType == SacramentTextType.FadeIn){
			myText.text = fullText;
			myCol.a = 0f;
			myText.color = myCol;
			fadingIn = true;
		}else{
			myText.text = "";
			currentText = "";
			currentChar = -1;
			scrollCountdown = scrollRate;
			isScrolling = true;
		}
		textActive = true;
		gameObject.SetActive(true);
		
	}
	public void DeactivateText(){
					textActive = false;
		_readyToAdvance = false;
		_stepRef.myHandler.DeactivateWait();
		if (textBGFadeOut){
			textBGFadeOut.FadeOut();
		}
	}
}
