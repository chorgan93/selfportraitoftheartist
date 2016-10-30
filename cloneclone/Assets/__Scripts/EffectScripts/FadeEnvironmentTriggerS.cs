using UnityEngine;
using System.Collections;

public class FadeEnvironmentTriggerS : MonoBehaviour {

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "FadeEnvironment"){
			other.gameObject.GetComponent<FadeEnvironmentS>().FadeLvUp();
		}

	}

	void OnTriggerExit(Collider other){
		
		if (other.gameObject.tag == "FadeEnvironment"){
			other.gameObject.GetComponent<FadeEnvironmentS>().FadeLvDown();
		}
		
	}
}
