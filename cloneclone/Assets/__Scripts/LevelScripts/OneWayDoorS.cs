using UnityEngine;
using System.Collections;

public class OneWayDoorS : MonoBehaviour {

	public int doorId = -1;
	public GameObject doorObject;
	public GameObject lockObject;

	// Use this for initialization
	void Start () {

		if (PlayerInventoryS.I.openedDoors.Contains(doorId)){
			doorObject.gameObject.SetActive(true);
			lockObject.gameObject.SetActive(false);
		}else{
			doorObject.gameObject.SetActive(false);
			lockObject.gameObject.SetActive(true);
		}
	
	}
}
