using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChargeUseUIS : MonoBehaviour {

	private float fadeRate = 2f;
	private float delayFade = 0.3f;
	private float delayFadeCount;

	private Color fadeCol;
	private float startAlpha;

	private float dropRate = 50f;
	private float currentDropRate;
	private float dropAccel = 20f;
	private Vector2 dropPos;
	private Vector2 startPos;
	private Vector2 itemSize;

	public Image itemImage;
	public Image gradientImage;

	private bool isShowing = false;

	// Use this for initialization
	void Start () {

		itemImage.enabled = false;
		gradientImage.enabled = false;

		fadeCol = itemImage.color;
		startAlpha = fadeCol.a;

		startPos = dropPos = itemImage.rectTransform.anchoredPosition;
		itemSize = itemImage.rectTransform.sizeDelta;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (isShowing){
			if (delayFadeCount > 0){
				delayFadeCount -= Time.deltaTime;
			}else{
				fadeCol = gradientImage.color;
				fadeCol.a -= fadeRate*Time.deltaTime;
				if (fadeCol.a <= 0){
					fadeCol.a = 0;
					isShowing = false;
					itemImage.enabled = false;
					gradientImage.enabled = false;
				}
				gradientImage.color = fadeCol;

				fadeCol = itemImage.color;
				fadeCol.a = gradientImage.color.a*0.5f;
				itemImage.color = fadeCol;
			}

			dropPos = itemImage.rectTransform.anchoredPosition;
			dropPos.y -= currentDropRate*Time.deltaTime;
			itemImage.rectTransform.anchoredPosition = dropPos;

			currentDropRate -= dropAccel*Time.deltaTime;
		}
	
	}

	public void StartUse(float newWidth, float newX){
		
		fadeCol = gradientImage.color;
		fadeCol.a = startAlpha;
		gradientImage.color = fadeCol;
		fadeCol = itemImage.color;
		fadeCol.a = startAlpha*0.5f;
		itemImage.color = fadeCol;

		startPos.x = newX;
		if (startPos.x < 0){
			startPos.x = 0;
		}

		itemImage.rectTransform.anchoredPosition = dropPos = startPos;

		itemSize.x = newWidth;
		itemImage.rectTransform.sizeDelta = itemSize;

		itemImage.enabled = true;
		gradientImage.enabled = true;

		delayFadeCount = delayFade;
		currentDropRate = dropRate;
		isShowing = true;

	}
}
