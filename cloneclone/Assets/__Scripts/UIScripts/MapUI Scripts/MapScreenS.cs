using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapScreenS : MonoBehaviour {

	public Text noMapText;
	public MapItemS[] mapItems;


	public void Activate(int mapToUse, int currentScene){

		bool showText = true;
		if (mapToUse >= 0 && mapToUse < mapItems.Length){
			mapItems[mapToUse].TurnOn();
			showText = false;
		}
			
		noMapText.enabled = showText;

		gameObject.SetActive(true);
	}
}
