using UnityEngine;
using System.Collections;

public class FlashMatS : MonoBehaviour {

	public int flashFrames = 8;
	private bool flashReset = false;

	private SpriteRenderer myRender;

	void Start(){
		myRender = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

		if (!flashReset){
		flashFrames --;
		if (flashFrames <= 0){
				myRender.material.SetFloat("_FlashAmount", 0f);
				flashReset = true;
		}
		}
	
	}
}
