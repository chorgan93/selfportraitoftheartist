using UnityEngine;
using System.Collections;

public class AutoSetProgressionS : MonoBehaviour {

	public int removeProgressNum = -1;

	// Use this for initialization
	void Start () {
	
		if (removeProgressNum >= 0){
			StoryProgressionS.RemoveProgress(removeProgressNum);
			StoryProgressionS.SaveProgress();
		}
	}

}
