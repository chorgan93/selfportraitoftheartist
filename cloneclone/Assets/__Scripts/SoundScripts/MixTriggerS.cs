using UnityEngine;
using System.Collections;

public class MixTriggerS : MonoBehaviour {

	public BGMLayerS targetLayer;

	public bool fadeIn = false;
	public bool fadeOut = false;
	public bool instant = false;

	public bool activateOnce = false;
	private bool activated = false;

	public bool activateOnStart = false;

	void Start(){
		if (activateOnStart){
			if (fadeIn){
				if (BGMHolderS.BG.ContainsChild(targetLayer.sourceRef.clip)){
					BGMHolderS.BG.GetLayerWithClip(targetLayer.sourceRef.clip).FadeIn(instant);
				}else{
					targetLayer.transform.parent = BGMHolderS.BG.transform;
					if (targetLayer.matchTimeStamp){
						targetLayer.sourceRef.timeSamples = BGMHolderS.BG.GetCurrentTimeSample();
					}
					targetLayer.FadeIn(instant);
					
				}
			}
			if (fadeOut){
				if (BGMHolderS.BG.ContainsChild(targetLayer.sourceRef.clip)){
					BGMHolderS.BG.GetLayerWithClip(targetLayer.sourceRef.clip).FadeOut(instant);
				}
			}
			activated = true;
		}
	}


	void OnTriggerEnter (Collider other) {
	
		if (other.gameObject.tag == "Player"){
			if (((activateOnce && !activated) || !activateOnce) && targetLayer != null){
				if (fadeIn){
					if (BGMHolderS.BG.ContainsChild(targetLayer.sourceRef.clip)){
						BGMHolderS.BG.GetLayerWithClip(targetLayer.sourceRef.clip).FadeIn(instant);
					}else{
						targetLayer.transform.parent = BGMHolderS.BG.transform;
						if (targetLayer.matchTimeStamp){
							targetLayer.sourceRef.timeSamples = BGMHolderS.BG.GetCurrentTimeSample();
						}
							targetLayer.FadeIn(instant);
						
					}
				}
				if (fadeOut){
					if (BGMHolderS.BG.ContainsChild(targetLayer.sourceRef.clip)){
						BGMHolderS.BG.GetLayerWithClip(targetLayer.sourceRef.clip).FadeOut(instant);
					}
				}
				activated = true;
			}
		}

	}
}
