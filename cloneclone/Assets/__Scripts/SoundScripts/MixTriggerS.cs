using UnityEngine;
using System.Collections;

public class MixTriggerS : MonoBehaviour {

	public BGMLayerS targetLayer;

	public bool fadeIn = false;
	public bool fadeOut = false;
	public bool instant = false;

    public bool depthsLoopFix = false;

	public bool activateOnce = false;
	private bool activated = false;

	public bool activateOnStart = false;
	public bool dontDestroyOnFadeOut = false;
    public bool dontTurnOffCollider = false;

	void Start(){
		if (activateOnStart){
			if (fadeIn){
				if (BGMHolderS.BG.ContainsChild(targetLayer.sourceRef.clip)){
					BGMHolderS.BG.GetLayerWithClip(targetLayer.sourceRef.clip).FadeIn(instant, targetLayer.maxVolume);
				}else{
					targetLayer.transform.parent = BGMHolderS.BG.transform;
					if (targetLayer.matchTimeStamp && targetLayer.sourceRef.clip.samples >= BGMHolderS.BG.GetCurrentTimeSample()){

                            targetLayer.sourceRef.timeSamples = BGMHolderS.BG.GetCurrentTimeSample();

                    }else if (targetLayer.matchTimeStamp && targetLayer.sourceRef.clip.samples < BGMHolderS.BG.GetCurrentTimeSample() && depthsLoopFix){
                        if (depthsLoopFix)
                        {
                            targetLayer.sourceRef.timeSamples = BGMHolderS.BG.GetCurrentTimeSample() - targetLayer.sourceRef.clip.samples;
                        }
                    }
					targetLayer.FadeIn(instant, targetLayer.maxVolume);
					
				}
			}
			if (fadeOut){
				if (BGMHolderS.BG.ContainsChild(targetLayer.sourceRef.clip)){
					BGMHolderS.BG.GetLayerWithClip(targetLayer.sourceRef.clip).FadeOut(instant, !dontDestroyOnFadeOut);
				}
			}
            activated = true;
            if (GetComponent<Collider>() != null && !dontTurnOffCollider)
            {
                GetComponent<Collider>().enabled = false;
            }
		}
	}


	void OnTriggerEnter (Collider other) {
	
		if (other.gameObject.tag == "Player"){
			if (((activateOnce && !activated) || !activateOnce) && targetLayer != null){
				if (fadeIn){
					if (BGMHolderS.BG.ContainsChild(targetLayer.sourceRef.clip)){
						BGMHolderS.BG.GetLayerWithClip(targetLayer.sourceRef.clip).FadeIn(instant, targetLayer.maxVolume);
					}else{
						targetLayer.transform.parent = BGMHolderS.BG.transform;
						if (targetLayer.matchTimeStamp && targetLayer.sourceRef.clip.samples >= BGMHolderS.BG.GetCurrentTimeSample()){
							targetLayer.sourceRef.timeSamples = BGMHolderS.BG.GetCurrentTimeSample();
						}
						targetLayer.FadeIn(instant, targetLayer.maxVolume);
						
					}
				}
				if (fadeOut){
					if (BGMHolderS.BG.ContainsChild(targetLayer.sourceRef.clip)){
						BGMHolderS.BG.GetLayerWithClip(targetLayer.sourceRef.clip).FadeOut(instant, !dontDestroyOnFadeOut);
					}
				}
				activated = true;

			}
            if (!activateOnStart && activateOnce && GetComponent<Collider>() != null && !dontTurnOffCollider)
            {
                GetComponent<Collider>().enabled = false;
            }
		}

	}
}
