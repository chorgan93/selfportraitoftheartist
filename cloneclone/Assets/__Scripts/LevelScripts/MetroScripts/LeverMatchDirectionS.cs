using UnityEngine;
using System.Collections;

public class LeverMatchDirectionS : MonoBehaviour {

	public GameObject leverPositive;
	public GameObject leverNegative;
	public bool alwaysNegative = false;

	// Use this for initialization
	void Start () {

		if (TrainCarS.currentDirection > 0 && !alwaysNegative){
			leverPositive.gameObject.SetActive(true);
			leverNegative.gameObject.SetActive(false);
		}else{
			leverNegative.gameObject.SetActive(true);
			leverPositive.gameObject.SetActive(false);
		}
	
	}

}
