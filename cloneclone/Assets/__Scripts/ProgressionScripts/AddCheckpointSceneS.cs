using UnityEngine;
using System.Collections;

public class AddCheckpointSceneS : MonoBehaviour {

	public int sceneIndex = -1;
	public int sceneLoadpoint = 0;

	// Use this for initialization
	void Start () {
		if (sceneIndex >= 0 && PlayerInventoryS.I != null){
			PlayerInventoryS.I.AddCheckpoint(sceneIndex, sceneLoadpoint);
            //StoryProgressionS.SaveProgress();
            CameraEffectsS.E.fadeRef.DoSave = true;
        }
	}
}
