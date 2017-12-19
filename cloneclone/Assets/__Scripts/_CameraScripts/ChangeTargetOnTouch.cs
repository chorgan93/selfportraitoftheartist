using UnityEngine;
using System.Collections;

public class ChangeTargetOnTouch : MonoBehaviour {

	public GameObject newTarget;

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player"){
			if (newTarget){
				CameraFollowS.F.SetNewPOI(newTarget, true);
			}else{
				CameraFollowS.F.ResetPOI();
			}
		}
	}
}
