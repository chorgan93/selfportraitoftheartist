using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapScreenS : MonoBehaviour {

	public Text noMapText;
	public MapItemS[] mapItems;

	public Image playerPosition;

	public void Activate(int mapToUse, int currentScene){

		bool showText = true;
		if (mapToUse >= 0 && mapToUse < mapItems.Length){
			mapItems[mapToUse].TurnOn(this);
			showText = false;
		}
			
		noMapText.enabled = showText;
		playerPosition.enabled = !showText;


		gameObject.SetActive(true);
	}
}
