using UnityEngine;
using System.Collections;

public class ChangeSceneTriggerS : MonoBehaviour {

	public string nextSceneString = "";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Player" && nextSceneString!=""){
			other.GetComponent<PlayerController>().SetTalking(true);
			CameraEffectsS.E.SetNextScene(nextSceneString);
			CameraEffectsS.E.FadeIn();
		}

	}
}
