using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SacramentOptionUndertextS : MonoBehaviour {

	private SacramentOptionS myOption;
	private bool _initialized = false;

	private Text myText;

	public float shakeRate = 0.1f;
	private float shakeCountdown;
	public float shakeOffsetX = 2f;
	public float shakeOffsetY = 1.5f;
	private RectTransform myTransform;
	private Vector2 currentPos;
	private Vector2 startPos;

	private float fadeRate;
	private Color fadeCol;
	private bool fadingIn = false;
	private float maxFade;
	private float delayFadeCountdown;

	private float hoverTimeMult;
	private float hoverXMult;
	private float hoverYMult;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (_initialized){
			if (fadingIn){
				if (delayFadeCountdown > 0){
					delayFadeCountdown -= Time.deltaTime;
				}else{
				fadeCol = myText.color;
				fadeCol.a += fadeRate*Time.deltaTime;
				if (fadeCol.a >= maxFade){
					fadeCol.a = maxFade;
					fadingIn = false;
				}
				myText.color = fadeCol;
				}
			}
			if (myOption.isHovering){
				shakeCountdown -= Time.deltaTime*hoverTimeMult;
				if (shakeCountdown <= 0){
					shakeCountdown = Random.Range(0, shakeRate);
					currentPos = startPos;
					currentPos.x += Random.insideUnitCircle.x*shakeOffsetX*hoverXMult;
					currentPos.y += Random.insideUnitCircle.y*shakeOffsetY*hoverYMult;
					myTransform.anchoredPosition = currentPos;
				}
			}else{
				shakeCountdown -= Time.deltaTime;
				if (shakeCountdown <= 0){
					shakeCountdown = Random.Range(0, shakeRate);
					currentPos = startPos;
					currentPos.x += Random.insideUnitCircle.x*shakeOffsetX;
					currentPos.y += Random.insideUnitCircle.y*shakeOffsetY;
					myTransform.anchoredPosition = currentPos;
				}
			}
		}

	}

	public void ActivateUndertext(SacramentOptionS myO){
		Initialize(myO);
		shakeCountdown = Random.Range(0, shakeRate);
		fadeCol = myText.color;
		fadeCol.a = 0f;
		myText.color = fadeCol;
		delayFadeCountdown = myO.delayFade;
		fadingIn = true;
	}

	public void DeactivateUndertext(){
		myTransform.anchoredPosition = startPos;
	}

	private void Initialize(SacramentOptionS myOpt){
		if (!_initialized){
		myOption = myOpt;
			fadeRate = myOption.fadeRate;
			myText = GetComponent<Text>();
			fadeCol = myText.color;
			maxFade = fadeCol.a;
		_initialized = true;
			myTransform = GetComponent<RectTransform>();
			startPos = currentPos = myTransform.anchoredPosition;
			hoverXMult = myOption.jumpPosXMult;
			hoverYMult = myOption.jumpPosYMult;
			hoverTimeMult = myOption.jumpTimeMult;
		}
	}
}
