using UnityEngine;
using System.Collections;

public class TranslateObjectS : MonoBehaviour {

	public Vector3 moveDirection;
	public float moveSpeed;
	public float moveAccel;

	private Vector3 targetPos;

	
	// Update is called once per frame
	void FixedUpdate () {
	
		targetPos = transform.position;
		targetPos += moveDirection*moveSpeed*Time.deltaTime;
		transform.position = targetPos;
		moveSpeed += moveAccel*Time.deltaTime;

	}
}
