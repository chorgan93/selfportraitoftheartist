using UnityEngine;
using System.Collections;

public class RandomScalingEffectS : MonoBehaviour {
	
	private Renderer _myRenderer;
	private float _animateRate = 0.083f;
	private float animateCountdown;
	private Vector2 startTiling;
	private Vector2 startOffset;
	private float tilingRandomMult = 0.5f;

	// Use this for initialization
	void Start () {

		_myRenderer = GetComponent<Renderer>();
		startTiling = _myRenderer.material.GetTextureScale("_MainTex");
		startOffset = _myRenderer.material.GetTextureOffset("_MainTex");
	
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

				Vector2 newOffset = startOffset;
				newOffset.x += Random.insideUnitCircle.x*tilingRandomMult;
				newOffset.y += Random.insideUnitCircle.y*tilingRandomMult;
				_myRenderer.material.SetTextureOffset("_MainTex", newOffset);
			
			}
				
		}

	
	}
}
