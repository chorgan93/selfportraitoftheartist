using UnityEngine;
using System.Collections;

public class ChangeSpriteColorsS : MonoBehaviour {

	public SpriteRenderer[] changeSprites;
	public Color newColor;

	// Use this for initialization
	void Start () {
	
		if (changeSprites.Length > 0){
			for (int i = 0; i < changeSprites.Length; i++){
				changeSprites[i].color = newColor;
			}
		}
	}

}
