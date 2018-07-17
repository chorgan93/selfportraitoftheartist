using UnityEngine;
using System.Collections;

public class NormalizeTimeScaleS : MonoBehaviour {

    public bool forceGoodQuality = false;

	// Use this for initialization
	void Start () {
		Time.timeScale = 1f;
		PlayerSlowTimeS.witchTimeActive = false;
        if (forceGoodQuality){
            QualitySettings.SetQualityLevel(1);
        }
	}

}
