using UnityEngine;
using System.Collections;

public class MuzzleFlareS : MonoBehaviour {

	private float fadeTime;
	private float fadeTimeCountdown;
	private float fadeMult;

	public float spawnDistance = 0.5f;

	private Vector3 originalScale;
	private float originalZRotation;

	private SpriteRenderer mySpriteRenderer;
	private Color fadeColor;

	// Use this for initialization
	void Start () {

		mySpriteRenderer = GetComponent<SpriteRenderer>();
		mySpriteRenderer.enabled = false;

		originalScale = transform.localScale;

		originalZRotation = transform.localRotation.z;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (mySpriteRenderer.enabled){

			fadeTimeCountdown -= Time.deltaTime;

			if (fadeTimeCountdown <= 0){
				mySpriteRenderer.enabled = false;
			}
			else{
				fadeMult = fadeTimeCountdown/fadeTime;

				fadeColor = mySpriteRenderer.color;
				fadeColor.a = fadeMult;
				mySpriteRenderer.color = fadeColor;
			}

		}
	
	}

	public void Fire(float flashTime, Vector3 direction, float size){

		Vector3 newScale = originalScale;
		newScale.x += size*originalScale.x*0.5f;
		newScale.y += size*originalScale.x*0.5f;
		transform.localScale = newScale;

		float rotateZ = 0;
		
		Vector3 targetDir = direction.normalized;
		
		if(targetDir.x == 0){
			if (targetDir.y > 0){
				rotateZ = 90;
			}
			else{
				rotateZ = -90;
			}
		}
		else{
			rotateZ = Mathf.Rad2Deg*Mathf.Atan((targetDir.y/targetDir.x));
		}	
		
		
		if (targetDir.x < 0){
			rotateZ += 180;
		}

		
		
		transform.rotation = Quaternion.Euler(new Vector3(0,0,rotateZ+originalZRotation));

		transform.localPosition = direction.normalized*(newScale.x/originalScale.x);

		fadeTime = fadeTimeCountdown = flashTime;
		fadeColor = mySpriteRenderer.color;
		fadeColor.a = 1;
		mySpriteRenderer.color = fadeColor;
		mySpriteRenderer.enabled = true;

	}


}
