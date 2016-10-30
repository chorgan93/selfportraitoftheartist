using UnityEngine;
using System.Collections;

public class RandomBloodS : MonoBehaviour {

	public BloodS[] possSets;
	int chosenSet;

	// Use this for initialization
	void Start () {
	
		chosenSet = Mathf.RoundToInt(Random.Range(0, possSets.Length-1));

		for (int i = 0; i < possSets.Length; i++){
			if (i == chosenSet){
				possSets[i].enabled = true;
			}else{
				possSets[i].enabled = false;
			}
		}

	}

}
