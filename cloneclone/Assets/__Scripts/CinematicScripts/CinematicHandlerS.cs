using UnityEngine;
using System.Collections;

public class CinematicHandlerS : MonoBehaviour {

	[Header("What to do here:")]
	public GameObject[] cinemaObjects;
	public float[] cinemaTimings;

	private float currentCountdown;
	private int currentStep;

	[Header("Where to go next:")]
	public string loadSceneString;
	private bool startedLoading = false;

	// Use this for initialization
	void Start () {

		Time.timeScale = 1f;

		currentStep = 0;
		ActivateStep();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!startedLoading){
			currentCountdown -= Time.deltaTime;
			if (currentCountdown <= 0){
				if (currentStep > cinemaObjects.Length-1){
					StartNextLoad();
				}else{
					ActivateStep();
				}
			}
		}
	
	}

	private void ActivateStep(){
		currentCountdown = cinemaTimings[currentStep];
		cinemaObjects[currentStep].SetActive(true);
		currentStep++;
	}

	private void StartNextLoad(){
		Application.LoadLevel(loadSceneString);
		startedLoading = true;
	}
}
