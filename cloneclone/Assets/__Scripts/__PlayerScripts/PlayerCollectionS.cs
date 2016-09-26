using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCollectionS : MonoBehaviour {

	private static bool initialized = false;

	public static List<int> upgradesGathered; 
	public static List<int> keysGathered; // 1,2,3,4
	public static int currencyCollected = 0;
	
	public static void Initialize(){

		if (!initialized){

			LoadUpgrades();
			LoadKeys();

			initialized = true;
		}
	
	}
	
	private static void LoadUpgrades(){

		upgradesGathered = new List<int>();

	}

	private static void LoadKeys(){

		keysGathered = new List<int>();

	}
}
