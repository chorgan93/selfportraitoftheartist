using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingTextS : MonoBehaviour {

	private float addCharacterTimeMax = 0.4f;
	private float addCharacterTime;

	private string loadingMessage = "LOADING";
	private int numPeriods = 0;
	private int numPeriodsMax = 3;

	private Text _myText;

	// Use this for initialization
	void Start () {

		_myText = GetComponent<Text>();
		addCharacterTime = addCharacterTimeMax;

		_myText.text = loadingMessage;
	
	}
	
	// Update is called once per frame
	void Update () {
	
		addCharacterTime -= Time.deltaTime;
		if (addCharacterTime <= 0){
			addCharacterTime = addCharacterTimeMax;
			numPeriods++;
			if (numPeriods > numPeriodsMax){
				numPeriods = 0;
			}
			string newMessage = loadingMessage;
			for (int i = 0; i < numPeriods; i++){
				newMessage += ".";
			}
			_myText.text = newMessage;
		}
	}
}
