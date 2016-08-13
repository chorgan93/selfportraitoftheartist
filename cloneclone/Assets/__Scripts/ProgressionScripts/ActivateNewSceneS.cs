using UnityEngine;
using System.Collections;

public class ActivateNewSceneS : MonoBehaviour {

	public string newSceneString;

	// Use this for initialization
	void Start () {
	
		Application.LoadLevel(newSceneString);

	}

}
