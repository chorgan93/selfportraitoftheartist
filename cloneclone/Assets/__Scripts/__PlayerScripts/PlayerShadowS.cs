using UnityEngine;
using System.Collections;

public class PlayerShadowS : MonoBehaviour {

	private Vector3 startPos;
	private Vector3 upPos;
	private PlayerController myController;

	// Use this for initialization
	void Start () {

		myController = GetComponentInParent<PlayerController>();
		startPos = transform.localPosition;
		upPos = startPos;
		upPos.y *= 0.7f;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (myController.facingDown && !myController.InAttack() && !myController.isBlocking && myController.myAnimator.GetFloat("Speed") > 0.8f){
			transform.localPosition = upPos;
		}else{
			transform.localPosition = startPos;
		}
	
	}
}
