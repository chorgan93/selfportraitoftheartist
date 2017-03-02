using UnityEngine;
using System.Collections;

public class BuddySwitchEffectS : MonoBehaviour {

	[Header("Rotation Components")]
	public Renderer[] effectLayers;
	public Vector3[] rotateRates;
	private Vector3[] startRotations = new Vector3[3];
	private bool showing = false;
	private float rotationDir = 1f;

	[Header("Fade Components")]
	public float fadeTime = 0.6f;
	private float fadeCountdown;
	public float[] startAlphas;
	private Color fadeColor;

	[Header("Change Effect Components")]
	public float changeRate;
	private float changeRateCountdown;
	private Vector2 startOffset;
	private Vector2 startTiling;
	public float offsetChangeMult;
	public float tilingChangeMult;

	[Header ("Flash Components")]
	public Texture flashTexture;
	public Texture startTexture;
	public int flashFrames = 4;
	private int flashCountdown;
	private bool flashing = false;
	public bool standalone = false;

	private Transform followTransform;

	// Use this for initialization
	void Start () {
	
		startTiling = effectLayers[0].material.GetTextureScale("_MainTex");
		startOffset = effectLayers[0].material.GetTextureOffset("_MainTex");
		if (!startTexture){
			startTexture = effectLayers[0].material.GetTexture("_MainTex");
		}

		for (int i = 0; i < effectLayers.Length; i++){
			startRotations[i] = effectLayers[i].transform.localRotation.eulerAngles;
		}
		if (!standalone){
			fadeColor = effectLayers[0].material.color;
			TurnOffRenderers();
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (showing){

			transform.position = followTransform.position;

			if (flashing){

				// white flash duration
				flashCountdown--;

				if (flashCountdown <= 0){
					flashing = false;

					// prep for fade out
					for (int i = 0; i < effectLayers.Length; i++){
						fadeColor.a = startAlphas[i];
						effectLayers[i].material.color = fadeColor;
						effectLayers[i].material.SetTexture("_MainTex", startTexture);
					}

				}
			}else{


				// randomize texture effect
				changeRateCountdown -= Time.deltaTime;

				if (changeRateCountdown <= 0){

					changeRateCountdown = changeRate;

					Vector2 scaleChanging = Vector2.zero;
					for (int k = 0; k < effectLayers.Length; k++){

						scaleChanging = startOffset;
						scaleChanging.x += Random.insideUnitCircle.x*offsetChangeMult;
						scaleChanging.y += Random.insideUnitCircle.x*offsetChangeMult;

						effectLayers[k].material.SetTextureOffset("_MainTex", scaleChanging);

						scaleChanging = startTiling;
						scaleChanging.x += Random.insideUnitCircle.x*tilingChangeMult;
						scaleChanging.y += Random.insideUnitCircle.x*tilingChangeMult;

						effectLayers[k].material.SetTextureScale("_MainTex", scaleChanging);
					}
				}

				// rotate
				Vector3 newRotation = Vector3.zero;
				for (int j = 0; j < effectLayers.Length; j++){
					
					newRotation = effectLayers[j].transform.localRotation.eulerAngles;
					newRotation += rotateRates[j]*Time.deltaTime*rotationDir;
					effectLayers[j].transform.localRotation = Quaternion.Euler(newRotation);
				}

				// fade out
				fadeCountdown -= Time.deltaTime;
				if (fadeCountdown <= 0){
					TurnOffRenderers();
				}else{
					for (int j = 0; j < effectLayers.Length; j++){
						fadeColor.a = startAlphas[j]*(fadeCountdown/fadeTime);
						effectLayers[j].material.color = fadeColor;
					}
				}
			}
		}
	
	}

	private void TurnOffRenderers(){

		foreach (Renderer r in effectLayers){
			r.enabled = false;
			showing = false;
		}

	}

	public void ChangeEffect(Color newCol, Transform newFollow){

		if (!standalone){
			transform.parent = null;
		}

		Color resetCol = Color.white;
		fadeColor = newCol;

		for (int i = 0; i < effectLayers.Length; i++){
			//resetCol.a = startAlphas[i];
			effectLayers[i].material.color = resetCol;
			effectLayers[i].material.SetTexture("_MainTex", flashTexture);
			effectLayers[i].transform.localRotation = Quaternion.Euler(startRotations[i]);
			effectLayers[i].enabled = true;
		}

		changeRateCountdown = 0f;
		fadeCountdown = fadeTime;

		flashCountdown = flashFrames;
		flashing = true;
		showing = true;

		followTransform = newFollow;
		rotationDir *= -1f;

	}
}
