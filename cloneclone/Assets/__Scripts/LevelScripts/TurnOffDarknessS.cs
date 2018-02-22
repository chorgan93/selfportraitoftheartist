using UnityEngine;
using System.Collections;

public class TurnOffDarknessS : MonoBehaviour {

	public DarknessS targetDark;

	// Use this for initialization
	void Start () {
	
		if (targetDark){
			targetDark.TurnOff();
		}

	}
}
