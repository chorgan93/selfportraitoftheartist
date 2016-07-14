using UnityEngine;
using System.Collections;

public class BlockDisplayS : MonoBehaviour {

	private Renderer myRenderer;
	private PlayerController myPlayer;

	private float fadeIntervalRate = 0.03f;
	private float intervalCountdown;
	public int numOfIntervals = 24;
	private int currentInterval;
	public float maxAlpha = 1f;
	private float alphaInterval;
	private Color myColor;
	private Color startColor;
	private bool completedFlash = false;


	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<Renderer>();
		myPlayer = GetComponentInParent<PlayerController>();
		myRenderer.enabled = false;
		startColor = myRenderer.material.color;

		alphaInterval = maxAlpha/(numOfIntervals*1f);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (myPlayer.isBlocking){

			if (!myRenderer.enabled && !completedFlash){
				DoFlash();
			}else{
				intervalCountdown -= Time.deltaTime;
				if (intervalCountdown <= 0){
					intervalCountdown = fadeIntervalRate;
					currentInterval++;
					if (currentInterval >= numOfIntervals){
						myRenderer.enabled = false;
						completedFlash = true;
					}else{
						myColor = startColor;
						myColor.a = myRenderer.material.color.a;
						myColor.a -= alphaInterval;
						myRenderer.material.color = myColor;
					}
				}
			}
		}else{
			if (myRenderer.enabled){
				myRenderer.enabled = false;
			}
			completedFlash = false;
		}
	
	}

	void DoFlash(){


		myColor = Color.white;
		myColor.a = maxAlpha;
		myRenderer.material.color = myColor;
		myRenderer.enabled = true;
		currentInterval = 0;
		intervalCountdown = fadeIntervalRate;

	}
}
