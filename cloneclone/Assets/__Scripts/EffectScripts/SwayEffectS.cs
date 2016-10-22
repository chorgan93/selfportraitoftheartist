using UnityEngine;
using System.Collections;

public class SwayEffectS : MonoBehaviour {
	
	public Vector3 skyOffset;
	public float swayIntensity = 1.5f;
	public float currentSwayTime = 0;
	public float swayDuration = 3f;
	
	public bool swayUp = true;

	
	Vector3 startPos;
	
	void Start () {
		
		startPos = transform.position;
		currentSwayTime = swayDuration / 2f;
		
	
		
		
	}
	
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (swayUp){
			currentSwayTime += Time.deltaTime;
			if (currentSwayTime >= swayDuration){
				swayUp = !swayUp;
			}
		}
		else{
			currentSwayTime -= Time.deltaTime;
			if (currentSwayTime <= 0){
				swayUp = !swayUp;
			}
		}
		
		
		float t = AnimCurveS.QuadEaseInOut(currentSwayTime, 0, swayIntensity, swayDuration);
		
		
		skyOffset.y = t - swayIntensity/2f;
		
		transform.position = startPos + skyOffset;
		

		
	}
}
