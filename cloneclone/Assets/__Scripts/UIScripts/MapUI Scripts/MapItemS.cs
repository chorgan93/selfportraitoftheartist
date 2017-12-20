using UnityEngine;
using System.Collections;

public class MapItemS : MonoBehaviour {

	public MapPieceS[] mapPieces;

	private bool _initialized = false;
	public bool initialized { get { return _initialized;  }}

	private MapScreenS myMapScreen;
	public MapScreenS mapRef { get { return myMapScreen; } }

	public void TurnOn(MapScreenS newMapS){
		if (!_initialized){
			_initialized = true;
			myMapScreen = newMapS;
			for (int i  = 0; i < mapPieces.Length; i++){
				mapPieces[i].SetMapRef(myMapScreen);
			}
		}
		gameObject.SetActive(true);
	}
}
