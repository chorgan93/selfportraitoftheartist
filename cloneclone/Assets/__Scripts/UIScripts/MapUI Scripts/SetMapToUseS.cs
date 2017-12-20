using UnityEngine;
using System.Collections;

public class SetMapToUseS : MonoBehaviour {

	public int setMapNum=-1;

	// Use this for initialization
	void Awake () {
	
		GameObject.Find("EquipMenu").GetComponent<EquipMenuS>().SetMapScene(setMapNum);

	}
}

