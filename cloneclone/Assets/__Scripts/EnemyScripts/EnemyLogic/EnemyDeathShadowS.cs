using UnityEngine;
using System.Collections;

public class EnemyDeathShadowS : MonoBehaviour {

	private SpriteRenderer _myRenderer;
	private Color myColor;
	public float fadeRate;
	public float growRate;

	public float startFade = 0.8f;
	public float startSizeMult = 1.3f;
	public int delayGrow= 3;
	private int delayGrowCount;

	public void StartFade(Sprite endSprite, Vector3 startSize){
		_myRenderer = GetComponent<SpriteRenderer>();
		_myRenderer.sprite = endSprite;

		myColor = _myRenderer.color;
		myColor.a = startFade;
		_myRenderer.color = myColor;

		transform.localScale = startSizeMult*startSize;

		delayGrow = delayGrowCount;
	}

	// Update is called once per frame
	void Update () {

		delayGrowCount--;
		if (delayGrowCount <= 0){

			if (!_myRenderer){

				_myRenderer = GetComponent<SpriteRenderer>();

				myColor = _myRenderer.color;
				myColor.a = startFade;
				_myRenderer.color = myColor;
			}

		myColor = _myRenderer.color;
		myColor.a -= fadeRate*Time.deltaTime;
		if (myColor.a <= 0){
			Destroy(gameObject);
		}else{
			_myRenderer.color = myColor;
		}

		Vector3 growSize = transform.localScale;
		if (growSize.x < 0){
			growSize.x -= Time.deltaTime*growRate;
		}else{
			growSize.x += Time.deltaTime*growRate;
		}
		growSize.y += Time.deltaTime*growRate;
		transform.localScale = growSize;

		}
	
	}
}
