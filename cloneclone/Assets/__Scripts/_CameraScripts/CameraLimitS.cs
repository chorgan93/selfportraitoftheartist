using UnityEngine;
using System.Collections;

public class CameraLimitS : MonoBehaviour {

	public float minX;
	public float maxX;
	public float minY;
	public float maxY;


	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Player"){
			CameraFollowS.F.SetLimits(transform.position.x + minX,
		                          transform.position.x + maxX,
		                          transform.position.y + minY,
		                          transform.position.y + maxY);

			Debug.Log(transform.position.x + minX);
		}

	}

	void OnTriggerExit(Collider other){

		if (other.gameObject.tag == "Player"){
			CameraFollowS.F.RemoveLimits();
		}

	}
}
