using UnityEngine;
using System.Collections;

public class ColorFilterSetter : MonoBehaviour {

	public Color filterColor = Color.black;

	// Use this for initialization
	void Awake () {
	
		GameObject filterObject = GameObject.Find("ColorFilter");

		if (filterObject != null){
			filterObject.GetComponent<SpriteRenderer>().color = filterColor;
		}

	}
}
