using UnityEngine;
using System.Collections;

public class ChargeAttackS : MonoBehaviour {

	private Renderer _myRenderer;
	private float _animateRate = 0.033f;
	private float animateCountdown;
	private Vector2 startTiling;
	private float tilingRandomMult = 0.5f;

	// Use this for initialization
	void Start () {

		_myRenderer = GetComponent<Renderer>();
		animateCountdown = _animateRate;
		startTiling = _myRenderer.material.GetTextureScale("_MainTex");
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_myRenderer.enabled){
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
