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
	private float bigFOVScalar = 1.1f;
	private float smallFOVScalar = 0.97f;

	[Header("Special Scene Properties")]
	public bool arcadeMode = false;

	// Use this for initialization
	void Start () {

		newScale = originalScale = transform.localScale;
		scaleChangeCountdown = scaleChangeTime;
		/*if (arcadeMode){
			GetComponent<SpriteRenderer>().enabled = false;
		}**/
	
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
				newScale*=CameraFollowS.ZOOM_LEVEL*CameraFollowS.F.orthoMultRef;
				if (CameraFollowS.ZOOM_LEVEL > 1){
					newScale*=bigFOVScalar;
				}
				if (CameraFollowS.ZOOM_LEVEL < 1){
					newScale*=smallFOVScalar;
				}
				transform.localScale = newScale;

			}
		}
	
	}
}
