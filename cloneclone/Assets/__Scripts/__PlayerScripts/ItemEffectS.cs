using UnityEngine;
using System.Collections;

public class ItemEffectS : MonoBehaviour {

	private SpriteRenderer myRender;
	private Animator myAnimator;
	public SpriteRenderer circleRender;

	private float fadeRate = 1f;
	public float maxFade = 0.5f;
	public float circleTimeMult = 0.5f;
	private Color fadeColor;

	public float showTimeMax = 1f;
	private float showTime;
	private bool showing = false;

	// Use this for initialization
	void Start () {

		myRender = GetComponent<SpriteRenderer>();
		myAnimator = GetComponent<Animator>();
		myRender.enabled = false;
		circleRender.enabled = false;
		fadeRate = maxFade/showTimeMax*circleTimeMult;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (showing){
			if (circleRender.color == Color.white){
				circleRender.color = fadeColor;
			}
			fadeColor = circleRender.color;
			fadeColor.a -= Time.deltaTime*fadeRate;
			if (fadeColor.a <= 0){
				fadeColor.a = 0;
			}
			circleRender.color = fadeColor;
			showTime -= Time.deltaTime;
			if (showTime <= 0){
				showing = false;
				myRender.enabled = false;
				circleRender.enabled = false;
			}
		}
	
	}

	public void Flash(Color newCol){
		fadeColor = newCol;
		fadeColor.a = maxFade;
		myRender.color = fadeColor;
		circleRender.color = Color.white;
		showing = true;
		showTime = showTimeMax;
		myRender.enabled = true;
		circleRender.enabled = true;
		myAnimator.SetTrigger("Use");
	}
}
