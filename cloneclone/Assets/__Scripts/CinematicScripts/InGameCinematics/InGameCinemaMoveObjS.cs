using UnityEngine;
using System.Collections;

public class InGameCinemaMoveObjS : MonoBehaviour {

	public float moveTime = -1f;
	public int myCinemaStep = 0;
	public Transform movingObject;
	public Transform targetPosition;
	public float moveSpeed;
	public Vector3 fixedDirection = Vector3.zero;
	private Vector3 moveDirection;

	public bool faceMoveDirection;
	private Vector3 startSize;
	public float movingLeftXMult = -1f;

	public GameObject[] turnOnEnd;
	public GameObject[] turnOffEnd;
	private bool completedMove = false;
	public float turnOnTime = 0.1f;

	// Use this for initialization
	void Start () {

		if (fixedDirection == Vector3.zero){
		moveDirection = targetPosition.position-movingObject.position;
		moveDirection.z = 0f;
		}else{
			moveDirection = fixedDirection;
		}


		if (faceMoveDirection){
		startSize = movingObject.localScale;
			if (moveDirection.x < 0){
				startSize.x *= movingLeftXMult;
			}else{
				startSize.x *= -movingLeftXMult;
			}
			movingObject.localScale = startSize;
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (moveTime > 0f){
			movingObject.position += moveSpeed*Time.deltaTime*moveDirection.normalized;
			moveTime -= Time.deltaTime;
		}else if (!completedMove){

			if (turnOnEnd != null){
			for (int i = 0; i < turnOnEnd.Length; i++){
				turnOnEnd[i].SetActive(true);
			}
			}
			if (turnOffEnd != null){
			for (int i = 0; i < turnOffEnd.Length; i++){
				turnOffEnd[i].SetActive(false);
			}
			}
			completedMove = true;
		}
	
	}
}
