using UnityEngine;
using System.Collections;

public class OverrideDemoLoadS : MonoBehaviour {

	public static bool beenToMainMenu = true;

	// Use this for initialization
	void Start () {
	
		if (beenToMainMenu){
			PlayerInventoryS.I.ReinitializeForDemo();
			beenToMainMenu = false;
		}
	}

}
