using UnityEngine;
using System.Collections;

public class CinematicTextS : MonoBehaviour {

	private TextMesh myText;
	private string targetString;
	private string displayString;

	public float scrollRate;
	private float scrollCountdown;
	private int currentChar;

	public float readTime = 1.4f; // time after completion of scroll before destroying

	// Use this for initialization
	void Start () {

		myText = GetComponent<TextMesh>();

		targetString = myText.text;
		myText.text = "";
		scrollCountdown = scrollRate;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (currentChar > targetString.Length-1){
			readTime -= Time.deltaTime;
			if (readTime <= 0){
				Destroy(gameObject);
			}
		}else{
			scrollCountdown -= Time.deltaTime;
			if (scrollCountdown <= 0){
				displayString += targetString[currentChar];
				myText.text = displayString;
				currentChar++;
				scrollCountdown = scrollRate;
			}
		}
	
	}
}
