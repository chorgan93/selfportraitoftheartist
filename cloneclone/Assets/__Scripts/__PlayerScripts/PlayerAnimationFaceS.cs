using UnityEngine;
using System.Collections;

public class PlayerAnimationFaceS : MonoBehaviour {

	private Rigidbody rigidReference;
	private Vector3 mySize;
	private Vector3 currentSize;
	private PlayerController myController;
	private EnemyDetectS enemyDetect;

	// Use this for initialization
	void Start () {

		mySize = transform.localScale;
		myController = GetComponentInParent<PlayerController>();
		rigidReference = myController.myRigidbody;
		enemyDetect = myController.myDetect;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!rigidReference){
			
			rigidReference = GetComponentInParent<PlayerController>().myRigidbody;
		}

		currentSize = transform.localScale;

		if (myController.facingUp){
			currentSize = mySize;
			currentSize.x *= -1f;

		}else if (myController.facingDown){
			currentSize = mySize;}
		else{
			if (!myController.isBlocking || (myController.isBlocking && enemyDetect.closestEnemy == null)){
		if (rigidReference.velocity.x < 0){
			currentSize = mySize;
			currentSize.x *= -1f;
		}
		if (rigidReference.velocity.x > 0){
			currentSize = mySize;
		}
			}
			else{
				float closestEnemyX = enemyDetect.closestEnemy.transform.position.x;
				if (closestEnemyX < transform.position.x){
					
					currentSize = mySize;
					currentSize.x *= -1f;
				}
				if (closestEnemyX > transform.position.x){
					
					currentSize = mySize;
				}
			}
		}
		transform.localScale = currentSize;
	
	}
}
