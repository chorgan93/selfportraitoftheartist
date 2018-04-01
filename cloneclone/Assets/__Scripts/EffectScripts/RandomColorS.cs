using UnityEngine;
using System.Collections;

public class RandomColorS : MonoBehaviour {

	private SpriteRenderer myRender;
	private Color currentCol;
	private Color nextCol;
	public Color[] colorSequence;
	private int currentColorNum = 0;
	public float colorChangeRate = 0.12f;
	private float colorChangeCountdown;

	// Use this for initialization
	void Start () {
		colorChangeCountdown = colorChangeRate;
		myRender = GetComponent<SpriteRenderer>();
		currentCol = myRender.color;
		currentColorNum = Mathf.FloorToInt(Random.Range(0, colorSequence.Length));
		nextCol = colorSequence[currentColorNum];
		nextCol.a = currentCol.a;
		myRender.color = nextCol;
	}
	
	// Update is called once per frame
	void Update () {
		colorChangeCountdown -= Time.deltaTime;
		if (colorChangeCountdown <= 0){
			colorChangeCountdown = colorChangeRate;
			currentColorNum++;
			if (currentColorNum > colorSequence.Length-1){
				currentColorNum = 0;
			}
			currentCol = myRender.color;
			nextCol = colorSequence[currentColorNum];
			nextCol.a = currentCol.a;
			myRender.color = nextCol;
		}
	}
}
