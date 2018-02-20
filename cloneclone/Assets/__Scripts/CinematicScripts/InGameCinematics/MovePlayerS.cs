using UnityEngine;
using System.Collections;

public class MovePlayerS : MonoBehaviour {

	public PlayerController playerRef;
	public Transform targetTransform;

	// Use this for initialization
	void Start () {
		playerRef.transform.position = targetTransform.position;
	}
	

}
