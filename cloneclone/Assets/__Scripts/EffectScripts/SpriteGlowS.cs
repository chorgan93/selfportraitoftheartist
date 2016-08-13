using UnityEngine;
using System.Collections;

public class SpriteGlowS : MonoBehaviour {

	public float xSizeVar;
	public float ySizeVar;
	private Vector3 startSize;

	private SpriteRenderer myRenderer;
	private Color startColor;

	public float minAlpha = 0.4f;
	public float maxAlpha = 0.8f;

	public float changeRate = 0.083f;
	private float changeCountdown;

	// Use this for initialization
	void Start () {

		startSize = transform.localScale;

		myRenderer = GetComponent<SpriteRenderer>();
		startColor = myRenderer.color;

		changeCountdown = 0f;
	
	}
	
	// Update is called once per frame
	void Update () {

		changeCountdown -= Time.deltaTime;
		if (changeCountdown <= 0){
			Color newCol = startColor;
			newCol.a = FindNewAlpha();
			myRenderer.color = newCol;

			Vector3 newSize = startSize;
			newSize.x += xSizeVar*Random.insideUnitCircle.x;
			newSize.y += ySizeVar*Random.insideUnitCircle.y;
			transform.localScale = newSize;

			changeCountdown = changeRate;
		}
	
	}

	float FindNewAlpha(){
		return (Random.Range(minAlpha, maxAlpha));
	}
}
