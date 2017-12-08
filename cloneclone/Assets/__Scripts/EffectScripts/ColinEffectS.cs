using UnityEngine;
using System.Collections;

public class ColinEffectS : MonoBehaviour {

	public Color[] flashColors;
	public float colorChangeRate = 0.2f;
	private float colorCountdown;
	private int currentColor = 0;

	private SpriteRenderer mySprite;

	// Use this for initialization
	void Start () {

		mySprite = GetComponent<SpriteRenderer>();
		mySprite.color = flashColors[0];
		colorCountdown = colorChangeRate;
	
	}
	
	// Update is called once per frame
	void Update () {

		colorCountdown -= Time.deltaTime;
		if (colorCountdown <= 0){
			colorCountdown = colorChangeRate;
			currentColor++;
			if (currentColor >= flashColors.Length){
				currentColor = 0;
			}
			mySprite.color = flashColors[currentColor];
		}
	
	}
}
