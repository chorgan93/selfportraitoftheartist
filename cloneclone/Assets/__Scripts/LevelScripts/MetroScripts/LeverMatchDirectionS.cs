using UnityEngine;
using System.Collections;

public class LeverMatchDirectionS : MonoBehaviour {

	public GameObject leverPositive;
	public GameObject leverNegative;

	// Use this for initialization
	void Start () {

		if (TrainCarS.currentDirection > 0){
			leverPositive.gameObject.SetActive(true);
			leverNegative.gameObject.SetActive(false);
		}else{
			leverNegative.gameObject.SetActive(true);
			leverPositive.gameObject.SetActive(false);
		}
	
	}

}
