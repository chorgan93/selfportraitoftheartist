using UnityEngine;
using System.Collections;

public class VignetteScalingEffectS : MonoBehaviour {

	public float startOffset = 0.04f;
	public float scaleChangeTime = 0.083f;
	private float scaleChangeCountdown;

	public float xChangeAmt = 0.8f;
	public float yChangeAmt = 0f;
	private Vector3 newScale;
	private Vector3 originalScale;

	// Use this for initialization
	void Start () {

		newScale = originalScale = transform.localScale;
		scaleChangeCountdown = scaleChangeTime;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (startOffset <= 0){
			startOffset -= Time.deltaTime;
		}
		else{
			scaleChangeCountdown -= Time.deltaTime;
			if (scaleChangeCountdown <= 0){
				scaleChangeCountdown = scaleChangeTime;
	
				newScale = originalScale;
				newScale.x += Random.insideUnitCircle.x * xChangeAmt;
				newScale.y += Random.insideUnitCircle.y * yChangeAmt;
				transform.localScale = newScale;
			}
		}
	
	}
}
