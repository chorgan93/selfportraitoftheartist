using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryProgressionS : MonoBehaviour {

	public static List<int> storyProgress = new List<int>();
	private static List<int> savedProgress = new List<int>();


	public static void SetStory(int newProgress){
		//storyProgress = newProgress;
		if (!storyProgress.Contains(newProgress)){
			storyProgress.Add(newProgress);
		}
	}

	public static void SaveProgress(){
		savedProgress = storyProgress;
		SaveLoadS.OverriteCurrentSave();
	}

	public static void ResetToSavedProgress(){
		storyProgress = savedProgress;
	}

	public static void NewGame(){
		storyProgress = savedProgress = new List<int>();
		//SaveLoadS.Load();
		InGameMenuManagerS.allowMenuUse = false;

		InGameMenuManagerS.hasUsedMenu = false;
		if (PlayerInventoryS.I != null){
			PlayerInventoryS.I.NewGame();
		}
	}

	public static int ReturnHighestProgress(){
		int highestProgress = -1;
		for (int i = 0; i < storyProgress.Count; i++){
			if (storyProgress[i] > highestProgress){
				highestProgress = storyProgress[i];
			}
		}
		return highestProgress;
	}
}
