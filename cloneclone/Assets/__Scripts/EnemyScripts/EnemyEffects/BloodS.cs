using UnityEngine;
using System.Collections;

public class BloodS : MonoBehaviour {

	public Color startColor;
	public Color[] bloodColors;
	public Sprite[] bloodSprites;

	public float waitToAppearTime = 0.02f;
	private float waitToAppearCountdown;

	public float bloodAnimRate = 0.083f;
	private float bloodAnimCountdown;
	private int currentSprite;

	public float bloodColorRate = 0.083f;
	private float bloodColorCountdown;
	private int currentColor;

	public int startFlashAmt = 6;
	private int currentFlashFrame = 0;

	private SpriteRenderer myRenderer;
	private bool initialized = false;

	// Use this for initialization
	void Start () {

		Initialize();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (waitToAppearCountdown > 0){
			waitToAppearCountdown -= Time.deltaTime;
		}
		else if (currentFlashFrame > 0){
			currentFlashFrame--;
			if (!myRenderer.enabled){
				myRenderer.enabled = true;
			}
		}else{
			if (currentSprite < bloodSprites.Length-1){
				bloodAnimCountdown -= Time.deltaTime;
				if (bloodAnimCountdown <= 0){
					bloodAnimCountdown = bloodAnimRate;
					currentSprite++;
					myRenderer.sprite = bloodSprites[currentSprite];
				}
			}

			if (currentColor < bloodColors.Length-1){
				bloodColorCountdown -= Time.deltaTime;
				if (bloodColorCountdown <= 0){
					bloodColorCountdown = bloodColorRate;
					currentColor++;
					myRenderer.color = bloodColors[currentColor];
				}                                       
			}
		}
	
	}

	void Initialize(){

		if (!initialized){

			myRenderer = GetComponent<SpriteRenderer>();
			myRenderer.enabled = false;
			myRenderer.color = startColor;
			myRenderer.sprite = bloodSprites[0];

			bloodAnimCountdown = 0;
			currentSprite = 0;
			bloodColorCountdown = 0;
			currentColor = 0;

			waitToAppearCountdown = waitToAppearTime;
			currentFlashFrame = startFlashAmt;

			initialized = true;
		}

	}

	public void AddWaitTime (float f){
		waitToAppearTime += f;
	}
}
