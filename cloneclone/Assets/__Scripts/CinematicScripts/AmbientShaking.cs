using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientShaking : MonoBehaviour {

    public float shakeTimeMin = 0.1f;
    public float shakeTimeMax = 0.4f;
    private float shakeCountdown = 0f;

    public int biggerShakeMin = 4;
    public int biggerShakeMax = 9;
    private int biggerShakeCountdown = 0;

	// Use this for initialization
	void Start () {
        shakeCountdown = Random.Range(shakeTimeMin, shakeTimeMax);
	}
	
	// Update is called once per frame
	void Update () {
        shakeCountdown -= Time.deltaTime;
        if (shakeCountdown <= 0){
            biggerShakeCountdown--;
            if (biggerShakeCountdown <= 0){
                biggerShakeCountdown = Mathf.RoundToInt(Random.Range(biggerShakeMin, biggerShakeMax));
            }
            CameraShakeS.C.MicroShake();
            shakeCountdown = Random.Range(shakeTimeMin, shakeTimeMax);
        }
	}
}
