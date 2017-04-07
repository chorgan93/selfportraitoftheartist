using UnityEngine;
using System.Collections;

public class ActivateOnBarrierOffS : MonoBehaviour {

	public GameObject[] onObjects;
	public GameObject[] offObjects;


	public void OnOff(){
		for (int i = 0; i < onObjects.Length; i++){
			onObjects[i].SetActive(true);
		}
		for (int i = 0; i < offObjects.Length; i++){
			offObjects[i].SetActive(false);
		}
	}
}
