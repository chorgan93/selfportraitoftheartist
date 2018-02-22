using UnityEngine;
using System.Collections;

public class DarknessS : MonoBehaviour {

	// just turns on darkness in a nice way to look like it's spreading
	public GameObject[] darkBits;
	public float turnOnTime = 2f;
	private float turnOnCountdown = 0;


	// Use this for initialization
	void Start () {


		turnOnCountdown = turnOnTime/(darkBits.Length*1f);
		StartCoroutine(TurnOn());

	}

	public void TurnOff(){
		StartCoroutine(TurnOffEffect());
	}

	IEnumerator TurnOn(){

		for (int i = 0; i < darkBits.Length; i++){
			darkBits[i].SetActive(true);
			yield return new WaitForSeconds(turnOnCountdown);
		}

	}

	IEnumerator TurnOffEffect(){
		for (int i = darkBits.Length-1; i >= 0; i--){
			darkBits[i].GetComponent<DarkBitS>().ActivateFadeOut();
			yield return new WaitForSeconds(turnOnCountdown);
		}
	}
}
