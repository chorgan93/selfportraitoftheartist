using UnityEngine;
using System.Collections;

public class SpriteDistortionS : MonoBehaviour {

	private SpriteRenderer mySprite;
	private SpriteRenderer parentSprite;

	public float changeRate = 0.08f;
	private float changeCountdown = 0f;
	public float changeSizeAmt = 0.2f;

	public float changePosAmtX = 0.015f;
	public float changePosAmtY = 0.0075f;
	private Vector3 currentPos;

	public bool matchColor = false;
	public bool matchAlpha = false;
	private Color matchAlphaCol;

	// Use this for initialization
	void Start () {


		mySprite = GetComponent<SpriteRenderer>();
		parentSprite = transform.parent.GetComponent<SpriteRenderer>();
		if(matchColor){
		mySprite.material.SetColor("_FlashColor", parentSprite.color);
		}
		if (matchAlpha){
			matchAlphaCol = mySprite.color;
		}
        mySprite.flipX = parentSprite.flipX;
        mySprite.flipY = parentSprite.flipY;
		mySprite.sprite = parentSprite.sprite;
		ChangeSize();

	}
	
	// Update is called once per frame
	void Update () {


		if (mySprite.enabled){
			mySprite.sprite = parentSprite.sprite;

			if (matchColor){
				mySprite.color = parentSprite.color;
			}
			if (matchAlpha){
				matchAlphaCol.a = parentSprite.color.a;
				mySprite.color = matchAlphaCol;
			}
	

				changeCountdown -= Time.deltaTime;

			if (changeCountdown <= 0){
				ChangeSize();
			}
		}
	
	}

	private void ChangeSize(){
	
			transform.localScale = Vector3.one+Random.insideUnitSphere*changeSizeAmt;

		changeCountdown = changeRate;currentPos = Vector3.zero;
		currentPos.x += changePosAmtX*Random.insideUnitCircle.x;
		currentPos.y += changePosAmtY*Random.insideUnitCircle.y;
		currentPos.z = transform.localPosition.z;
		transform.localPosition = currentPos;
	}
}
