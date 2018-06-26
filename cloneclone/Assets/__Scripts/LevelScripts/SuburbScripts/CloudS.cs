using UnityEngine;
using System.Collections;

public class CloudS : MonoBehaviour {

	public float driftSpeed;
	private float currentDriftSpeed;
	public float speedVariation;
	public Vector3 driftDirection;
	private Color currentCol;
	public float fadeRate = 0.25f;
	private Vector3 startPos;
	private Vector3 currentPos;
	public float yPositionVariation;
	public float lifeTime = 10f;
	private float currentLifeTime;
	public bool fadingIn = false;
	public bool fadingOut = false;
	private float maxA;

	private SpriteRenderer myRenderer;
	public SpriteRenderer matchAlphaRender;
	private float matchRenderMaxA;

	// Use this for initialization
	void Start () {

		transform.parent =null;
		myRenderer = GetComponent<SpriteRenderer>();
		maxA = myRenderer.color.a;
		if (matchAlphaRender){

			matchRenderMaxA = matchAlphaRender.color.a;
		}
		if (fadingIn){
			currentCol = myRenderer.color;
			currentCol.a = 0f;
			myRenderer.color = currentCol;
			if (matchAlphaRender){
				currentCol.a = matchRenderMaxA * currentCol.a/maxA;
				matchAlphaRender.color = currentCol;
			}
		}
		currentDriftSpeed = driftSpeed+speedVariation*Random.Range(-1f, -0.1f);
		startPos = currentPos = transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (fadingIn){
			currentCol = myRenderer.color;
			currentCol.a += fadeRate*Time.deltaTime;
			if (currentCol.a >= maxA){
				currentCol.a = maxA;
				fadingIn = false;
			}
			myRenderer.color = currentCol; 
			if (matchAlphaRender){
				currentCol.a = matchRenderMaxA * currentCol.a/maxA;
				matchAlphaRender.color = currentCol;
			}
		}else if (fadingOut){

			currentCol = myRenderer.color;
			currentCol.a -= fadeRate*Time.deltaTime;
			if (currentCol.a <= 0){
				currentCol.a = 0;
				myRenderer.color = currentCol;
				fadingOut = false;
				ResetCloud();
			}
			myRenderer.color = currentCol;
			if (matchAlphaRender){
				currentCol.a = matchRenderMaxA * currentCol.a/maxA;
				matchAlphaRender.color = currentCol;
			}
		}
		Drift();
	
	}

	void ResetCloud(){
		currentPos = startPos;
		currentPos.y += yPositionVariation*Random.insideUnitCircle.x;
		transform.position = currentPos;
		fadingIn = true;
		currentLifeTime = lifeTime;
		currentDriftSpeed = driftSpeed+speedVariation*Random.Range(-1f, -0.1f);
	}

	void Drift(){
		currentLifeTime -= Time.deltaTime;
		if (currentLifeTime <= 0 && !fadingOut){
			fadingOut = true;
		}
		currentPos = transform.position;
		currentPos += driftDirection*currentDriftSpeed*Time.deltaTime;
		transform.position = currentPos;
	}
}
