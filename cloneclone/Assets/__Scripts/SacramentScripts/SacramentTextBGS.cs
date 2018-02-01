using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SacramentTextBGS : MonoBehaviour {

	private Image myImage;
	private Color myImageCol;
	private float imageMaxFade;

	public float fadeRate;
	private bool fadingIn = false;
	private bool fadingOut = false;

	private float fadeDelayTime = 0f;

	private bool _initialized = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (fadeDelayTime > 0){
			fadeDelayTime -= Time.deltaTime;
		}
		else if (fadingIn){
			myImageCol.a += fadeRate*Time.deltaTime;
			if (myImageCol.a >= imageMaxFade){
				myImageCol.a = imageMaxFade;
				fadingIn = false;
			}
			myImage.color = myImageCol;
		}
		else if (fadingOut){
			myImageCol.a -= fadeRate*Time.deltaTime;
			if (myImageCol.a <= 0f){
				myImageCol.a = 0f;
				fadingOut = false;
				gameObject.SetActive(false);
			}
			myImage.color = myImageCol;
		}
	
	}

	public void FadeIn(float fadeDelay){
		if (!_initialized){
			Initialize();
		}
		fadeDelayTime = fadeDelay;
		gameObject.SetActive(true);
		fadingOut = false;
		fadingIn = true;
		myImageCol.a = 0f;
		myImage.color = myImageCol;
	}

	public void FadeOut(){
		if (!_initialized){
			Initialize();
		}
		gameObject.SetActive(true);
		fadingIn = false;
		fadingOut = true;
		myImageCol.a = imageMaxFade;
		myImage.color = myImageCol;
	}

	void Initialize(){
		myImage = GetComponent<Image>();
		myImageCol = myImage.color;
		imageMaxFade = myImageCol.a;
	}
}
