using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StaminaArrowS : MonoBehaviour {

	private bool doMatch = false;
	public Image matchColor;
	private Image myImage;
	public Sprite[] animFrames;
	private int currentFrame = 0;
	public float animRate = 0.1f;
	private float animRateCountdown;

	// Use this for initialization
	void Start () {

		myImage = GetComponent<Image>();
		currentFrame = Mathf.RoundToInt(Random.Range(0, animFrames.Length-1));
		myImage.sprite = animFrames[currentFrame];

			if (matchColor != null){
				doMatch = true;
			}

		animRateCountdown = animRate;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (doMatch){
			myImage.color = matchColor.color;
		}
	
		animRateCountdown -= Time.deltaTime;
		if (animRateCountdown <= 0){
			animRateCountdown = animRate;
			currentFrame++;
			if (currentFrame > animFrames.Length-1){
				currentFrame = 0;
			}
			myImage.sprite = animFrames[currentFrame];
		}
	}
}
