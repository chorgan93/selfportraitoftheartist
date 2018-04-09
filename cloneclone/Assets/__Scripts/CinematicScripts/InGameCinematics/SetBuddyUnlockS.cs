using UnityEngine;
using System.Collections;

public class SetBuddyUnlockS : MonoBehaviour {

	public bool setBuddyUnlock = false;
	public int setProgress = -1;

	// Use this for initialization
	void Start () {
		PlayerController.familiarUnlocked = setBuddyUnlock;
		if (setProgress > 0){
			StoryProgressionS.SetStory(setProgress);
		}
		if (setBuddyUnlock){
			PlayerController playerRef = GameObject.Find("Player").GetComponent<PlayerController>();
			playerRef.TurnOnBuddy();
		}
	}

}
