using UnityEngine;
using System.Collections;

public class ChargeAttackS : MonoBehaviour {

	private Renderer _myRenderer;
	private float _animateRate = 0.033f;
	private float animateCountdown;
	private Vector2 startTiling;
	private float tilingRandomMult = 0.5f;

	private float visibleTime = 0.4f;
	private float startAlpha = 1f;
	private float fadeRate;
	private Color fadeColor;

	public Texture startFlash;
	private Texture startTexture;
	private int flashFrames = 3;
	private int flashMax = 7;

	[Header("Attack Properties")]
	public float spawnRange = 1f;

	// Use this for initialization
	void Start () {

		_myRenderer = GetComponent<Renderer>();
		animateCountdown = _animateRate;

		startTiling = _myRenderer.material.GetTextureScale("_MainTex");
		startTexture = _myRenderer.material.GetTexture("_MainTex");
		_myRenderer.enabled = false;

		fadeRate = startAlpha/visibleTime;

		fadeColor = _myRenderer.material.color;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_myRenderer.enabled){
		

			flashFrames--;
			if (flashFrames < 0){

				if (_myRenderer.material.GetTexture("_MainTex") == startFlash){
					_myRenderer.material.SetTexture("_MainTex", startTexture);
				}

			fadeColor.a -= Time.deltaTime*fadeRate;
			if (fadeColor.a <= 0){
				fadeColor.a = 0;
				_myRenderer.enabled = false;
			}else{
				_myRenderer.material.color = fadeColor;
			}
				animateCountdown -= Time.deltaTime;
				if (animateCountdown <= 0){
					animateCountdown = _animateRate;
					Vector2 newTiling = startTiling;
					newTiling.x += Random.insideUnitCircle.x*tilingRandomMult;
					newTiling.y += Random.insideUnitCircle.y*tilingRandomMult;
					_myRenderer.material.SetTextureScale("_MainTex", newTiling);
				}
			}
		}
	
	}

	public void TriggerAttack(Vector3 attackDir){

		Vector3 spawnPos = attackDir.normalized;
		spawnPos*=spawnRange;
		spawnPos.z = transform.localPosition.z;
		transform.localPosition = spawnPos;

		fadeColor = _myRenderer.material.color;
		fadeColor.a = startAlpha;

		_myRenderer.material.color = Color.white;

		_myRenderer.material.SetTexture("_MainTex", startFlash);
		_myRenderer.enabled = true;
		flashFrames = flashMax;

		CameraShakeS.C.TimeSleep(0.1f);
		CameraShakeS.C.LargeShake();

	}
}
