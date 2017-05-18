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

	public static bool CanGetXP = true;

	private float fadeMax = 0.8f;

	private float subtractTimerMax = 1f;
	private float subtractTimer;
	private float showTimerMax = 1f;
	private float showTimer;

	private float fadeRateOut = 1.25f;
	private float fadeRateIn = 2.5f;
	private bool fadingIn = false;
	private bool fadingOut = false;

	private bool showing = false;
	private bool _isHiding = false;

	// Use this for initialization
	void Start () {

		displayColor = totalDisplay.color;
		displayColor.a = 0;
		beingAddedDisplay.color = displayColor;

		currencyDisplayAmt = currencyTotalAmt = PlayerCollectionS.currencyCollected;
		totalDisplay.text = currencyDisplayAmt.ToString();
		beingAddedDisplay.text = "";

		if (PlayerController.equippedUpgrades.Contains(0)){
			Show();
		}else{
			Hide ();
		}
	
	}

	void Update(){
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Equals)){
			AddCurrency(1000);
		}
		#endif
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (fadingOut && !showing){
			displayColor = beingAddedDisplay.color;
			displayColor.a -= Time.deltaTime*fadeRateOut;
			if (displayColor.a <= 0){
				displayColor.a = 0;
				fadingOut = false;
			}
			beingAddedDisplay.color = displayColor;
			//displayColor.a *= 0.75f;
			//borderDisplay.color = iconDisplay.color = displayColor;
		}
		else if (fadingIn){
			displayColor = beingAddedDisplay.color;
			displayColor.a += Time.deltaTime*fadeRateIn;
			if (displayColor.a >= fadeMax){
				displayColor.a = fadeMax;
				fadingIn = false;
			}
			beingAddedDisplay.color = displayColor;
			//displayColor.a *= 0.75f;
			//borderDisplay.color = iconDisplay.color = displayColor;
		}else{

			if (subtractTimer > 0){
				subtractTimer -= Time.deltaTime;
			}else{
				if (Mathf.Abs(beingAddedAmt) > 59){
					if (beingAddedAmt > 0){
						beingAddedAmt = Mathf.RoundToInt((beingAddedAmt*1f)-Time.deltaTime*subtractRate);
					}else{
						beingAddedAmt = Mathf.RoundToInt((beingAddedAmt*1f)+Time.deltaTime*subtractRate);
					}
					if (currencyTotalAmt > currencyDisplayAmt){
						currencyDisplayAmt = Mathf.RoundToInt((currencyDisplayAmt*1f)+Time.deltaTime*subtractRate);
					}else{
						currencyDisplayAmt = Mathf.RoundToInt((currencyDisplayAmt*1f)-Time.deltaTime*subtractRate);
					}
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
			if (beingAddedAmt > 0){
				beingAddedDisplay.text = "+"+beingAddedAmt;
			}
			else{
				beingAddedDisplay.text = beingAddedAmt.ToString();
			}
		}
	
	}

	public void DeathPenalty(){
		AddCurrency(Mathf.RoundToInt(-0.01f*PlayerCollectionS.currencyCollected)*10);
	}

	public void AddCurrency (int currencyToAdd){
		if (CanGetXP){
		PlayerCollectionS.currencyCollected += currencyToAdd;
		currencyTotalAmt = PlayerCollectionS.currencyCollected;

		if (subtractTimer > 0){
			beingAddedAmt += currencyToAdd;
		}else{
			beingAddedAmt = currencyToAdd;
		}

		subtractTimer = subtractTimerMax;

		subtractRate = Mathf.Abs(beingAddedAmt/subtractTimerMax);

		if (!showing){
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
	}

	public void SetShowing(bool nShow){
		showing = nShow;
		if (showing){
			fadingIn = true;
		}else{
			fadingOut = false;
		}
	}

	public void Show(){
		_isHiding = false;
		totalDisplay.enabled = true;
		beingAddedDisplay.enabled = true;
		borderDisplay.enabled = true;
		iconDisplay.enabled = true;

	}
	public void Hide(){
		_isHiding = true;
		totalDisplay.enabled = false;
		beingAddedDisplay.enabled = false;
		borderDisplay.enabled = false;
		iconDisplay.enabled = false;
	}
}
