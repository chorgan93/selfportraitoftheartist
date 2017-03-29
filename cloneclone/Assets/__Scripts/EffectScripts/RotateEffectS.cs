using UnityEngine;
using System.Collections;

public class RotateEffectS : MonoBehaviour {

	public Vector3 rotateRate;

	public float rotateIntervalMax = 0.083f;
	private float rotateInterval;
	private bool constantRotate = false;

	// Use this for initialization
	void Start () {

		rotateInterval = rotateIntervalMax;
		if (rotateIntervalMax <= 0){
			constantRotate = true;
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (constantRotate){
			transform.Rotate(rotateRate*Time.deltaTime);
		}else{
		rotateInterval -= Time.deltaTime;
		if (rotateInterval <= 0){
			rotateInterval = rotateIntervalMax;
			transform.Rotate(rotateRate*Time.deltaTime);
		}
		}
	
	}
}
