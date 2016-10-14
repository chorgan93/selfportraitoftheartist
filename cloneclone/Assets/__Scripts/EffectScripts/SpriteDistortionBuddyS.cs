using UnityEngine;
using System.Collections;

public class SpriteDistortionBuddyS : MonoBehaviour {

	private SpriteRenderer mySprite;
	private SpriteRenderer parentSprite;

	public float changeRate = 0.08f;
	private float changeCountdown = 0f;
	public float changeSizeAmt = 0.2f;

	private BuddyS buddyReference;

	// Use this for initialization
	void Start () {

		buddyReference = transform.GetComponentInParent<BuddyS>();

		mySprite = GetComponent<SpriteRenderer>();
		parentSprite = transform.parent.GetComponent<SpriteRenderer>();
		mySprite.material.SetColor("_FlashColor", buddyReference.shadowColor);
		mySprite.sprite = parentSprite.sprite;
		ChangeSize();

	}
	
	// Update is called once per frame
	void Update () {


		if (mySprite.enabled){
			mySprite.sprite = parentSprite.sprite;
	

				changeCountdown -= Time.deltaTime;

			if (changeCountdown <= 0){
				ChangeSize();
			}
		}
	
	}

	private void ChangeSize(){
	
			transform.localScale = Vector3.one+Random.insideUnitSphere*changeSizeAmt;

		changeCountdown = changeRate;
	}
}
