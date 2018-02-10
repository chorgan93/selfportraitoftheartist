using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SacramentOptionS : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	private bool _initialized = false;
	private SacramentHandlerS myHandler;

	[Header("Visual Properties")]
	public float fadeRate = 1f;
	public float delayFade = 0f;
	private float delayFadeCountdown;
	private Color fadeCol;
	public Text mainText;
	private float maxFade;
	private bool fadingIn = false;
	public SacramentOptionUndertextS[] textJumps;
	public float jumpPosXMult = 3f;
	public float jumpPosYMult = 2f;
	public float jumpTimeMult = 2.5f;


	[Header("Navigation Properties")]
	public int limitedOption = -1;
	private bool isLimited = false;
	public SacramentStepS[] possNextSteps;
	private int numTimesChosen = 0;

	[Header("Sound Properties")]
	public GameObject hoverSound;
	public GameObject selectSound;

	[Header("Web Properties")]
	public string URLoption = "";
	private bool isURL = false;

	private bool _isHovering = false;
	public bool isHovering {get { return _isHovering; } }

	private bool optionActive = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		// change for controller once non-standalone
		if (optionActive && _initialized){
			if (fadingIn){
				if (delayFadeCountdown > 0){
					delayFadeCountdown -= Time.deltaTime;
				}else{
				fadeCol = mainText.color;
				fadeCol.a += fadeRate*Time.deltaTime;
				if (fadeCol.a >= maxFade){
					fadeCol.a = maxFade;
					fadingIn = false;
				}
				mainText.color = fadeCol;
				}
			}
			else if (Input.GetMouseButtonDown(0) && _isHovering){
				SelectOption();
			}
		}
	
	}

	public void Initialize(SacramentHandlerS newH){
		if (!_initialized){
			myHandler = newH;
			_initialized = true;
			if (limitedOption > 0){
				isLimited = true;
			}
			fadeCol = mainText.color;
			maxFade = fadeCol.a;

			if (URLoption != ""){
				isURL = true;
			}
		}
		ActivateOption();
	}

	public void ActivateOption(){

		if (isLimited && limitedOption <= 0){
			DeactivateOption();
		}else{
		for (int i = 0; i < textJumps.Length; i++){
			textJumps[i].ActivateUndertext(this);
		}
					optionActive = true;
			delayFadeCountdown = delayFade;

			fadeCol = mainText.color;
			fadeCol.a = 0f;
			mainText.color = fadeCol;
			fadingIn = true;
			gameObject.SetActive(true);
		}

	}
	public void DeactivateOption(){
		for (int i = 0; i < textJumps.Length; i++){
			textJumps[i].DeactivateUndertext();
		}
		optionActive = false;
		_isHovering = false;
		gameObject.SetActive(false);
	}

	public void Hide(){
		gameObject.SetActive(false);
	}

	public void SelectOption(){
		if (isURL){
			Application.OpenURL(URLoption);
		}else{
		if (isLimited){
			limitedOption--;
		}
		if (selectSound){
			Instantiate(selectSound);
		}
		_isHovering = false;
			myHandler.GoToStep(ChooseNextStep());
			numTimesChosen++;
		}
	}

	SacramentStepS ChooseNextStep(){
		if (possNextSteps.Length > numTimesChosen){
			return possNextSteps[numTimesChosen];
		} else{
			return possNextSteps[possNextSteps.Length-1];
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
			_isHovering = true;

	}

	public void OnPointerExit(PointerEventData eventData)
	{
			_isHovering = false;
		
	}
}
