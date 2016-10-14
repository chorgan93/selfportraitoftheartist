using UnityEngine;
using System.Collections;

public class SlashEffectS : MonoBehaviour {

	private float startMoveAmt = 0.5f;
	private float moveAmt00 = 0.3f;
	private float moveAmt01 = 0.6f;

	private float rotateAmt = 3f;

	public LineRenderer myTrail00;
	public LineRenderer myTrail01;

	private float lifeTime = 0.06f;

	private bool negativeMove = false;
	private int numMoves = 1;

	// Use this for initialization
	void Start () {

		transform.Rotate(new Vector3(0,0,Random.insideUnitCircle.x * rotateAmt));

		if (Random.Range(0, 1) < 0.5f){
			negativeMove = true;
		}

		// set trail 1
		Vector3 startPos = transform.position;
		if (negativeMove){
			startPos += transform.up*startMoveAmt*Mathf.Abs(transform.parent.localScale.x);
		}else{
			startPos -= transform.up*startMoveAmt*Mathf.Abs(transform.parent.localScale.x);
		}
		if (transform.position.z > 0){
			startPos.z = 1f;
		}else{
			startPos.z = -1f;
		}
		myTrail00.SetPosition(0, startPos);

		// finish trail 1, start trail 2
		startPos = transform.position;
		if (negativeMove){
			startPos -= transform.up*moveAmt00*Mathf.Abs(transform.parent.localScale.x)*numMoves;
		}else{
			startPos += transform.up*moveAmt00*Mathf.Abs(transform.parent.localScale.x)*numMoves;
		}
		if (transform.position.z > 0){
			startPos.z = 1f;
		}else{
			startPos.z = -1f;
		}
		myTrail00.SetPosition(1, startPos);
		myTrail01.SetPosition(0, startPos);

		// finish trail 2
		if (negativeMove){
			startPos -= transform.up*moveAmt01*Mathf.Abs(transform.parent.localScale.x)*numMoves;
		}else{
			startPos += transform.up*moveAmt01*Mathf.Abs(transform.parent.localScale.x)*numMoves;
		}
		if (transform.position.z > 0){
			startPos.z = 1f;
		}else{
			startPos.z = -1f;
		}
		myTrail01.SetPosition(1, startPos);


	}

	void FixedUpdate(){
		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0){
			Destroy(gameObject);
		}
	}
}
