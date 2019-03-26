using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WarningManagerS : MonoBehaviour {

	public Text warningText;
	public Text warningTextBg;
	public Text warningTextShake;
	public Color resetGreen;

	public Image bg;
	private Color bgCol = new Color(0,0,0,0.66f);

	private Vector2 bgOffset;

	private Color fadeCol;
	private float currentFade;

	public float fadeRate = 1f;
	public float showTime = 2f;
	private float currentShowTime;
	private bool isShowing = false;

	private float shakeTimeMax;
	private float shakeTimeCountdown;
	private float shakeIntensity;
	private bool isShaking;

	[Header("Shake Effect Intensity")]
	public float shakeIntensityLow = 0.5f;
	public float shakeIntensityMid = 1;
	public float shakeIntensityHigh = 2;


	[Header("Shake Effect Timings")]
	public float shakeDurationLow = 0.2f;
	public float shakeDurationMid = 0.3f;
	public float shakeDurationHigh = 0.5f;

	private float altPosCountdown = 0.08f;
	private float altPosCountdownMax = 0.08f;
	private float altPosRange = 0.2f;

	Vector2 shakePos;
	bool constantShow = false;



	// Use this for initialization
	void Start () {

		warningText.text = warningTextShake.text = warningTextBg.text = "";
		warningText.enabled = warningTextShake.enabled = warningTextBg.enabled = false;
		bg.enabled = false;

		bgOffset = warningTextBg.rectTransform.anchoredPosition;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (isShowing){
			if (currentShowTime > 0){
				if (!constantShow){
				currentShowTime -= Time.deltaTime;
				}
				HandleShake();
			}else{
				currentFade = warningText.color.a;
				currentFade -= fadeRate*Time.deltaTime;
				if (currentFade <= 0){
					isShowing = false;
					warningText.text = warningTextShake.text = warningTextBg.text = "";
					warningText.enabled = warningTextShake.enabled = warningTextBg.enabled = bg.enabled = false;
				}else{
					fadeCol = bg.color;
					fadeCol.a = currentFade*bgCol.a;
					bg.color = fadeCol;
					fadeCol = warningText.color;
					fadeCol.a = currentFade;
					warningText.color = warningTextShake.color = fadeCol;
					fadeCol = warningTextBg.color;
					fadeCol.a = currentFade;
					warningTextBg.color = fadeCol;
					fadeCol = bg.color;
				}
			}
		}
	
	}

	void HandleShake(){

		if (isShaking){
			
			shakeTimeCountdown -= Time.deltaTime;
			if (shakeTimeCountdown <= 0){
				shakeTimeCountdown = 0;
				isShaking = false;
			}

			shakePos = bgOffset+Random.insideUnitCircle*shakeIntensity*shakeTimeCountdown/shakeTimeMax;
			if (shakeTimeCountdown > 0){
				shakePos.y/=2f;
				shakePos.x*=8f;
			}
			warningTextBg.rectTransform.anchoredPosition = shakePos;
				
			shakePos = Random.insideUnitCircle*shakeIntensity*shakeTimeCountdown/shakeTimeMax;
			if (shakeTimeCountdown > 0){
				shakePos.y/=2f;
				shakePos.x*=8f;
			}
			warningTextShake.rectTransform.anchoredPosition = shakePos;
		}
		
	}

	public void NewMessage(string newMessage, Color mainCol, Color subCol, bool lockShow, int shakeInt  = 0){
				
        warningText.text = warningTextBg.text = warningTextShake.text = LocalizationManager.instance.GetLocalizedValue(newMessage);
		warningText.color = warningTextShake.color = mainCol;
		warningTextBg.color = subCol;
		bg.color = bgCol;
		warningText.enabled = warningTextBg.enabled = warningTextShake.enabled = bg.enabled = true;

				isShaking = isShowing = true;
		currentShowTime = showTime;
		constantShow = lockShow;

		if (shakeInt == 2){
			shakeTimeCountdown = shakeTimeMax = shakeDurationHigh;
			shakeIntensity = shakeIntensityHigh;
		}
		else if (shakeInt == 1){
			shakeTimeCountdown = shakeTimeMax = shakeDurationMid;
			shakeIntensity = shakeIntensityMid;
		}
		else {
			shakeTimeCountdown = shakeTimeMax = shakeDurationLow;
			shakeIntensity = shakeIntensityLow;
		}
	}

	public void EndShow(string warningString){
        if (LocalizationManager.instance.GetLocalizedValue(warningString) == warningText.text){
			isShaking = false;
			currentShowTime = 0f;
			warningTextBg.rectTransform.anchoredPosition = shakePos;
			warningTextShake.rectTransform.anchoredPosition = Vector2.zero;
		}
	}

	public void EndAll(){
		isShaking = false;
		currentShowTime = 0f;
		warningTextBg.rectTransform.anchoredPosition = shakePos;
		warningTextShake.rectTransform.anchoredPosition = Vector2.zero;
	}
}
