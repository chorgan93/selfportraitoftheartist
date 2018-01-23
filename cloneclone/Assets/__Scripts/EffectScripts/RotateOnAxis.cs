using UnityEngine;
using System.Collections;

public class RotateOnAxis : MonoBehaviour {

	public float rotateAmt;

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
			transform.RotateAround(transform.position, transform.right, rotateAmt*Time.deltaTime);
		}else{
		rotateInterval -= Time.deltaTime;
		if (rotateInterval <= 0){
				rotateInterval = rotateIntervalMax;
				transform.RotateAround(transform.position, transform.right, rotateAmt*Time.deltaTime);
		}
		}
	
	}
}
