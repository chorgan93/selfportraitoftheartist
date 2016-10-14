﻿using UnityEngine;
using System.Collections;

public class SpriteDistortionPlayerS : MonoBehaviour {

	private SpriteRenderer mySprite;
	private SpriteRenderer parentSprite;

	public float changeRate = 0.08f;
	private float changeCountdown = 0f;
	public float changeSizeAmt = 0.2f;

	private PlayerStatsS playerReference;

	// Use this for initialization
	void Start () {

		playerReference = transform.GetComponentInParent<PlayerStatsS>();

		mySprite = GetComponent<SpriteRenderer>();
		parentSprite = transform.parent.GetComponent<SpriteRenderer>();
		mySprite.material.SetColor("_FlashColor", parentSprite.color);
		mySprite.sprite = parentSprite.sprite;
		ChangeSize();
	}
	
	// Update is called once per frame
	void Update () {

		if (playerReference.PlayerIsDead()){
			mySprite.enabled = false;
		}

		if (mySprite.enabled){
			if (parentSprite.color != mySprite.material.GetColor("_FlashColor")){
				mySprite.material.SetColor("_FlashColor", parentSprite.color);
			}
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
