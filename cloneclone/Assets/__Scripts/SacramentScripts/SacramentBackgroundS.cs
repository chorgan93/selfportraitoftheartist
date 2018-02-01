using UnityEngine;
using System.Collections;

public class SacramentBackgroundS : MonoBehaviour {

	[Header("Appear Properties")]
	public float fadeInRate = -1f;
	private bool fadingIn = false;
	private bool imageFadesIn = false;
	public float fadeOutRate = -1f;
	private bool fadingOut = false;
	private bool imageFadesOut = false;
	public float showTime = 3f;
	private float showCountdown;

	private SpriteRenderer myImage;
	private Color myCol;
	private float maxAlpha;

	private bool _initialized = false;
	private bool imageActive = false;

	private bool showedWaitSymbol = false;

	[Header("Legibility Properties")]
	public SacramentTextBGS textBG;
	public float delayBGTime = 0f;


	[Header("Sound Properties")]
	public GameObject onSound;
	public GameObject offSound;


	
	// Update is called once per frame
	void Update () {

		if (imageActive){
			if (imageFadesIn && fadingIn){
				myCol = myImage.color;
				myCol.a += fadeInRate*Time.deltaTime;
				if (myCol.a >= maxAlpha){
					myCol.a = maxAlpha;
					fadingIn = false;
				}
				myImage.color =  myCol;
			}else if (imageFadesOut && fadingOut){
				myCol = myImage.color;
				myCol.a -= fadeOutRate*Time.deltaTime;
				if (myCol.a <= 0){
					myCol.a = 0;
					fadingOut = false;
					DeactivateImage();
				}
				myImage.color  = myCol;
			}
		}
	
	}



	public void ActivateImage(){
		if (!_initialized){
			_initialized = true;
			myImage = GetComponent<SpriteRenderer>();

			if (fadeInRate > 0){
				imageFadesIn = true;
			}
			if (fadeOutRate > 0){
				imageFadesOut = true;
			}

			myCol = myImage.color;
			maxAlpha = myCol.a;
		}
		if (imageFadesIn){
			myCol = myImage.color;
			myCol.a = 0;
			fadingIn = true;
		}else{
			myCol = myImage.color;
			myCol.a = maxAlpha;
			//fadingOut = true;
		}
		myImage.color =  myCol;
		showCountdown = showTime;
		showedWaitSymbol = false;
		gameObject.SetActive(true);
		imageActive = true;

		if (textBG){
			textBG.FadeIn(delayBGTime);
		}

		if (onSound){
			Instantiate(onSound);
		}
	}
	public void ActivateBackground(){
		ActivateImage();
	}
	public void DeactivateBackground(){
		fadingOut = true;
		imageFadesOut= true;
		if (textBG){
			textBG.FadeOut();
		}
	}
	public void DeactivateImage(){
		gameObject.SetActive(false);

		imageActive = false;
		if (offSound){
			Instantiate(offSound);
		}
	}
	public void Hide(){
		gameObject.SetActive(false);

	}
}
