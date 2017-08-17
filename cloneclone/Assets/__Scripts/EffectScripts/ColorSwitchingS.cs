using UnityEngine;
using System.Collections;

public class ColorSwitchingS : MonoBehaviour {

	public Color[] switchColors;
	private int currentCol = 0;
	public float colorSwitchRate = 0.33f;
	private float colorSwitchCountdown;
	public bool randomizeStart = true;

	private SpriteRenderer _myRenderer;

	// Use this for initialization
	void Start () {

		_myRenderer = GetComponent<SpriteRenderer>();
		currentCol = Mathf.FloorToInt(Random.Range(0, switchColors.Length));
		_myRenderer.color = switchColors[currentCol];
		colorSwitchCountdown = colorSwitchRate;
	
	}
	
	// Update is called once per frame
	void Update () {

		colorSwitchCountdown -= Time.deltaTime*WitchRate();
		if (colorSwitchCountdown <= 0){
			colorSwitchCountdown = colorSwitchRate;
			currentCol++;
			if (currentCol > switchColors.Length-1){
				currentCol = 0;
			}
			_myRenderer.color = switchColors[currentCol];
		}
	
	}

	float WitchRate(){
		if (PlayerSlowTimeS.witchTimeActive){
			return 0.1f;
		}else{
			return 1f;
		}
	}
}
