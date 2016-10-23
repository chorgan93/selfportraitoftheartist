using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInteractCheckS : MonoBehaviour {

	// script that keeps track of anything that an item can be used on

	private List<LockedDoorS> _doorsInRange;
	public List<LockedDoorS> doorsInRange { get { return _doorsInRange; } }

	// Use this for initialization
	void Start () {

		_doorsInRange = new List<LockedDoorS>();
	
	}


	public bool CheckInteraction(int itemIDToCheck){
		bool didInteraction = false;
		foreach (LockedDoorS d in _doorsInRange){
			if (d.keyID == itemIDToCheck && !didInteraction){
				didInteraction = true;
				d.CheckUnlock(itemIDToCheck);
			}
		}
		return didInteraction;
	}

	public void AddDoor(LockedDoorS newDoor){
		_doorsInRange.Add(newDoor);
	}
	public void RemoveDoor(LockedDoorS newDoor){
		_doorsInRange.Remove(newDoor);
	}
}
