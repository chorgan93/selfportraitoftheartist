using UnityEngine;
using System.Collections;

public class TimedShakesS : MonoBehaviour {

	public float[] shakeDelays;
	public int[] shakePowers;
	private int currentShake = 0;
	private float currentCountdown = 0;

	// Use this for initialization
	void Start () {

		currentCountdown = shakeDelays[currentShake];
	
	}
	
	// Update is called once per frame
	void Update () {

		currentCountdown -= Time.deltaTime;
		if (currentCountdown <= 0){
			if (shakePowers[currentShake] == 0){
				CameraShakeS.C.MicroShake();
			}else if (shakePowers[currentShake] == 1){
				CameraShakeS.C.SmallShake();
			}else if (shakePowers[currentShake] == 2){
				CameraShakeS.C.LargeShake();
			}

			currentShake++;
			if (currentShake > shakeDelays.Length-1){
				enabled = false;
			}else{
				currentCountdown = shakeDelays[currentShake];
			}
		}
	}
}
