using UnityEngine;
using System.Collections;

public class InGameCinemaMoveObjS : MonoBehaviour {

	public float moveTime = -1f;
	public int myCinemaStep = 0;
	public Transform movingObject;
	public Transform targetPosition;
	public float moveSpeed;
	private Vector3 moveDirection;

	public bool faceMoveDirection;
	private Vector3 startSize;
	public float movingLeftXMult = -1f;

	// Use this for initialization
	void Start () {

		moveDirection = (targetPosition.transform.position-movingObject.transform.position).normalized;
		moveDirection.z = 0f;

		if (faceMoveDirection){
		startSize = movingObject.transform.localScale;
			if (moveDirection.x < 0){
				startSize.x *= movingLeftXMult;
			}else{
				startSize.x *= -movingLeftXMult;
			}
			movingObject.transform.localScale = startSize;
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (moveTime > 0f){
			movingObject.transform.position += moveSpeed*Time.deltaTime*moveDirection;
			moveTime -= Time.deltaTime;
		}
	
	}
}
