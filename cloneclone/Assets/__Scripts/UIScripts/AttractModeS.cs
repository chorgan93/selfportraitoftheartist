using UnityEngine;
using System.Collections;

public class AttractModeS : MonoBehaviour {

	#if !UNITY_WEBGL
	private MovieTexture myMovie;

	private bool isLoading = false;

	public SpriteRenderer fadeOutScreen;
	private ControlManagerS myControl;

	private AudioSource myAudio;
	private float audioFade = 1f;

	private string returnScene = "MenuScene";
	AsyncOperation async;
	bool checkSync = false;

	// Use this for initialization
	void Start () {
	
		myMovie = (MovieTexture)GetComponent<Renderer>().material.mainTexture;
		myMovie.Play();

		myControl = GetComponent<ControlManagerS>();
		fadeOutScreen.gameObject.SetActive(false);

		myAudio = GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {
	
		if (!myMovie.isPlaying && !isLoading){
			// load next scene
			StartLoading();
		}

		if (!isLoading){
			if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || myControl.GetCustomInput(10) || myControl.GetCustomInput(3) || myControl.GetCustomInput(13)){
				StartLoading();
			}
		}else{
			if (myAudio.volume > 0){
				myAudio.volume -= Time.deltaTime*audioFade;
				if (myAudio.volume <= 0){
					myAudio.Stop();
				}
			}
			if (checkSync){
			if (async.progress >= 0.9f){
				async.allowSceneActivation = true;
			}
			}

		}

	}

	void StartLoading(){
		StartCoroutine(LoadNextScene());
		isLoading = true;
		fadeOutScreen.gameObject.SetActive(true);
	}

	private IEnumerator LoadNextScene(){
		yield return fadeOutScreen.color.a < 1f;
		async = Application.LoadLevelAsync(returnScene);
		checkSync = true;
		async.allowSceneActivation = false;
		yield return async;
	}
	#endif
}
