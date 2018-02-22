using UnityEngine;
using System.Collections;

public class FlashMat3DS : MonoBehaviour {

	public Texture flashTexture;
	public int flashFrames = 8;

	private Renderer myRenderer;
	private Texture myTexture;
	private Color myColor;
	private RandomScalingEffectS randomScale;

	// Use this for initialization
	void Start () {
	
		myRenderer = GetComponent<Renderer>();
		myColor = myRenderer.material.color;
		myTexture = myRenderer.material.GetTexture("_MainTex");

		randomScale = GetComponent<RandomScalingEffectS>();
		if (randomScale){
			randomScale.enabled = false;
		}
		myRenderer.material.SetTexture("_MainTex", flashTexture);
		myRenderer.material.color = Color.white;

	}
	
	// Update is called once per frame
	void Update () {
	
		flashFrames--;
		if (flashFrames <= 0){
			myRenderer.material.SetTexture("_MainTex", myTexture);
			myRenderer.material.color = myColor;
			if (randomScale){
				randomScale.enabled = true;
			}
			enabled = false;
		}

	}
}
