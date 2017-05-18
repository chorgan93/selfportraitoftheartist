using UnityEngine;
using System.Collections;

public class TurnOffItemUseS : MonoBehaviour {

	public PlayerController playerRef;

	// Use this for initialization
	void Start () {
	
		playerRef.SetAllowItem(false);

	}
}
