using UnityEngine;
using System.Collections;

public class TimelineS : MonoBehaviour {

	private ControlManagerS myControl;
	private bool hasMoved = false;
	public Vector3 arrowOffset = new Vector3(0,1,0);
	public Vector3 dateOffset = new Vector3(0,-0.75f,0);
	public SpriteRenderer[] timelineSprites;
	public SpriteRenderer[] timelineSpriteOutlines;
	public string[] timelineDates;
	public string[] timelinePhases;
	public Color[] bgColors;

	private int currentPos = 0;

	public SpriteRenderer[] arrowSprite;
	public TextMesh[] timelineDateTexts;
	public TextMesh[] timelinePhaseTexts;

	public Sprite timelineSpriteOn;
	public Sprite timelineSpriteOff;

	// Use this for initialization
	void Start () {
	
		myControl = GetComponent<ControlManagerS>();

		SetPosition(0);

	}
	
	// Update is called once per frame
	void Update () {

		if (hasMoved){
			if (Mathf.Abs(myControl.HorizontalMenu()) < 0.1f){
				hasMoved = false;
			}
		}else{
			if (myControl.HorizontalMenu() > 0.1f){
				hasMoved = true;
				SetPosition(currentPos+1);
			}
			if (myControl.HorizontalMenu() < -0.1f){
				hasMoved = true;
				SetPosition(currentPos-1);
			}
		}
	
	}

	void SetPosition(int newPos){
		timelineSprites[currentPos].sprite = timelineSpriteOff;
		timelineSprites[currentPos].color = Color.white;
		timelineSpriteOutlines[currentPos].gameObject.SetActive(false);
		currentPos = newPos;
		if (currentPos >= timelineSprites.Length){
			currentPos = 0;
		}
		if (currentPos < 0){
			currentPos = timelineSprites.Length-1;
		}

		timelineSprites[currentPos].sprite = timelineSpriteOn;
		timelineSprites[currentPos].color = bgColors[currentPos];
		timelineSpriteOutlines[currentPos].color = bgColors[currentPos];
		timelineSpriteOutlines[currentPos].gameObject.SetActive(true);
		for (int i = 0; i < arrowSprite.Length; i++){
			arrowSprite[i].transform.position = timelineSprites[currentPos].transform.position+arrowOffset;

		}
		for (int i = 0; i < timelineDateTexts.Length; i++){
			timelineDateTexts[i].text = timelineDates[currentPos];
			if (i != 0){
				timelineDateTexts[i].color = bgColors[currentPos];
			}
			timelineDateTexts[i].transform.position = timelineSprites[currentPos].transform.position+dateOffset;
		}
		for (int i = 0; i < timelinePhaseTexts.Length; i++){
			timelinePhaseTexts[i].text = timelinePhases[currentPos];
			if (i != 0){
				timelinePhaseTexts[i].color = bgColors[currentPos];
			}
		}
	}
}
