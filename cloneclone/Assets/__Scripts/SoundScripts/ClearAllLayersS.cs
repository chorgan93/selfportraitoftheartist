using UnityEngine;
using System.Collections;

public class ClearAllLayersS : MonoBehaviour {

	public bool clearInstant = false;
	public bool destroyOnClear = false;

	// Use this for initialization
	void Start () {
	
		BGMHolderS.BG.EndAllLayers(clearInstant, destroyOnClear);

	}

}
