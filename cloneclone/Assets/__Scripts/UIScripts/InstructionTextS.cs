using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InstructionTextS : MonoBehaviour {

	public float fadeRate = 1f;
	private Text myText;
	public Image bgText;
	private Color textColor;
	private Color bgColor;

	private bool showing = false;
	private bool timedShowing = false;
	private float showCountdown;

	// Use this for initialization
	void Start () {

		myText = GetComponent<Text>();
		textColor = myText.color;
		textColor.a = 0;
		myText.color = textColor;

		bgColor = bgText.color;
		bgColor.a = 0;
		bgText.color = bgColor;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (showing){
			if (myText.color.a < 1){
				textColor = myText.color;
				textColor.a += Time.deltaTime*fadeRate;
				myText.color = textColor;
				bgColor.a = textColor.a;
				bgText.color = bgColor;
			}else{
				if (timedShowing){
					showCountdown-=Time.deltaTime;
					if (showCountdown <= 0){
						showing = false;
					}
				}
			}
		}else{
			if (myText.color.a > 0){
				textColor = myText.color;
				textColor.a -= Time.deltaTime*fadeRate/2f;
				myText.color = textColor;
				bgColor.a = textColor.a;
				bgText.color = bgColor;
			}
		}
	
	}

	public void SetShowing(bool newShow, string newText = ""){
		timedShowing = false;
		showing = newShow; 

		if (newText != ""){
			myText.text = newText;
		}
	}

	public void SetTimedMessage(string newText, float timeToShow){
		timedShowing = true;
		showCountdown = timeToShow;
		showing = true;
		myText.text = newText;
	}
}
