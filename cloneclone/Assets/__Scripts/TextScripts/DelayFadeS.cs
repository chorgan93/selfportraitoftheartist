using UnityEngine;
using System.Collections;

public class DelayFadeS : MonoBehaviour {

	public FadeScreenUI fadeTarget;
	public float delayFade = 0f;
	public bool delayWake = false;
	public int stopAtProgression = -1;

	// Use this for initialization
	void Awake () {
		if (stopAtProgression <= -1 || (stopAtProgression > -1 && !StoryProgressionS.storyProgress.Contains(stopAtProgression))){
			fadeTarget.ChangeFadeTime(delayFade, delayWake);
		}
	}

}
