using UnityEngine;
using System.Collections;

public class SceneManagerS : MonoBehaviour {

	public bool lockBuddy = false;
	public bool lockMenus = false;

	void Awake(){
		InGameCinematicS.turnOffBuddies = lockBuddy;
		InGameMenuManagerS.allowMenuUse = !lockMenus;
	}
}
