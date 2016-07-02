using UnityEngine;
using System.Collections;

public class PlayerAnimationFaceS : MonoBehaviour {

	private Rigidbody rigidReference;
	private Vector3 mySize;
	private Vector3 currentSize;

	// Use this for initialization
	void Start () {

		mySize = transform.localScale;
		rigidReference = GetComponentInParent<PlayerController>().myRigidbody;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!rigidReference){
			
			rigidReference = GetComponentInParent<PlayerController>().myRigidbody;
		}

		currentSize = transform.localScale;

		if (rigidReference.velocity.x < 0){
			currentSize = mySize;
			currentSize.x *= -1f;
		}
		if (rigidReference.velocity.x > 0){
			currentSize = mySize;
		}
		transform.localScale = currentSize;
	
	}
}
