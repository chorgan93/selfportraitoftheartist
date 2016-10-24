using UnityEngine;
using System.Collections;

public class ActivateNewSceneS : MonoBehaviour {

	public string newSceneString;

	AsyncOperation async;

	// Use this for initialization
	void Start () {
	
		StartLoading();

	}

	void Update(){

		if (async.progress >= 0.9f){
			async.allowSceneActivation = true;
		}

	}

	private void StartLoading(){
		StartCoroutine(LoadNextScene());
	}
	
	private IEnumerator LoadNextScene(){
		async = Application.LoadLevelAsync(newSceneString);
		async.allowSceneActivation = false;
		yield return async;
	}

}
