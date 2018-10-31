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

	public static void RemoveProgress(int targetProgress){
		if (storyProgress.Contains(targetProgress)){
			storyProgress.Remove(targetProgress);
		}
	}

	public static void SaveProgress(){
		savedProgress = storyProgress;
		SaveLoadS.OverwriteCurrentSave();
	}

	public static void ResetToSavedProgress(){
		storyProgress = savedProgress;
	}

	public static void NewGame(){
        if (PlayerInventoryS.I != null)
        {
            PlayerInventoryS.I.NewGame();
        }
		storyProgress = savedProgress = new List<int>();
		storyProgress.Clear();
		savedProgress.Clear();
		//SaveLoadS.Load();
		InGameMenuManagerS.allowMenuUse = false;
		InGameMenuManagerS.hasUsedMenu = false;
		
	}

    public static bool CheckForPastProgress(int progNum){
        bool hasReached = false;
        if (savedProgress != null){
            if (savedProgress.Contains(progNum)){
                hasReached = true;
            }
        }
        if (storyProgress != null){
            if (storyProgress.Contains(progNum)){
                hasReached = true;
            }
        }
        return hasReached;
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
