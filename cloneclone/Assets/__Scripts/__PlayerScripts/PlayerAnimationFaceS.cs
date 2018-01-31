using UnityEngine;
using System.Collections;

public class PlayerAnimationFaceS : MonoBehaviour {

	private Rigidbody rigidReference;
	private Vector3 mySize;
	private Vector3 currentSize;
	private PlayerController myController;
	private EnemyDetectS enemyDetect;

	private bool dontFace = false;

	// Use this for initialization
	void Start () {

		mySize = transform.localScale;
		myController = GetComponentInParent<PlayerController>();
		rigidReference = myController.myRigidbody;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!enemyDetect){
			
			enemyDetect = myController.myDetect;
		}

		if (!rigidReference){
			
			rigidReference = GetComponentInParent<PlayerController>().myRigidbody;
		}

		if (Time.timeScale != 0 && !dontFace){
		currentSize = transform.localScale;

		if (myController.facingUp){
			currentSize = mySize;
			currentSize.x *= -1f;

		}else if (myController.facingDown){
			currentSize = mySize;}
		else{
			if (enemyDetect.closestEnemy == null || myController.isDashing || myController.chargingAttack 
			                                         || myController.myStats.PlayerIsDead() || myController.IsRunning()){
				// first, do a check to see if player is inputting movement, otherwise face velocity direction
				if (myController.isDoingMovement){
					if (myController.myControl.Horizontal() < 0){
						currentSize = mySize;
						currentSize.x *= -1f;
					}
					if (myController.myControl.Horizontal() > 0){
						currentSize = mySize;
					}
					}else if (myController.inAttackDelay){
						if (myController.attackStartDirection.x < 0){
							currentSize = mySize;
							currentSize.x *= -1f;
						}
						if (myController.attackStartDirection.x > 0){
							currentSize = mySize;
						}
					}
						else{
					if (rigidReference.velocity.x < 0){
						currentSize = mySize;
						currentSize.x *= -1f;
					}
					if (rigidReference.velocity.x > 0){
						currentSize = mySize;
					}
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

	public void AllowFace(){
		dontFace = false;
	}
	public void StopFace(){
		dontFace = true;
	}
}
