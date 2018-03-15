using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SacramentCombatActionOptionS : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {


	[Header("Visual Properties")]
	public float fadeRate = 1f;
	public float delayFade = 0f;
	private float delayFadeCountdown;
	private Color fadeCol;
	public Text mainText;
	private float maxFade;
	private bool fadingIn = false;
	public SacramentCombatOptionUndertextS[] textJumps;
	public float jumpPosXMult = 3f;
	public float jumpPosYMult = 2f;
	public float jumpTimeMult = 2.5f;

	private SacramentCombatantS myHandler;


	[Header("Navigation Properties")]
	public int actionOption = 0;
	private bool _initialized;

	[Header("Sound Properties")]
	public GameObject hoverSound;
	public GameObject selectSound;

	private bool _isHovering = false;
	public bool isHovering {get { return _isHovering; } }

	private bool optionActive = false;
	public bool canBeSelected { get { return optionActive; } }

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
			else if (_isHovering){
				if ((myHandler.myManager.myStep.myHandler.usingMouse && Input.GetMouseButtonDown(0)) 
					|| (myHandler.myManager.myStep.myHandler.TalkButton() && !myHandler.myManager.myStep.myHandler.usingMouse)){ 
					SelectOption();
				}
			}
		}
	
	}

	public void Initialize(SacramentCombatantS newH){
		if (!_initialized){
			myHandler = newH;
			_initialized = true;
			fadeCol = mainText.color;
			maxFade = fadeCol.a;
		}
		ActivateOption();
	}

	public void ActivateOption(){


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
		if (selectSound){
			Instantiate(selectSound);
		}
		_isHovering = false;
		myHandler.SelectAction(actionOption);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (myHandler.myManager.myStep.myHandler.usingMouse){
			_isHovering = true;
		}

	}

	public void OnPointerExit(PointerEventData eventData)
	{

		if (myHandler.myManager.myStep.myHandler.usingMouse){
			_isHovering = false;
		}

	}

	public void StartHover(){
		myHandler.EndHovering();
		_isHovering = true;
		myHandler.SetOptionMark(mainText.rectTransform.anchoredPosition, mainText);
		//Debug.Log("I should be selectable!! " + gameObject.name); 
	}
	public void EndHover(){
		_isHovering = false;
		//Debug.Log("I am no longer selectable!! " + gameObject.name); 
	}
}
