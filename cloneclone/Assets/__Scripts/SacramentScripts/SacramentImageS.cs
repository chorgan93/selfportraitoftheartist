using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SacramentImageS : MonoBehaviour {

	[Header("Appear Properties")]
	public float fadeInRate = -1f;
	private bool fadingIn = false;
	private bool imageFadesIn = false;
	public float fadeOutRate = -1f;
	private bool fadingOut = false;
	private bool imageFadesOut = false;
	public float showTime = 3f;
	private float showCountdown;
	private bool waitForClick = false;

	public Image myImage;
	private Color myCol;
	private float maxAlpha;

	[Header("Effect Properties")]
	public Image distortion;
	public float distortionRate = 0.1f;
	private float distortionCountdown = 0f;
	private RectTransform distortionTransform;
	private Vector2 distortionSize;
	private Vector2 distortionStartSize;
	public float distortionSizeMult = 0.2f;

	private SacramentStepS _myStep;
	private bool _initialized = false;
	private bool imageActive = false;

	private bool showedWaitSymbol = false;

	[Header("Sound Properties")]
	public GameObject onSound;
	public GameObject offSound;

	[Header("Conditional Images")]
	public Sprite[] conditionalImages;
	private int numTimesSeen = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (imageActive){
			DistortImage();
			if (imageFadesIn && fadingIn){
				myCol = myImage.color;
				myCol.a += fadeInRate*Time.deltaTime;
				if (myCol.a >= maxAlpha){
					myCol.a = maxAlpha;
					fadingIn = false;
				}
				myImage.color = distortion.color = myCol;
			}
			else if (showCountdown > 0){
				showCountdown -= Time.deltaTime;
			}else if (imageFadesOut && fadingOut){
				myCol = myImage.color;
				myCol.a -= fadeOutRate*Time.deltaTime;
				if (myCol.a <= 0){
					myCol.a = 0;
					fadingOut = false;
				}
				myImage.color = distortion.color = myCol;
			}else if (waitForClick){
				if (!showedWaitSymbol){
					showedWaitSymbol = true;
					_myStep.myHandler.ActivateWait();
				}
				if (Input.GetMouseButtonDown(0) || _myStep.myHandler.myControl.TalkButton()){
					_myStep.AdvanceImage();
				}
			}else{
				_myStep.AdvanceImage();
			}
		}
	
	}

	void DistortImage(){
		if (distortion){
			distortionCountdown -= Time.deltaTime;
			if (distortionCountdown <= 0){
				distortionCountdown = distortionRate;
				distortionSize = distortionStartSize+Random.insideUnitCircle*distortionSizeMult;
				distortionTransform.sizeDelta = distortionSize;
			}
		}
	}

	public void ActivateImage(SacramentStepS newStep){
		if (!_initialized){
			_myStep = newStep;
			_initialized = true;
			numTimesSeen = 0;

			if (fadeInRate > 0){
				imageFadesIn = true;
			}
			if (fadeOutRate > 0){
				imageFadesOut = true;
			}else{
				waitForClick = true;
			}

			myCol = myImage.color;
			maxAlpha = myCol.a;
			if (distortion){
				distortionTransform = distortion.GetComponent<RectTransform>();
				distortionStartSize = distortionTransform.sizeDelta;
			}
		}

        if (conditionalImages != null)
        {
            if (conditionalImages.Length > 0)
            {
                if (numTimesSeen < conditionalImages.Length)
                {
                    myImage.sprite = distortion.sprite = conditionalImages[numTimesSeen];
                    numTimesSeen++;
                }
            }
        }

		if (imageFadesIn){
			myCol = myImage.color;
			myCol.a = 0;
			fadingIn = true;
		}else{
			myCol = myImage.color;
			myCol.a = maxAlpha;
			fadingOut = true;
		}
		myImage.color = distortion.color = myCol;
		showCountdown = showTime;
		showedWaitSymbol = false;
		gameObject.SetActive(true);
		imageActive = true;

		if (distortion){
			distortion.gameObject.SetActive(true);
		}

		if (onSound){
			Instantiate(onSound);
		}
	}
	public void DeactivateImage(){
		gameObject.SetActive(false);
		_myStep.myHandler.DeactivateWait();
		if (distortion){
			distortion.gameObject.SetActive(false);
		}
		imageActive = false;
		if (offSound){
			Instantiate(offSound);
		}
	}
	public void Hide(){
		gameObject.SetActive(false);
		if (distortion){
			distortion.gameObject.SetActive(false);
		}
	}
}
