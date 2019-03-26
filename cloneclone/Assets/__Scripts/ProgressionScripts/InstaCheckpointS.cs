using UnityEngine;
using System.Collections;

public class InstaCheckpointS : MonoBehaviour {

	public string instaSaveScene;
	public int instaSavePos;

	// Use this for initialization
	void Start () {
		GameOverS.reviveScene = instaSaveScene;
		GameOverS.revivePosition = instaSavePos;
        //StoryProgressionS.SaveProgress();
        CameraEffectsS.E.fadeRef.DoSave = true;
    }

}
