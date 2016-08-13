using UnityEngine;
using System.Collections;

public class TitleTextEffectS : MonoBehaviour {

	public int numFlashesMin = 1;
	public int numFlashesMax = 4;
	private int currentFlash;

	public int numFramesMin = 2;
	public int numFramesMax = 6;
	private int currentFrame;

	public float hideTimeMin = 0.2f;
	public float hideTimeMax = 2f;
	private float hideTime;

	private Vector3 startPos;
	public float posVariance = 3f;

	private float startSize;
	public float sizeVariance = 200f;

	private TextMesh myMesh;
	private string startText;

	// Use this for initialization
	void Start () {

		startPos = transform.position;

		myMesh = GetComponent<TextMesh>();
		startSize = myMesh.fontSize;
		startText = myMesh.text;

		hideTime = GetHideTime();
		myMesh.text = "";

		currentFlash = GetNumFlashes();
		SetUpNewFlash();

	
	}
	
	// Update is called once per frame
	void Update () {

		if (hideTime > 0){
			if (myMesh.text != ""){
				myMesh.text = "";
			}
			hideTime -= Time.deltaTime;
		}else{
			if (myMesh.text != startText){
				myMesh.text = startText;
			}
			currentFrame--;
			if (currentFrame <= 0){
				currentFlash--;
				if (currentFlash <= 0){
					hideTime = GetHideTime();
					currentFlash = GetNumFlashes();
				}else{
					SetUpNewFlash();
				}
			}
		}
	
	}

	float GetHideTime(){
		return (Random.Range(hideTimeMin, hideTimeMax));
	}

	int GetNumFlashes(){
		return (Mathf.RoundToInt(Random.Range(numFlashesMin, numFlashesMax)));
	}

	int GetNumFrames(){
		return (Mathf.RoundToInt(Random.Range(numFramesMin, numFramesMax)));
	}

	void SetUpNewFlash(){
		
		currentFrame = GetNumFrames();
		myMesh.fontSize = Mathf.RoundToInt(startSize+Random.insideUnitCircle.x*sizeVariance);
		transform.position = startPos+Random.insideUnitSphere*posVariance;
	}
}
