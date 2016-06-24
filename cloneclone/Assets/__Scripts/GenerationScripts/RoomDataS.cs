using UnityEngine;
using System.Collections;

public class RoomDataS : MonoBehaviour {

	public int roomID = -1;

	private TextMesh idVisualization;

	void Start(){

		idVisualization = GetComponentInChildren<TextMesh>();

		idVisualization.text = "";

	}

	void Update(){

		if (roomID >= 0){
			idVisualization.text = roomID.ToString();
		}

	}
}
