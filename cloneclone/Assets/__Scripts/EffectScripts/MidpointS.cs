using UnityEngine;
using System.Collections;

public class MidpointS : MonoBehaviour {

	public Transform pointOne;
	public Transform pointTwo;


	// Use this for initialization
	void Start () {

		transform.position = (pointOne.position + pointTwo.position)/2f;
	
	}

}
