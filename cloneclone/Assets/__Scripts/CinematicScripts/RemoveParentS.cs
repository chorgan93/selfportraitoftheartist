using UnityEngine;
using System.Collections;

public class RemoveParentS : MonoBehaviour {

	public Transform newParent;

	// Use this for initialization
	void Start () {
		if (newParent){
			transform.parent = newParent;
		}else{
			transform.parent = null;
		}
	}
}
