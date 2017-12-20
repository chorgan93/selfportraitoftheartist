using UnityEngine;
using System.Collections;

public class MapItemS : MonoBehaviour {

	public MapPieceS[] mapPieces;

	public void TurnOn(){
		gameObject.SetActive(true);
	}
}
