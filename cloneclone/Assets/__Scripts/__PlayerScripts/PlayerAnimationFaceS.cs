using UnityEngine;
using System.Collections;

public class PlayerAnimationFaceS : MonoBehaviour {

	public enum PlayerFaceState { noFace, faceLeft, faceRight, faceUp, faceDown } ;

	private static PlayerFaceState currentFace = PlayerFaceState.noFace;

	private Rigidbody rigidReference;
	private Vector3 mySize;
	private Vector3 currentSize;
	private PlayerController myController;
	private EnemyDetectS enemyDetect;

	private bool lockAttackFace = false;

	private bool dontFace = false;

	// Use this for initialization
	void Start () {

		mySize = transform.localScale;
		myController = GetComponentInParent<PlayerController>();
		rigidReference = myController.myRigidbody;

		if (!PlayerController.doWakeUp){
			myController.SetFaceDirection(currentFace);
			EvaluateStartFace();
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!enemyDetect){
			
			enemyDetect = myController.myDetect;
		}

		if (!rigidReference){
			
			rigidReference = GetComponentInParent<PlayerController>().myRigidbody;
		}

		if (Time.timeScale != 0 && !dontFace && !myController.isStunned){
		currentSize = transform.localScale;

			if (myController.facingUp && !myController.isShooting){
			currentSize = mySize;
				currentSize.x *= -1f;
				currentFace = PlayerFaceState.faceUp;

			}else if (myController.facingDown && !myController.isShooting){
				currentSize = mySize;
				currentFace = PlayerFaceState.faceDown;}
		else{
			if (enemyDetect.closestEnemy == null || myController.isDashing || myController.chargingAttack 
			                                         || myController.myStats.PlayerIsDead() || myController.IsRunning()){
				// first, do a check to see if player is inputting movement, otherwise face velocity direction
				if (myController.isDoingMovement){
					if (myController.myControl.Horizontal() < 0){
						currentSize = mySize;
							currentSize.x *= -1f;
							currentFace = PlayerFaceState.faceLeft;
					}
					if (myController.myControl.Horizontal() > 0){
							currentSize = mySize;
							currentFace = PlayerFaceState.faceRight;
					}
					}else if (myController.inAttackDelay){
						if (myController.attackStartDirection.x < 0){
							currentSize = mySize;
							currentSize.x *= -1f;
							currentFace = PlayerFaceState.faceLeft;
						}
						if (myController.attackStartDirection.x > 0){
							currentSize = mySize;
							currentFace = PlayerFaceState.faceRight;
						}
					}
					else if (!lockAttackFace){
					if (rigidReference.velocity.x < 0){
						currentSize = mySize;
							currentSize.x *= -1f;
							currentFace = PlayerFaceState.faceLeft;
					}
					if (rigidReference.velocity.x > 0){
						currentSize = mySize;
							currentFace = PlayerFaceState.faceRight;
					}
				}
			}
			else{
				float closestEnemyX = enemyDetect.closestEnemy.transform.position.x;
				if (closestEnemyX < transform.position.x){
					
					currentSize = mySize;
					currentSize.x *= -1f;
						currentFace = PlayerFaceState.faceLeft;
				}
				if (closestEnemyX > transform.position.x){
					
						currentSize = mySize;
						currentFace = PlayerFaceState.faceRight;
				}
			}
		}
		transform.localScale = currentSize;
		}
	
	}

	void EvaluateStartFace(){
		if (currentFace == PlayerFaceState.faceLeft || currentFace == PlayerFaceState.faceUp){
			currentSize = mySize;
			currentSize.x *= -1f;
		}else if (currentFace == PlayerFaceState.faceRight || currentFace == PlayerFaceState.faceDown){
			currentSize = mySize;
		}

		transform.localScale = currentSize;
	}

	public void AllowFace(){
		dontFace = false;
	}
	public void StopFace(){
		dontFace = true;
	}



	public void SetAttackLock(bool newLock){
		lockAttackFace = newLock;
	}

}
