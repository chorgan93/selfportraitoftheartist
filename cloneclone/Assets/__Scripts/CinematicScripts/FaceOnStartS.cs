using UnityEngine;
using System.Collections;

public class FaceOnStartS : MonoBehaviour {

	public Transform targetTransform;
	public float lookingLeftXMult = -1f;

	// Use this for initialization
	void Start () {

		if (targetTransform){
			Vector3 lookDir = (targetTransform.position - transform.position).normalized;
			Vector3 startSize = transform.localScale;
			if (lookDir.x < 0){
				startSize.x *= lookingLeftXMult;
			}else{
				startSize.x *= -lookingLeftXMult;
			}
			transform.localScale = startSize;
		}

	}
}
