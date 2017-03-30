using UnityEngine;
using System.Collections;

public class MatchCinemaPositionS : MonoBehaviour {

	public Transform targetPos;

	void Start(){
		transform.position = targetPos.transform.position;
	}
}
