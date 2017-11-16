using UnityEngine;
using System.Collections;

public class ProgressionTriggerS : MonoBehaviour {
	
	public int progressionSet = -1;
	private bool _activated = false;
	public bool activateOnStart = false;

	void Start(){
		if (StoryProgressionS.storyProgress.Contains(progressionSet)){
			_activated = true;
		}
		if (!_activated && activateOnStart){

			StoryProgressionS.SetStory(progressionSet);
			Debug.Log("Added story beat " + progressionSet);
		}
	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.tag == "Player" && !_activated && !activateOnStart){
			StoryProgressionS.SetStory(progressionSet);
		}

	}
}
