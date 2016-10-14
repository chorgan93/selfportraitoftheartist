using UnityEngine;
using System.Collections;

public class FlashEffectS : MonoBehaviour {

	public float fadeRate = 10f;
	public float maxFade = 0.4f;
	
	private SpriteRenderer mySprite;
	private Color myColor;

	// Use this for initialization
	void Start () {

		mySprite = GetComponent<SpriteRenderer>();
		myColor = mySprite.color;
		myColor.a = 0f;
		mySprite.color = myColor;
		mySprite.enabled = false;
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (mySprite.enabled){
			myColor = mySprite.color;
			myColor.a -= fadeRate*Time.deltaTime;
			if (myColor.a <= 0){
				mySprite.enabled = false;
			}
			mySprite.color = myColor;
		}

	}

	public void NewColor(Color newCol){
		myColor = newCol;
	}

	public void Flash(){
	
		myColor.a = maxFade;
		mySprite.color = myColor;
		mySprite.enabled = true;
	}
}
