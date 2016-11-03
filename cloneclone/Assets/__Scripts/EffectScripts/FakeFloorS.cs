using UnityEngine;
using System.Collections;

public class FakeFloorS : MonoBehaviour {
	
	private float turnOffGravityY;
	public float yVariance = 1f;
	private bool gravityOff = false;

	// Use this for initialization
	void Start () {
		turnOffGravityY = transform.position.y-Random.Range(0,1f)*yVariance;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < turnOffGravityY && !gravityOff){
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			gravityOff = true;
		}
	}
}
