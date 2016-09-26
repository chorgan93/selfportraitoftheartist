using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCurrencyDisplayS : MonoBehaviour {

	public Text totalDisplay;
	public Text beingAddedDisplay;
	public Image borderDisplay;
	public Image iconDisplay;

	private Color displayColor;

	private float subtractRate = 500f;
	private int currencyDisplayAmt;
	private int currencyTotalAmt;
	private int beingAddedAmt = 0;

	private float fadeMax = 0.8f;

	private float subtractTimerMax = 1f;
	private float subtractTimer;
	private float showTimerMax = 1f;
	private float showTimer;

	private float fadeRateOut = 1.25f;
	private float fadeRateIn = 2.5f;
	private bool fadingIn = false;
	private bool fadingOut = false;

	// Use this for initialization
	void Start () {

		displayColor = totalDisplay.color;
		displayColor.a = 0;
		totalDisplay.color = beingAddedDisplay.color = borderDisplay.color = iconDisplay.color = displayColor;

		totalDisplay.text = "";
		beingAddedDisplay.text = "";
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (fadingOut){
			displayColor = totalDisplay.color;
			displayColor.a -= Time.deltaTime*fadeRateOut;
			if (displayColor.a <= 0){
				displayColor.a = 0;
				fadingOut = false;
			}
			totalDisplay.color = beingAddedDisplay.color = borderDisplay.color = iconDisplay.color = displayColor;
		}
		else if (fadingIn){
			displayColor = totalDisplay.color;
			displayColor.a += Time.deltaTime*fadeRateIn;
			if (displayColor.a >= fadeMax){
				displayColor.a = fadeMax;
				fadingIn = false;
			}
			totalDisplay.color = beingAddedDisplay.color = borderDisplay.color = iconDisplay.color = displayColor;
		}else{
			if (subtractTimer > 0){
				subtractTimer -= Time.deltaTime;
			}else{
				if (beingAddedAmt > 0){
					beingAddedAmt = Mathf.RoundToInt((beingAddedAmt*1f)-Time.deltaTime*subtractRate);
					currencyDisplayAmt = Mathf.RoundToInt((currencyDisplayAmt*1f)+Time.deltaTime*subtractRate);
					showTimer = showTimerMax;
				}else if (showTimer > 0){
					currencyDisplayAmt = currencyTotalAmt;
					beingAddedAmt = 0;
					showTimer -= Time.deltaTime;
				}else{
					fadingOut = true;
				}
			}
		}

		if (totalDisplay.color.a > 0){
			totalDisplay.text = currencyDisplayAmt.ToString();
			beingAddedDisplay.text = "+"+beingAddedAmt;
		}
	
	}

	public void AddCurrency (int currencyToAdd){
		PlayerCollectionS.currencyCollected += currencyToAdd;
		currencyTotalAmt = PlayerCollectionS.currencyCollected;

		if (subtractTimer > 0){
			beingAddedAmt += currencyToAdd;
		}else{
			beingAddedAmt = currencyToAdd;
		}

		subtractTimer = subtractTimerMax;

		if (fadingOut){
			fadingOut = false;
			displayColor = totalDisplay.color;
			displayColor.a = 1f;
			totalDisplay.color = beingAddedDisplay.color = borderDisplay.color = iconDisplay.color = displayColor;
		}else{
			fadingIn = true;
		}
	}
}
