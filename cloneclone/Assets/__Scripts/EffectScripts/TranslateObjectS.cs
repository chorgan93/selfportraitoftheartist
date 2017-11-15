using UnityEngine;
using System.Collections;

public class TranslateObjectS : MonoBehaviour {

	public Vector3 moveDirection;
	public float moveSpeed;
	public float moveAccel;

	private Vector3 targetPos;

	public float moveTimeMax = -1;
	private float moveTime;
	private bool stopMovingOnTime = false;

	void Start(){
		if (moveTimeMax > 0){
			stopMovingOnTime = true;
			moveTime = moveTimeMax;
		}
	}

	
	// Update is called once per frame
	void FixedUpdate () {

		targetPos = transform.position;
		targetPos += moveDirection*moveSpeed*Time.deltaTime;
		transform.position = targetPos;
		moveSpeed += moveAccel*Time.deltaTime;

		if (stopMovingOnTime){
			moveTime -= Time.deltaTime;
			if (moveTime <= 0){
				enabled = false;
			}
		}

	}
}
