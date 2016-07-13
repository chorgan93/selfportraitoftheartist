using UnityEngine;
using System.Collections;

public class RefreshDisplayS : MonoBehaviour {

		
		private Renderer myRenderer;
		private PlayerController myPlayer;
		private Quaternion startRotation;
		private Vector3 rotateRate = new Vector3(0,600,0f);
		private float rotateCountdownMax = 0.083f;
		private float rotateCountdown;
		
		private int flashFramesMax = 6;
		private int currentFlashFrames = 0;
		public float fullAlpha = 0.75f;
		public float noAlpha = 0f;
		private Color currentColor;
		private Color startColor;
		private bool isFlashing = false;
		
		// Use this for initialization
		void Start () {
			
			myRenderer = GetComponent<Renderer>();
			myPlayer = GetComponentInParent<PlayerController>();
			startRotation = transform.rotation;
			startColor = myRenderer.material.color;
			myRenderer.enabled = false;
			myPlayer.myStats.AddRefresh(this);
			
		}
		
		// Update is called once per frame
		void Update () {

				
				if (isFlashing){
					currentFlashFrames--;
			if (currentFlashFrames < flashFramesMax-1){
				currentColor = startColor;
				currentColor.a = noAlpha + myPlayer.myStats.currentMana/myPlayer.myStats.maxMana*(fullAlpha-noAlpha);
				myRenderer.material.color = currentColor;
					if (currentFlashFrames <= 0){
						isFlashing = false;
						myRenderer.material.color = currentColor;
						myRenderer.enabled = false;
					}
			}
				}
				

			
		}
		
		public void DoFlash(){
			
			isFlashing = true;
			currentColor = Color.white;
			myRenderer.enabled = true;
			currentFlashFrames = flashFramesMax;
			
		}

	}

