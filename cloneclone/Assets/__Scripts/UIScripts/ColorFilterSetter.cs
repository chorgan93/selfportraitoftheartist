using UnityEngine;
using System.Collections;

public class ColorFilterSetter : MonoBehaviour {

	public Color filterColor = Color.black;
	public Color raysColor = new Color(0,1,1);

	// Use this for initialization
	void Awake () {
	
		GameObject filterObject = GameObject.Find("ColorFilter");

		if (filterObject != null){
			filterObject.GetComponent<SpriteRenderer>().color = filterColor;
		}


	}

	void Start(){
		if (CameraEffectsS.E != null){
			
			CameraEffectsS.E.SetRaysColor(raysColor);
		}
	}
}
