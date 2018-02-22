using UnityEngine;
using System.Collections;

public class AmbientShakingS : MonoBehaviour {

	public float shakeRate = 0.1f;
	private float shakeCountdown = 0f;
	public int shakeAmt = 1;

	// Use this for initialization
	void Start () {
		shakeCountdown = shakeRate;
	}
	
	// Update is called once per frame
	void Update () {
		shakeCountdown -= Time.deltaTime;
		if (shakeCountdown <= 0){
			shakeCountdown = shakeRate;
			if (shakeAmt == 0){
				CameraShakeS.C.MicroShake();
			}else if (shakeAmt == 1){
				CameraShakeS.C.SmallShake();
			}else{
				CameraShakeS.C.LargeShake();
			}
		}
	}
}
