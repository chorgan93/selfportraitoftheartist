using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VignetteEffectS : MonoBehaviour {

	private Image myImage;
	private float maxAlpha = 0.4f;
	private float fadeRate = 2f;
	private Color currentCol;

	public static VignetteEffectS V;

	void Awake(){

		V = this;

	}

	// Use this for initialization
	void Start () {

		myImage = GetComponent<Image>();
		myImage.enabled = false;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (myImage.enabled){
			currentCol = myImage.color;
			currentCol.a -= Time.deltaTime*fadeRate;
			if (currentCol.a <= 0){
				myImage.enabled = false;
				currentCol.a = 0;
			}
			myImage.color = currentCol;
		}
	
	}

	public void Flash(Color newCol){
		/*currentCol = newCol;
		currentCol.a = maxAlpha;
		myImage.color = currentCol;
		myImage.enabled = true;**/
	}
}
