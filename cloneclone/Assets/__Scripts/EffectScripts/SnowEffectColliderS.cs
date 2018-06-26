using UnityEngine;
using System.Collections;

public class SnowEffectColliderS : MonoBehaviour {

	public Vector3 offsetPos;

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Snow"){
			other.transform.position += offsetPos;
		}
	}
}
