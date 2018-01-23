using UnityEngine;
using System.Collections;

public class ReflectObjEffectS : MonoBehaviour {

	public Texture whiteTex;
	private Texture startTex;
	private Renderer myRenderer;
	private Color startColor;

	public float whiteTime = 0.1f;
	public float blackTime = 0.1f;
	public float fadeRate = 1f;
	private Color fadeCol;
	private Vector3 startScale;
	private GrowEffectS myGrow;

	private bool _initialized = false;

	// Use this for initialization
	void Initialize (EnemyProjectileS newProj) {

		if (!_initialized){
			myRenderer = GetComponent<Renderer>();
			startScale = transform.localScale;
			transform.localScale = startScale*newProj.transform.localScale.x;
			myGrow = GetComponent<GrowEffectS>();
			myGrow.enabled = false;
			startTex = myRenderer.material.GetTexture("_MainTex");
			startColor = myRenderer.material.color;
		}
	
		transform.Rotate(newProj.transform.rotation.eulerAngles);
		StartCoroutine(EffectManager());

	}

	IEnumerator EffectManager(){
		myRenderer.material.SetTexture("_MainTex", whiteTex);
		myRenderer.material.color = Color.white;
		yield return new WaitForSeconds(whiteTime);

		myRenderer.material.color = Color.black;
		yield return new WaitForSeconds(blackTime);
		myRenderer.material.SetTexture("_MainTex", startTex);
		myRenderer.material.color = startColor;
		myGrow.enabled = true;

		fadeCol = startColor;
		while (fadeCol.a > 0){
			fadeCol.a -= fadeRate*Time.deltaTime;
			if (fadeCol.a > 0){
				myRenderer.material.color = fadeCol;
			}
			yield return null;
		}
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	public void Spawn (EnemyProjectileS reflectedProjectile) {
	
		transform.position = reflectedProjectile.transform.position;
		Initialize(reflectedProjectile);

	}
}
