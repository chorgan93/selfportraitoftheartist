using UnityEngine;
using System.Collections;

public class LeverMatchDirectionS : MonoBehaviour {

	public GameObject leverPositive;
	public GameObject leverNegative;

	// Use this for initialization
	void Start () {

		if (TrainCarS.currentDirection > 0){
			leverPositive.gameObject.SetActive(true);
		}else{
			leverNegative.gameObject.SetActive(true);
		}
	
	}

}
