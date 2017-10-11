using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RankUIItemS : MonoBehaviour {

	[Header("UI Assignments")]
	public Image[] scoreType;
	private int currentScoreType = 0;
	public Text scoreAmt;
	public Image[] scoreRenders;
	private List<float> scoreMaxAlphas;

	private Color fadeColor;
	private float currentFadeAmt = 1f;

	//____________________MOVEMENT PROPERTIES
	private Vector2 currentAnchoredTarget;
	private Vector2 prevAnchoredPosition;
	private float moveT;

	[Header("Movement Properties")]
	public float moveTimeMax = 0.5f;
	public float startXOffset;
	private float moveTimeCount;
	private bool moving = false;
	private bool fadingIn = false;
	private bool fadingOut = false;

	private bool _initialized = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TurnOn(int scoreT, float scoreAmount){
		if (!_initialized){
			Initialize();
		}
		for (int i = 0; i < scoreType.Length; i++){
			if (i == scoreT){
				scoreType[i].enabled = true;
				currentScoreType = i;
			}else{
				scoreType[i].enabled = false;
			}
		}
		fadeColor = scoreType[currentScoreType].color;
		fadeColor.a = 0f;
		scoreType[currentScoreType].color = fadeColor;
		scoreAmt.text = scoreAmount.ToString();
		scoreAmt.color = fadeColor;
		scoreAmt.enabled = true;
		
	}

	void Initialize(){

		scoreMaxAlphas.Clear();
		for (int i = 0; i < scoreRenders.Length; i++){
			scoreMaxAlphas.Add(scoreRenders[i].color.a);
			fadeColor = scoreRenders[i].color;
			fadeColor.a = 0f;
			scoreRenders[i].color = fadeColor;
		}

	}

	public void TurnOff(){
		
	}

	public void SetNewPos(bool doFadeIn, bool doFadeOut){
		fadingIn = doFadeIn;
		fadingOut = doFadeOut;
	}
}
