using UnityEngine;
using System.Collections;

public class BGMHolderS : MonoBehaviour {

	public static BGMHolderS BG;

	// Use this for initialization
	void Awake(){
		if (BG != null){
			Destroy(gameObject);
		}else{
			BG = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public int GetCurrentTimeSample(){
		int currentSample = 0;
		if (transform.childCount > 0){
			
			for (int i = 0; i < transform.childCount; i++){
				if (transform.GetChild(i).gameObject.GetComponent<BGMLayerS>() != null && currentSample <= 0f){
					if (transform.GetChild(i).gameObject.GetComponent<BGMLayerS>().isPlayingAndHeard()){
						currentSample = transform.GetChild(i).gameObject.GetComponent<BGMLayerS>().sourceRef.timeSamples;
					}
				}
			}

		}
		return currentSample;
	}

	public bool ContainsChild(AudioClip checkClip){
		bool containsChild = false;

		if (transform.childCount > 0){
			for (int i = 0; i < transform.childCount; i++){
				if (transform.GetChild(i).gameObject.GetComponent<AudioSource>() != null && !containsChild){
					containsChild = (transform.GetChild(i).gameObject.GetComponent<AudioSource>().clip == checkClip);
				}
			}
		}

		return containsChild;
	}

	public BGMLayerS GetLayerWithClip(AudioClip checkClip){
		BGMLayerS containsChild = null;
		
		if (transform.childCount > 0){
			for (int i = 0; i < transform.childCount; i++){
				if (transform.GetChild(i).gameObject.GetComponent<AudioSource>() != null && !containsChild){
					if (transform.GetChild(i).gameObject.GetComponent<AudioSource>().clip == checkClip){
						containsChild = transform.GetChild(i).gameObject.GetComponent<BGMLayerS>();
					}
				}
			}
		}
		
		return containsChild;
	}

	public void EndAllExcept(BGMLayerS[] layers, bool instant, bool destroy){


		if (transform.childCount > 0){

			bool foundOnList = false;

			for (int i = 0; i < transform.childCount; i++){
				if (transform.GetChild(i).gameObject.GetComponent<BGMLayerS>() != null){
					foundOnList = false;
					for (int j = 0; j < layers.Length; j++){
						if (layers[j].sourceRef.clip == transform.GetChild(i).gameObject.GetComponent<BGMLayerS>().sourceRef.clip){
							foundOnList = true;
						}
					}
					if (!foundOnList){
						transform.GetChild(i).gameObject.GetComponent<BGMLayerS>().FadeOut(instant, destroy);
					}
				}
			}
		}
	}

	public void EndAllLayers(bool instant, bool destroy){
		if (transform.childCount > 0){
			// Debug.Log("Ending music!");
			for (int i = 0; i < transform.childCount; i++){
				if (transform.GetChild(i).gameObject.GetComponent<BGMLayerS>() != null){
					transform.GetChild(i).gameObject.GetComponent<BGMLayerS>().FadeOut(instant, destroy);
				}
			}
		}
	}
}
