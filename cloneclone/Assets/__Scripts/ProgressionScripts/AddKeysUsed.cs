using UnityEngine;
using System.Collections;

public class AddKeysUsed : MonoBehaviour {

	public int[] addKeys;

	// Use this for initialization
	void Start () {
	
		for (int i = 0; i < addKeys.Length; i++){
			PlayerInventoryS.I.AddClearedWall(addKeys[i]);
		}

	}

}
