using UnityEngine;
using System.Collections;

public class TurnOffInGameUIS : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		GameObject.Find("Player Status").SetActive(false);
		GameObject.Find("SinBorder").SetActive(false);

	}
}
