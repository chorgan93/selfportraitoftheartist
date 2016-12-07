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


	IEnumerator TurnOn(){

		for (int i = 0; i < darkBits.Length; i++){
			darkBits[i].SetActive(true);
			yield return new WaitForSeconds(turnOnCountdown);
		}

	}
}
