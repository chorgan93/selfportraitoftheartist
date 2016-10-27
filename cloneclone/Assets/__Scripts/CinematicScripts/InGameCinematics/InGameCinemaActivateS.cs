using UnityEngine;
using System.Collections;

public class InGameCinemaActivateS : MonoBehaviour {

	public float activateTime = -1f;
	public int myCinemaStep = 0;

	public GameObject[] onObjects;
	public GameObject[] offObjects;

	// Use this for initialization
	void Start () {

		foreach (GameObject on in onObjects){
			on.SetActive(true);
		}

		foreach (GameObject off in offObjects){
			off.SetActive(false);
		}
	
	}

}
