using UnityEngine;
using System.Collections;

public class TurnOnOffAtProgressionS : MonoBehaviour {

	public int progressNum = -1;
	public GameObject[] onAtProgressObjects;
	public GameObject[] offAtProgressObjects;

	// Use this for initialization
	void Start () {

		if (progressNum > -1 && StoryProgressionS.storyProgress >= progressNum){
			for (int i = 0; i < onAtProgressObjects.Length; i++){
				onAtProgressObjects[i].gameObject.SetActive(true);
			}
			for (int i = 0; i < offAtProgressObjects.Length; i++){
				offAtProgressObjects[i].gameObject.SetActive(false);
			}
		}
	
	}
}
