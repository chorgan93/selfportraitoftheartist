using UnityEngine;
using System.Collections;

public class SetDirectionStatic : MonoBehaviour {

	public int newDir;

	// Use this for initialization
	void Start () {
		TrainCarS.currentDirection = newDir;
	}

}
