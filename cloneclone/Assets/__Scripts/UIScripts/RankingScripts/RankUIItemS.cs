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
	private float moveT;

	[Header("Movement Properties")]
	public float moveTimeMax = 0.5f;
	public float startXOffset;
	private float moveTimeCount;
	private bool moving = false;
	private bool fadingIn = false;
	private bool fadingOut = false;

	private bool _initialized = false;

	
	// Update is called once per frame
	void Update () {

		if (fadingIn || fadingOut){
			fadeCount += Time.deltaTime;
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
					fadeColor.a = scoreMaxAlphas[i]/fadeT;
				}
				scoreRenders[i].color = fadeColor;
			}
			fadeColor = scoreAmt.color;
			if (fadingIn){
				fadeColor.a = fadeT;
			}else if (fadingOut){
				fadeColor.a = 1f/fadeT;
			}
			if (fadeT >= 1f){
				fadingIn = false;
				fadingOut = false;
			}
		}

		if (moving){
			moveTimeCount += Time.deltaTime;
			if (moveTimeCount >= 0){
				moveTimeCount = moveTimeMax;
				moving = false;
			}
			moveT = moveTimeCount/moveTimeMax;
			moveT = Mathf.Sin(moveT * Mathf.PI * 0.5f);
			myTransform.anchoredPosition = Vector2.Lerp(prevAnchoredPosition, currentAnchoredTarget, moveT);

		}
	
	}

	public void TurnOn(int scoreT, float scoreAmount){
		if (!_initialized){
			Initialize();
		}
		if (!isScoreAdd){
			currentScoreType = scoreT;
		scoreTypeImage.sprite = scoreType[currentScoreType];
		fadeColor = scoreTypeImage.color;
		fadeColor.a = 0f;
		scoreTypeImage.color = fadeColor;
		}else{
			scoreTypeImage.enabled = false;
		}
		if (isScoreAdd){
			scoreAmt.text = "+ " + scoreAmount.ToString();
		}else{
		scoreAmt.text = scoreAmount.ToString();
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
		myTransform.anchoredPosition = prevAnchoredPosition = newPos;

	}

	void Initialize(){

		if (!_initialized){
		scoreMaxAlphas.Clear();
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
	}

	public void SetNewPos(bool doFadeIn, bool doFadeOut){
		fadingIn = doFadeIn;
		fadingOut = doFadeOut;
		fadeCount = 0f;
	}
}
