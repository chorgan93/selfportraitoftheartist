using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingEffectS : MonoBehaviour {

    public FadeSpriteObjectS[] airLines;
    public float lineActivateTime = 0.1f;
    private float lineActivateCountdown;

	
	// Update is called once per frame
	void Update () {
        lineActivateCountdown -= Time.deltaTime;
            if (lineActivateCountdown <= 0){
                lineActivateCountdown = lineActivateTime;
                int lineToActivate = Mathf.FloorToInt(Random.Range(0, airLines.Length));
                airLines[lineToActivate].gameObject.SetActive(true);
                airLines[lineToActivate].Reinitialize();
            }
	}
}
