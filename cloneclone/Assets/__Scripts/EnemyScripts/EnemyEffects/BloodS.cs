﻿using UnityEngine;
using System.Collections;

public class BloodS : MonoBehaviour {
	
	public Color startColor;
	public Color kidStartColor;
	public Color[] bloodColors;
	public Color[] kidBloodColors;
	public Sprite[] bloodSprites;
	private bool kidsMode = false;
	
	public float waitToAppearTime = 0.02f;
	private float waitToAppearCountdown;
	
	public float bloodAnimRate = 0.083f;
	private float bloodAnimCountdown;
	private int currentSprite;
	
	public float bloodColorRate = 0.083f;
	private float bloodColorCountdown;
	private int currentColor;
	
	public int startFlashAmt = 6;
	private int currentFlashFrame = 0;
	
	private SpriteRenderer myRenderer;
	private bool initialized = false;
	
	public int bloodSpriteNum = 0;
	
	// Use this for initialization
	void Start () {
		
		if (!StartRaycast()){
			Destroy(gameObject);
		}
		else{
			Initialize();
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!initialized){
			Initialize();
		}else{
			if (waitToAppearCountdown > 0){
				waitToAppearCountdown -= Time.deltaTime;
			}
			else if (currentFlashFrame > 0){
				currentFlashFrame--;
				if (!myRenderer.enabled){
					myRenderer.enabled = true;
				}
			}else{
				if (currentSprite < bloodSprites.Length-1){
					bloodAnimCountdown -= Time.deltaTime;
					if (bloodAnimCountdown <= 0){
						bloodAnimCountdown = bloodAnimRate;
						currentSprite++;
						myRenderer.sprite = bloodSprites[currentSprite];
					}
				}
				
				if (currentColor < bloodColors.Length-1){
					bloodColorCountdown -= Time.deltaTime;
					if (bloodColorCountdown <= 0){
						bloodColorCountdown = bloodColorRate;
						currentColor++;
						if (kidsMode){
							myRenderer.color = kidBloodColors[currentColor];
							if (currentColor == kidBloodColors.Length-1){
								enabled = false;
							}
						}else{
							myRenderer.color = bloodColors[currentColor];
							if (currentColor == bloodColors.Length-1){
								enabled = false;
							}
						}
					}                                       
				}
			}
		}
		
	}
	
	void Initialize(){
		
		if (!initialized){
			
			myRenderer = GetComponent<SpriteRenderer>();
			//myRenderer.enabled = false;
			if (kidsMode){
				myRenderer.color = startColor;
			}else{
				myRenderer.color = kidStartColor;
			}
			myRenderer.sprite = bloodSprites[0];
			
			bloodAnimCountdown = 0;
			currentSprite = 0;
			bloodColorCountdown = 0;
			currentColor = 0;
			
			waitToAppearCountdown = waitToAppearTime;
			currentFlashFrame = startFlashAmt;
			
			PlayerInventoryS.I.dManager.AddBlood(Application.loadedLevel, transform.position, bloodSpriteNum, gameObject);
			
			initialized = true;
		}
		
	}
	
	public void AddWaitTime (float f){
		waitToAppearTime += f;
	}
	
	private bool StartRaycast(){
		
		bool amIAboveGround = false;
		
		RaycastHit hit;
		
		Physics.Raycast(transform.position, new Vector3(0,0,1f), out hit, 30f);
		
		if (hit.collider != null){
			if (hit.collider.gameObject.tag == "Background"){
				amIAboveGround = false;
			}else{
				amIAboveGround = true;
			}
		}else{
			amIAboveGround = false;
		}
		
		return amIAboveGround;
		
	}
}
