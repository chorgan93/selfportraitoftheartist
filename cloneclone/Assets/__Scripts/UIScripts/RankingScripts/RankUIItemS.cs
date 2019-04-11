using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RankUIItemS : MonoBehaviour {

	[Header("UI Assignments")]
	public Sprite[] scoreType;
	private int currentScoreType = 0;
	public Text scoreAmt;
	public Image scoreTypeImage;
	public Image[] scoreRenders;
	private List<float> scoreMaxAlphas;

	[Header("Special Properties")]
	public bool isScoreAdd = false;
	public bool isBonusAdd = false;

	[Header("Color Properties")]
	public float fadeTime = 0.4f;
	private float fadeCount;
	private float fadeT;
	private Color fadeColor;
	private float currentFadeAmt = 1f;

	//____________________MOVEMENT PROPERTIES
	private Vector2 currentAnchoredTarget;
	private Vector2 prevAnchoredPosition;
	private RectTransform myTransform;
	public RectTransform rectTransform { get { return myTransform; } }
	private float moveT;

	[Header("Movement Properties")]
	public float moveTimeMax = 0.5f;
	public float startXOffset;
	private float moveTimeCount;
	private bool moving = false;
	private bool fadingIn = false;
	private bool fadingOut = false;
	private float delayMoveCount = 0f;

	private RankUIS myUIManager;

	private bool _initialized = false;

    bool bonusLocalized = false;

	
	// Update is called once per frame
	void Update () {

		if (delayMoveCount > 0){
            delayMoveCount -= Time.deltaTime*myUIManager.SpeedUpMultiplier();
		}else{
		if (fadingIn || fadingOut){
                fadeCount += Time.deltaTime* myUIManager.SpeedUpMultiplier();
			if (fadeCount >= fadeTime){
				fadeCount = fadeTime;
			}
			fadeT = fadeCount/fadeTime;
			fadeT = Mathf.Sin(fadeT * Mathf.PI * 0.5f);
			for (int i = 0; i < scoreRenders.Length; i++){
				fadeColor = scoreRenders[i].color;
				if (fadingIn){
					fadeColor.a = scoreMaxAlphas[i]*fadeT;
				}else if (fadingOut){
						fadeColor.a = scoreMaxAlphas[i]*(1f-fadeT);
				}
				scoreRenders[i].color = fadeColor;
			}
			fadeColor = scoreAmt.color;
			if (fadingIn){
				fadeColor.a = fadeT;
			}else if (fadingOut){
					fadeColor.a = 1f*(1f-fadeT);
			}
			if (!isScoreAdd){
				scoreTypeImage.color = fadeColor;
			}
			scoreAmt.color = fadeColor;
			if (fadeT >= 1f){
				fadingIn = false;
				if (fadingOut){
				fadingOut = false;
					TurnOff();
				}
			}
		}

		if (moving){
			
                moveTimeCount += Time.deltaTime* myUIManager.SpeedUpMultiplier();
			if (moveTimeCount >= moveTimeMax){
				moveTimeCount = moveTimeMax;
				moving = false;
			}
			moveT = moveTimeCount/moveTimeMax;
			moveT = Mathf.Sin(moveT * Mathf.PI * 0.5f);
			myTransform.anchoredPosition = Vector2.Lerp(prevAnchoredPosition, currentAnchoredTarget, moveT);
			}

		}
	
	
	}

	public void TurnOn(int scoreT, float scoreAmount, RankUIS myManager){
		if (!_initialized){
			myUIManager = myManager;
			Initialize();
		}
		if (!isScoreAdd){
			currentScoreType = scoreT;
		scoreTypeImage.sprite = scoreType[currentScoreType];
		fadeColor = scoreTypeImage.color;
		fadeColor.a = 0f;
		scoreTypeImage.color = fadeColor;
		}
		if (!isBonusAdd){
		if (isScoreAdd){
				scoreAmt.text = "+ " + scoreAmount.ToString();
		}else{
		scoreAmt.text = scoreAmount.ToString();
		}
        }else if (!bonusLocalized)
            {
                scoreAmt.text = LocalizationManager.instance.GetLocalizedValue(scoreAmt.text).Replace("{S}", scoreAmount.ToString());
                bonusLocalized = true;
            
        }
		fadeColor = scoreAmt.color;
		fadeColor.a = 0f;
		scoreAmt.color = fadeColor;
		scoreAmt.enabled = true;
		gameObject.SetActive(true);

	}

	public void MoveTo(Vector2 newTarget, float newMoveTime = 0.5f){
		moveTimeMax = newMoveTime;
		moveTimeCount = 0f;
		prevAnchoredPosition = myTransform.anchoredPosition;
		currentAnchoredTarget = newTarget;
		moving = true;
	}

	public void SetPosition(Vector2 newPos){
		moving = false;
		moveTimeCount = 0f;
		myTransform.anchoredPosition = prevAnchoredPosition = newPos;

	}

	public void EndDelay(){
		delayMoveCount = 0f;
	}

	void Initialize(){

		if (!_initialized){
			scoreMaxAlphas = new List<float>();
		for (int i = 0; i < scoreRenders.Length; i++){
			scoreMaxAlphas.Add(scoreRenders[i].color.a);
			fadeColor = scoreRenders[i].color;
			fadeColor.a = 0f;
			scoreRenders[i].color = fadeColor;
		}
			myTransform = GetComponent<RectTransform>();
			_initialized = true;
		}

	}

	public void TurnOff(){
		gameObject.SetActive(false);
		if (!isScoreAdd){
		myUIManager.AddOffItem(this);
		}
	}

	public void SetFade(bool doFadeIn, bool doFadeOut){
		fadingIn = doFadeIn;
		fadingOut = doFadeOut;
		fadeCount = 0f;
	}

	public void SetMaxAlpha(){
		for (int i = 0; i < scoreRenders.Length; i++){
			
			fadeColor.a = scoreMaxAlphas[i];
			
			scoreRenders[i].color = fadeColor;
		}
		fadeColor = scoreAmt.color;
			fadeColor.a = 1f;
		if (!isScoreAdd){
		scoreAmt.color = scoreTypeImage.color = fadeColor;
		}else{
			scoreAmt.color = fadeColor;
		}
	}

	public void SetNewPos(bool doFadeIn, bool doFadeOut, Vector2 newPos, float delay = 0f){
		fadingIn = doFadeIn;
		fadingOut = doFadeOut;
		fadeCount = 0f;
		delayMoveCount = delay;
		if (fadingOut){
			MoveTo(newPos, 0.6f);
		}else{
			MoveTo(newPos);
		}
	}
}
