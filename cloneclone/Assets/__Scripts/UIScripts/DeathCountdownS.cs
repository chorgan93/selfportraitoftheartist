using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeathCountdownS : MonoBehaviour {

	public static float deathCountdown = -1f;
	private float deathCountdownTimer; // counted down
	private float deathCountShow; // showing time rounded to two decimal places
	private Text myText;
	public Image deathScreen;

	private bool _initialized = false;
	private bool countdownActive = false;

	public DeathCountdownHandlerS currentHandler;

	public static DeathCountdownS DC;

	private float fadeRate = 1f;
	private bool fadingIn = false;
	private bool fadingOut = false;

	private Color myCol;
	private Color startCol;
	private Color endCol = Color.red;
	private Color offCol = Color.grey;

	// Use this for initialization
	void Awake () {
	
		DC = this;

		Initialize();

	}

	void Initialize(){
		if (!_initialized){
			myText = GetComponent<Text>();
			startCol = myCol = myText.color;
			_initialized = true;
			deathScreen.gameObject.SetActive(false);
		}
		if(deathCountdown > 0){
			countdownActive = true;
			deathCountdownTimer = deathCountdown;
			deathCountShow = Mathf.Round(deathCountdownTimer*100f)/100f;
			myText.text = deathCountShow.ToString();
			FadeInCountdown();
		}else{
			myText.text = "";
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (countdownActive){
			if (deathCountdownTimer > 0){
				deathCountdownTimer -= Time.unscaledDeltaTime;
				deathCountShow = Mathf.Round(deathCountdownTimer*100f)/100f;
				myText.text = deathCountShow.ToString();
			}else{
				if (!fadingIn && !fadingOut){
					CountdownEnd();
				}
			}
		}
		if (fadingIn){
			myCol.a += Time.deltaTime*fadeRate;
			if (myCol.a >= 1f){
				myCol.a = 1f;
				fadingIn = false;
			}
			myText.color = myCol;
		}
		if (fadingOut){
			myCol.a -= Time.deltaTime*fadeRate;
			if (myCol.a <= 0f){
				myCol.a = 0f;
				fadingOut = false;
			}
			myText.color = myCol;
		}
	
	}

	void CountdownEnd(){
		myCol = endCol;
		myText.color = endCol;
		myText.text = "0.00";
		countdownActive = false;
		deathScreen.gameObject.SetActive(true);
		if (currentHandler != null){
			currentHandler.ActivateDeath();
		}
	}

	public void ActivateCountdown(float newTime){
		deathCountdown = deathCountdownTimer= newTime;
		countdownActive = true;
		FadeInCountdown();
	}

	public void TurnOffCountdown(){
		countdownActive = false;
		myCol = offCol;
		FadeOutCountdown();
	}

	public void FadeInCountdown(){
		myCol = startCol;
		myCol.a = 0f;
		myText.color = myCol;
		fadingIn = true;
	}

	public void FadeOutCountdown(){
		if (myText.color.a > 0){
		myCol.a = 1f;
		myText.color = myCol;
		fadingOut = true;
			if (deathCountdownTimer > 0 && countdownActive){
			deathCountdown = deathCountdownTimer;
		}
		}else{
			myText.text = "";
		}
	}
}
