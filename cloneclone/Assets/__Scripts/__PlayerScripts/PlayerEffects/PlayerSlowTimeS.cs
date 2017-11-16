using UnityEngine;
using System.Collections;

public class PlayerSlowTimeS : MonoBehaviour {

	public Renderer _myRenderer;
	private Collider _myCollider;
	public GameObject outlineObj;
	public static bool witchTimeActive = false;
	[Header("Acting Properties")]
	public float witchTimeLength = 2f;
	public float witchTimeExtend = 0.8f;
	private float currentWitchTimeMax;
	public float witchTimeMax = 5f;
	private float currentWitchTime = 0f;
	[Header("Effect Properties")]
	public Vector3 growRate = new Vector3(3f, 2f, 0f);
	private Vector3 startScale;
	private Vector3 currentGrowScale = Vector3.zero;
	public float growStepTime = 0.3f;
	private float currentGrowStepTime;
	public float growStepMult = 0.88f;
	private float growStepCount;
	public int growIterations = 5;
	private int currentIteration;
	private bool endTriggered = false;

	private float shrinkMult = 3f;

	private PlayerController playerRef;

	// Use this for initialization
	void Start () {

		startScale = transform.localScale;

		playerRef = GetComponentInParent<PlayerController>();
		playerRef.SetWitchObject(this);

		_myRenderer.enabled = false;
		_myCollider = GetComponent<Collider>();
		_myCollider.enabled = false;

		outlineObj.SetActive(false);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (witchTimeActive){
			if (currentWitchTime < currentWitchTimeMax){
			if (currentIteration < growIterations){
				growStepCount -= Time.deltaTime;
				if (growStepCount <= 0){
					currentIteration++;
					currentGrowScale.x *= growRate.x;
					currentGrowScale.y *= growRate.y;
					transform.localScale = currentGrowScale;
					growStepCount = currentGrowStepTime;
					currentGrowStepTime *= growStepMult;

					if (currentIteration >= growIterations){
						currentGrowStepTime = growStepTime;
					}
				}
			}
			currentWitchTime+=Time.deltaTime;
			}
			else{
				endTriggered = true;
				growStepCount -= Time.deltaTime*shrinkMult;
				if (growStepCount <= 0){
					currentIteration--;
					currentGrowScale.x /= growRate.x;
					currentGrowScale.y /= growRate.y;
					transform.localScale = currentGrowScale;
					growStepCount = currentGrowStepTime;
					currentGrowStepTime *= growStepMult;

				}
				if (currentIteration <= 0){
				EndWitchTime();
				}
			}
		}
	
	}

	public void TriggerWitchTime(){
		currentIteration = 0;
		currentGrowScale = startScale;
		outlineObj.SetActive(true);
		growStepCount = 0f;
		witchTimeActive = true;
		currentWitchTimeMax = witchTimeLength;
		currentWitchTime = 0f;
		_myCollider.enabled = _myRenderer.enabled = true;
		CameraEffectsS.E.SetContrast(false);
		currentGrowStepTime = growStepTime;
		if (playerRef.currentCombatManager){
			playerRef.currentCombatManager.SetWitchTime(true);
		}
		endTriggered = false;
		BGMHolderS.BG.SetWitch(true);
	}

	public void ExtendWitchTime(){
		if (witchTimeActive && !endTriggered){
		currentWitchTimeMax += witchTimeExtend;
		if (currentWitchTimeMax > witchTimeMax){
			currentWitchTimeMax = witchTimeMax;
		}
		}
	}
	public void EndWitchTime(bool fromReset = false){
		if (witchTimeActive || fromReset){

			BGMHolderS.BG.SetWitch(false);
		}
		witchTimeActive = false;
		outlineObj.SetActive(false);
		_myCollider.enabled = _myRenderer.enabled = false;
		transform.localScale = startScale;
		playerRef.EndWitchTime(true);
		CameraEffectsS.E.SetContrast(true);
		if (playerRef.currentCombatManager){
			playerRef.currentCombatManager.SetWitchTime(false);
		}
	}
}
