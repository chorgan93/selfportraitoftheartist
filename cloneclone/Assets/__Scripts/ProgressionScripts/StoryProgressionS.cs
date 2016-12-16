using UnityEngine;
using System.Collections;

public class StoryProgressionS : MonoBehaviour {

	public static int storyProgress = 0;
	private static int savedProgress = 0;

	public static void AdvanceStory(){
		storyProgress ++;
	}

	public static void SetStory(int newProgress){
		storyProgress = newProgress;
	}

	public static void SaveProgress(){
		savedProgress = storyProgress;
	}

	public static void ResetToSavedProgress(){
		storyProgress = savedProgress;
	}
}
