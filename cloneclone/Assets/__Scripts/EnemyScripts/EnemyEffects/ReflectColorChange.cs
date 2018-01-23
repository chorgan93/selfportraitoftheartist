using UnityEngine;
using System.Collections;

public class ReflectColorChange : MonoBehaviour {

	private bool _activated = false;
	public bool activated { get { return _activated; } }
	private SpriteRenderer myRenderer;
	private int currentCol = 0;
	public Color[] colorsToSwitch;
	private float whiteTimeMax = 0.2f;
	private float whiteCountdown = 0f;
	private float changeColorMax = 0.1f;
	private float changeColorCountdown;

	public SpawnOnProjectileS mySpawner;

	// Use this for initialization
	void Start () {
		myRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (_activated){
			if (whiteCountdown > 0){
				whiteCountdown -= Time.deltaTime;
			}else{
				changeColorCountdown -= Time.deltaTime;
				if (changeColorCountdown <= 0){
					currentCol++;
					if (currentCol >= colorsToSwitch.Length){
						currentCol = 0;
					}
					myRenderer.color = colorsToSwitch[currentCol];
					if (mySpawner){
						mySpawner.SetNewParticleColor(colorsToSwitch[currentCol]);
					}
					changeColorCountdown = changeColorMax;
				}
			}
		}

	}

	public void ActivateReflect(){
		whiteCountdown = whiteTimeMax;
		_activated = true;
		currentCol = -1;
		changeColorCountdown = 0;
		myRenderer.color = Color.white;
	}
}
