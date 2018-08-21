using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChargeRiseEffectS : MonoBehaviour {

	private GameObject[] riserObjs;
	private float[] riserGrowRates = new float[12]{4,4,4,4,10,10,10,10,20,20,20,20};
	private float[] riserStartAlphas = new float[12]{0.5f,0.5f,0.5f,0.5f,0.3f,0.3f,0.3f,0.3f,0.12f,0.12f,0.12f,0.12f};
	public float growMult = 1f;
	private Vector3[] riserStartSizes;
	private Vector3[] riserStartPositions;
	private Vector3 currentRisePosition = Vector3.zero;
	private Vector3 currentRiseSize = Vector3.zero;
	private Renderer[] riserRenderers;
	private Color[] riserStartCols;
	private Color currentRiseCol;

	private float riseT = 0f;
	private float riseTime = 1f;
	private float riseTimeMax = 1f;

	public float fadeOutTime = 0.75f;
	private float fadeT = 0f;

	private bool effectActive = false;

	private bool _initialized = false;
	private bool tryTrigger = false;

	Transform startParent = null;
	Vector3 startPos = Vector3.zero;


	void Start(){
		Initialize();
	}


	// Update is called once per frame
	void Update () {


		if (effectActive){
			riseTime -= Time.deltaTime;
			//Debug.Log(riseTime);
			if (riseTime <= 0){
				effectActive = false;
				gameObject.SetActive(false);
			}
			else{
				
				riseT = riseTime/riseTimeMax;
				riseT = Mathf.Sin(riseT * Mathf.PI * 0.5f);
				if (riseTime < fadeOutTime){
					fadeT = riseTime/fadeOutTime;
					fadeT = Mathf.Sin(fadeT * Mathf.PI * 0.5f);
				}else{
					fadeT = -1f;
				}
				int i = 0;
				foreach (GameObject riserObj in riserObjs){
					currentRiseSize = riserObj.transform.localScale;
					currentRiseSize.z += riserGrowRates[i]*riseT*Time.deltaTime*growMult;
					riserObj.transform.localScale = currentRiseSize;

					currentRisePosition = riserObj.transform.localPosition;
					currentRisePosition.y += riserGrowRates[i]*riseT/4f*Time.deltaTime*growMult;
					riserObj.transform.localPosition = currentRisePosition;

					if (fadeT >= 0){
						currentRiseCol = riserStartCols[i];
						currentRiseCol.a = Mathf.Lerp(0f, riserStartCols[i].a, fadeT);
						riserRenderers[i].material.color = currentRiseCol;
					}
					i++;
				}
			}
		}
	
	}

	void Initialize(){

		if (!_initialized){
			riserObjs = new GameObject[transform.childCount];
			for (int i = 0; i < riserObjs.Length; i++){
				riserObjs[i] = transform.GetChild(i).gameObject;
			}

			riserStartSizes = new Vector3[riserObjs.Length];
			riserStartPositions = new Vector3[riserObjs.Length];
			riserStartCols = new Color[riserObjs.Length];
			riserRenderers = new Renderer[riserObjs.Length];

			startPos = transform.localPosition;
			int j = 0;
			foreach (GameObject riserObj in riserObjs){
				riserStartSizes[j] = riserObj.transform.localScale;
				riserStartPositions[j] = riserObj.transform.localPosition;
				riserRenderers[j] = riserObj.GetComponent<Renderer>();
				riserStartCols[j] = riserRenderers[j].material.color;
				riserStartCols[j].a = riserStartAlphas[j];
				j++;
			}
			tryTrigger = false;
			effectActive = false;
			startParent = transform.parent;
			startPos = transform.localPosition;
			gameObject.SetActive(false);
			_initialized = true;

		}


	}

	public void TriggerEffect(Vector3 offset){
		riseTime = riseTimeMax;
		effectActive = true;
		transform.parent = startParent;
		transform.localPosition = startPos;
		transform.position += offset;
			//savedPos = transform.position = startPos+position;
		int i = 0;
		foreach (GameObject riserObj in riserObjs){
			riserObj.transform.localPosition = riserStartPositions[i];
			riserObj.transform.localScale = riserStartSizes[i];
			riserRenderers[i].material.color = riserStartCols[i];
			i++;
		}
		transform.parent= null;
		gameObject.SetActive(true);

	}

	public void ChangeStartColor(Color newCol){
		if (!effectActive){
			for (int i = 0; i < riserStartCols.Length; i++){
				riserStartCols[i].r = newCol.r;
				riserStartCols[i].g = newCol.g;
				riserStartCols[i].b = newCol.b;
			}
		}
	}
}
