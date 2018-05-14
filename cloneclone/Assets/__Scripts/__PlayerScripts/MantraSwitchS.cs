using UnityEngine;
using System.Collections;

public class MantraSwitchS : MonoBehaviour {

	private TextMesh myText;
	public TextMesh subText;

	public float showTimeMax = 0.8f;
	private float showTime;

	private bool _showing = false;

	// Use this for initialization
	void Start () {
		myText = GetComponent<TextMesh>();
		//subText = transform.GetChild(0).GetComponent<TextMesh>();
		//myText.text = subText.text = "";

	}
	
	// Update is called once per frame
	void Update () {
		if (_showing){
			showTime -= Time.deltaTime;
			if (showTime <= showTimeMax-0.12f){
				subText.color = Color.black;
			}
			if (showTime <= 0){
				myText.text = subText.text = "";
				_showing = false;
			}
		}
	}

	public void ShowMantraText(string newText){
		showTime = showTimeMax;
		_showing = true;
		subText.color = Color.white;
		myText.text = subText.text = newText;
	}
}
