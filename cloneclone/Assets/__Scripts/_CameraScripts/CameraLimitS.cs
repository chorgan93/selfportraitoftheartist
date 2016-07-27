using UnityEngine;
using System.Collections;

public class CameraLimitS : MonoBehaviour {

	public float minX;
	public float maxX;
	public float minY;
	public float maxY;

	public bool removeLimit = false;


	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Player"){

			if (removeLimit){
				CameraFollowS.F.RemoveLimits();

			}else{
				CameraFollowS.F.SetLimits(transform.position.x + minX,
		                          transform.position.x + maxX,
		                          transform.position.y + minY,
		                          transform.position.y + maxY);
			}
		}

	}
}
