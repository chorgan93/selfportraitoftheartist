using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapPieceS : MonoBehaviour {

	public int mySceneNum = -1;
	public Image highlightImage;
	private bool inThisScene = false;
	public GameObject[] additionalIcons;

	private Color highlightColor;
	private float highlightTime = 1.2f;
	private float highlightCount;
	private float highlightMult = 1f;
	private float highlightMaxAlpha = 0.4f;
	private float highlightMinAlpha = 0.1f;

	// Use this for initialization
	void Start () {

		if (PlayerInventoryS.I.HasBeenToScene(mySceneNum)){
			gameObject.SetActive(true);
			if (Application.loadedLevel == mySceneNum){
				inThisScene = true;
				highlightImage.enabled = true;
				highlightColor = highlightImage.color;
				highlightColor.a = highlightMinAlpha;
				highlightImage.color = highlightColor;
			}else{
				highlightImage.enabled = false;
			}
			TurnOnIcons();
		}else{
			gameObject.SetActive(false);
			TurnOffIcons();
		}
	
	}

	void Update(){
		if (inThisScene){
			highlightCount += Time.deltaTime*highlightMult;
			if (highlightCount > highlightTime){
				highlightCount = highlightTime;
				highlightMult *= -1f;
			}
			if (highlightCount < 0f){
				highlightCount = 0f;
				highlightMult *= -1f;
			}
			highlightColor = highlightImage.color;
			highlightColor.a = Mathf.Sin(highlightCount/highlightTime)*highlightMaxAlpha+highlightMinAlpha;
			highlightImage.color = highlightColor;
		}
	}


	void TurnOnIcons(){
		for (int i = 0; i < additionalIcons.Length; i++){
			additionalIcons[i].SetActive(true);
		}
	}

	void TurnOffIcons(){
		for (int i = 0; i < additionalIcons.Length; i++){
			additionalIcons[i].SetActive(false);
		}
	}
}
