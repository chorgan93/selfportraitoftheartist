using UnityEngine;
using System.Collections;

public class ParentToObjectS : MonoBehaviour {

	public Transform desiredParent;
	public Vector3 desiredLocalPos = Vector3.zero;

	// Use this for initialization
	void Start () {
		transform.parent = desiredParent;
		transform.localPosition = desiredLocalPos;
	}
}
