using UnityEngine;
using System.Collections;

public class BGColorS : MonoBehaviour {

	
	public Color[] colors;
	private Color currentColor;
	private Color targetColor;
	public float colorChangeTime = 10f;
	private float colorChangeT;
	
	private Camera myCam;
	
	Vector3 startPos;
	
	void Start () {
		
		colorChangeT = colorChangeTime;
		
		if (colors.Length > 0){
			targetColor = colors[Mathf.FloorToInt(Random.Range(0, colors.Length))];
			myCam = GetComponent<Camera>();
		}
		
		
	}
	
	
	// Update is called once per frame
	void FixedUpdate () {
		
	
		
		// change colors
		if (colors.Length > 0){
			colorChangeT -= Time.deltaTime;
			
			float tC = (colorChangeTime-colorChangeT)/colorChangeTime;
			if (tC > 1){
				tC = 1;
			}
			myCam.backgroundColor = Color.Lerp(currentColor, targetColor, tC);
			
			if (colorChangeT <= 0){
				colorChangeT = colorChangeTime;
				currentColor = myCam.backgroundColor;
				targetColor = colors[Mathf.FloorToInt(Random.Range(0, colors.Length))];
			}
		}
		
		
	}
}

