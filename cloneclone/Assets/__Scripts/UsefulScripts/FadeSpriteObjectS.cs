using UnityEngine;
using System.Collections;

public class FadeSpriteObjectS : MonoBehaviour {

	private EffectSpawnManagerS _myManager;
	public EffectSpawnManagerS myManager { get { return _myManager; } }

	private int _spawnCode = -1;
	private int spawnCode { get { return _spawnCode; } }

	public float fadeRate = 1f;
	public float delayFadeTime;
	private float startDelayFadeTime = -1f;
	public float startFadeAlpha = 1f;
	public bool destroyOnFade = true;
	public bool ignoreWitchTime = false;

	private bool stopFading = false;

	private SpriteRenderer myRenderer;
	private Color currentCol;

	public bool loopFade = false;
	public float loopDelay = 0.3f;
	private float loopDelayCountdown;
	public float loopAlphaReset;

	private float maxDrift = 0;
	private float maxYDrift = 0;
	private float currentDrift;
	private bool drifting = false;

	private float witchMult = 0.1f;


	// Use this for initialization
	void Awake () {

		myRenderer = GetComponent<SpriteRenderer>();
		currentCol = myRenderer.color;
		if (startFadeAlpha > -1){
			currentCol.a = startFadeAlpha;
		}
		myRenderer.color = currentCol;

		startDelayFadeTime = delayFadeTime;
		loopDelayCountdown = loopDelay;

	
	}
	
	// Update is called once per frame
	void Update () {

	
		if (!stopFading){
		if (delayFadeTime > 0){
				if (PlayerSlowTimeS.witchTimeActive && !ignoreWitchTime){
					
						delayFadeTime -= Time.deltaTime*witchMult;

				}else{
					delayFadeTime -= Time.deltaTime;
				}
				if (drifting){
					if (PlayerSlowTimeS.witchTimeActive && !ignoreWitchTime){
							currentDrift = maxDrift*0.5f;
							transform.position += currentDrift*Time.deltaTime*transform.up*witchMult;
							currentDrift = maxYDrift*0.5f;
							transform.position += currentDrift*Time.deltaTime*transform.right*witchMult;

					}else{
					currentDrift = maxDrift*0.5f;
					transform.position += currentDrift*Time.deltaTime*transform.up;
					currentDrift = maxYDrift*0.5f;
					transform.position += currentDrift*Time.deltaTime*transform.right;
					}
				}
		}
		else{
			currentCol = myRenderer.color;
				if (PlayerSlowTimeS.witchTimeActive && !ignoreWitchTime){
					currentCol.a -= Time.deltaTime*fadeRate*witchMult;
				}else{
					currentCol.a -= Time.deltaTime*fadeRate;
				}
			if (currentCol.a <= 0f){
			if (destroyOnFade){
					if (!_myManager){
					Destroy(gameObject);
					}
					else{
						_myManager.Despawn(gameObject, _spawnCode);
					}
				}else{
					currentCol.a = 0f;
					}
					stopFading = true;
				}else{
					if (drifting){
						if (PlayerSlowTimeS.witchTimeActive && !ignoreWitchTime){
							currentDrift = maxDrift;
							transform.position += currentDrift*Time.deltaTime*transform.up*witchMult;
							currentDrift = maxYDrift;
							transform.position += currentDrift*Time.deltaTime*transform.right*witchMult;
						}else{
						currentDrift = maxDrift;
						transform.position += currentDrift*Time.deltaTime*transform.up;
						currentDrift = maxYDrift;
						transform.position += currentDrift*Time.deltaTime*transform.right;
						}
					}
				}
			myRenderer.color = currentCol;
		}
		}else{
			if (loopFade){
				if (PlayerSlowTimeS.witchTimeActive && !ignoreWitchTime){
					loopDelayCountdown -= Time.deltaTime*witchMult;
				}else{
				loopDelayCountdown -= Time.deltaTime;
				}
				if (loopDelayCountdown <= 0){
					stopFading = false;
					Color resetCol = myRenderer.color;
					resetCol.a = loopAlphaReset;
					myRenderer.color = resetCol;
					loopDelayCountdown = loopDelay;
				}
			}
		}

	
	}

	public void Reinitialize(){

		delayFadeTime = startDelayFadeTime;
		stopFading = false;

		if (!myRenderer){
			myRenderer = GetComponent<SpriteRenderer>();
		}

		currentCol = myRenderer.color;
		currentCol.a = startFadeAlpha;
		myRenderer.color = currentCol;
	}

	public void SetManager(EffectSpawnManagerS e, int sCode = -1){
		_myManager = e;
		_spawnCode = sCode;
		Reinitialize();
	}

	public void SetTempFadeDelay(float newFadeDelay){
		if (startDelayFadeTime < 0){
			startDelayFadeTime = delayFadeTime;
		}
		delayFadeTime = newFadeDelay;
	}

	public void SetDrift(float newDrift){
		if (newDrift != 0){
			drifting = true;
			maxDrift = currentDrift = newDrift;
			//if (transform.localScale.x < 0){
			//	maxDrift*=-1f;
			//	currentDrift = maxDrift;
			//}
		}else{
			drifting = false;
		}
	}

	public void SetYDrift(float newDrift){
		if (newDrift != 0){
			drifting = true;
			maxYDrift = currentDrift = newDrift;
			//if (transform.localScale.x < 0){
			//	maxDrift*=-1f;
			//	currentDrift = maxDrift;
			//}
		}else{
			drifting = false;
		}
	}
}
