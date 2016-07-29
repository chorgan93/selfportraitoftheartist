using UnityEngine;
using System.Collections;

public class RotateEffectS : MonoBehaviour {

	public Vector3 rotateRate;

	public float rotateIntervalMax = 0.083f;
	private float rotateInterval;

	// Use this for initialization
	void Start () {

		rotateInterval = rotateIntervalMax;
	
	}
	
	// Update is called once per frame
	void Update () {

		rotateInterval -= Time.deltaTime;
		if (rotateInterval <= 0){
			rotateInterval = rotateIntervalMax;
			transform.Rotate(rotateRate*Time.deltaTime);
		}
	
	}
}
