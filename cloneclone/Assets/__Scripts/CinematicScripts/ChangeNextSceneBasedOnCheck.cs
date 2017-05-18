using UnityEngine;
using System.Collections;

public class ChangeNextSceneBasedOnCheck : MonoBehaviour {

	private CinematicHandlerS myHandler;
	public int checkForScene;
	public GameObject addCheckpointObj;
	public string changeNextSceneString;

	// Use this for initialization
	void Start () {
	
		myHandler = GetComponent<CinematicHandlerS>();
		if (PlayerInventoryS.I != null){
			if (!PlayerInventoryS.I.HasReachedScene(checkForScene)){
				addCheckpointObj.gameObject.SetActive(true);
				myHandler.loadSceneString = changeNextSceneString;
			}
		}

	}

}
