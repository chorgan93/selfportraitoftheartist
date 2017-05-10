using UnityEngine;
using System.Collections;

public class SceneManagerS : MonoBehaviour {

	public bool lockBuddy = false;
	public bool lockMenus = false;
	public bool allowFastTravel = true;

	void Awake(){
		InGameCinematicS.turnOffBuddies = lockBuddy;
		InGameMenuManagerS.allowMenuUse = !lockMenus;
		InGameMenuManagerS.allowFastTravel = allowFastTravel;
	}
}
