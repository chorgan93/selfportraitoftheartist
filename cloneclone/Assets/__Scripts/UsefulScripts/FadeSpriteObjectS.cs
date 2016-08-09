using UnityEngine;
using System.Collections;

public class FadeSpriteObjectS : MonoBehaviour {

	private EffectSpawnManagerS _myManager;
	public EffectSpawnManagerS myManager { get { return _myManager; } }

	private int _spawnCode = -1;
	private int spawnCode { get { return _spawnCode; } }

	public float fadeRate = 1f;
	public float delayFadeTime;
	private float startDelayFadeTime;
	public float startFadeAlpha = 1f;
	public bool destroyOnFade = true;

	private SpriteRenderer myRenderer;
	private Color currentCol;


	// Use this for initialization
	void Start () {

		myRenderer = GetComponent<SpriteRenderer>();
		currentCol = myRenderer.color;
		currentCol.a = startFadeAlpha;
		myRenderer.color = currentCol;

		startDelayFadeTime = delayFadeTime;

	
	}
	
	// Update is called once per frame
	void Update () {

	

		if (delayFadeTime > 0){
			delayFadeTime -= Time.deltaTime;
		}
		else{
			currentCol = myRenderer.color;
			currentCol.a -= Time.deltaTime*fadeRate;
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
			}
			myRenderer.color = currentCol;
		}

	
	}

	public void Reinitialize(){

		delayFadeTime = startDelayFadeTime;

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
}
