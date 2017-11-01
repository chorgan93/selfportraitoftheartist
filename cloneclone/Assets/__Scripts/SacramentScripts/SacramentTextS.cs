using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SacramentTextS : MonoBehaviour {
	

	public enum SacramentTextType {Scrolling, FadeIn, Instant};
	public SacramentTextType textType = SacramentTextType.Scrolling;

	private Text myText;
	private bool _initialized;

	[Header("Appear Properties")]
	public float delayAppearance = 0f;
	private float delayAppearanceCountdown;

	[Header("Fade Properties")]
	public float fadeRate;
	public float maxAlpha = 1f;
	private bool fadingIn = false;
	private Color myCol;


	[Header("Scroll Properties")]
	public float scrollRate;
	private string fullText;
	private string currentText;
	private int currentChar = 0;
	private float scrollCountdown;
	private bool isScrolling = false;

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
	
	}
	
	// Update is called once per frame
	void Update () {

		if (textActive){
			if (delayAppearanceCountdown > 0){
				delayAppearanceCountdown -= Time.deltaTime;
			}else{
				if (!playedAppearSound){
					if (appearSound){
						Instantiate(appearSound);
					}
					playedAppearSound = true;
				}
			if (_readyToAdvance && Input.GetMouseButtonDown(0)){
				_stepRef.AdvanceText();
			}
		if (fadingIn){
				myCol = myText.color;
				myCol.a += fadeRate*Time.deltaTime;
					if (myCol.a >= maxAlpha || Input.GetMouseButtonDown(0)){
					myCol.a = maxAlpha;
					fadingIn = false;
					_readyToAdvance = true;
					_stepRef.myHandler.ActivateWait();
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
						_stepRef.myHandler.ActivateWait();
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

				if (Input.GetMouseButtonDown(0)){
					myText.text = currentText = fullText;
					isScrolling = false;
					_readyToAdvance = true;
					_stepRef.myHandler.ActivateWait();
				}
		}
			}
		}
	
	}

	public void ActivateText(SacramentStepS myStep){
		if (!_initialized){
			_stepRef = myStep;
			myText = GetComponent<Text>();
					fullText = myText.text;
			myCol = myText.color;
			maxAlpha = myCol.a;
			_initialized = true;
		}
		playedAppearSound = false;
		delayAppearanceCountdown = delayAppearance;
		if (textType == SacramentTextType.Instant){
			myText.text = fullText;
			_readyToAdvance = true;
			_stepRef.myHandler.ActivateWait();
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
	}
}
