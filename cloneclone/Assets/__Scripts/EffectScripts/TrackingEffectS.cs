using UnityEngine;
using System.Collections;

public class TrackingEffectS : MonoBehaviour {

	private Animator myAnimator;
	private string animatorTrigger = "Effect";
	private string multName = "SpeedMult";
	private SpriteRenderer myRenderer;
	private float startRotateZ;
	private Color changeCol;
	public float offsetDistance;
	private Vector3 offsetPos;
	private Vector3 startPos;

	//private Transform startParent;
	//private float resetParentCount = 0.8f;
	//private float resetParentCountdown;
	//private bool checkReset = false;

	// Use this for initialization
	void Start () {

		myAnimator = GetComponent<Animator>();
		myRenderer = GetComponent<SpriteRenderer>();
		startRotateZ = transform.rotation.eulerAngles.z;
		startPos = transform.localPosition;
		startPos.z+=3f;

		//startParent = transform.parent;
	
	}

	/*void Update(){
		if (checkReset){
			resetParentCountdown -= Time.deltaTime;
			if (resetParentCountdown <= 0){
				transform.parent = startParent;
				checkReset = false;
			}
		}
	}**/
	
	public void FireEffect(Vector3 aimDirection, Color newCol, float delayMult, Vector3 offCenter){
		changeCol = newCol;
		changeCol.a = myRenderer.color.a;
		myRenderer.color = changeCol;

		myAnimator.SetFloat(multName, 0.5f/delayMult);
		myAnimator.SetTrigger(animatorTrigger);
		FaceDirection(aimDirection);
		offsetPos = offCenter+aimDirection.normalized*offsetDistance;
		offsetPos.z = 0f;
		if (offCenter.x != 0 && offCenter.y != 0){
			transform.position = offsetPos;
		}else{
			transform.localPosition = startPos+offsetPos;
		}
		myRenderer.enabled = true;
		/*transform.parent = null;
		resetParentCountdown = resetParentCount;
		checkReset = true;**/
	}

	public void TurnOffEffect(){
		if (myRenderer){
			myRenderer.enabled = false;
		}
	}

	private void FaceDirection(Vector3 direction){

		float rotateZ = 0;

		Vector3 targetDir = direction.normalized;

		if(targetDir.x == 0){
			if (targetDir.y > 0){
				rotateZ = 90;
			}
			else{
				rotateZ = -90;
			}
		}
		else{
			rotateZ = Mathf.Rad2Deg*Mathf.Atan((targetDir.y/targetDir.x));
		}	


		if (targetDir.x < 0){
			rotateZ += 180;
		}



		transform.rotation = Quaternion.Euler(new Vector3(0,0,rotateZ+startRotateZ));


	}
}
