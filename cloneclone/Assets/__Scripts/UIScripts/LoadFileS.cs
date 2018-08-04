using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadFileS : MonoBehaviour {

    public TextMesh nameText;
    public GameObject turnOffOnNonexist;
    public int saveDataNum = 0;

    public TextMesh currentChapterName;
    public TextMesh lastSceneName;
    public TextMesh playTimeText;



	// Use this for initialization
	void Start () {

        nameText.text = "FILE 0" + (saveDataNum+1).ToString() + ": NO DATA";
        if (SaveLoadS.savedGames.Count  <= saveDataNum){
            // turn off file
            turnOffOnNonexist.SetActive(false);
        }else{
            GameDataS myData = SaveLoadS.savedGames[saveDataNum];
            // set up file
            if (myData.storyProgression != null)
            {
                if (myData.storyProgression.Contains(666))
                {
                    nameText.text = "FILE " + (saveDataNum + 1).ToString() + ": " + myData.playerInventory.playerName + " †";
                }
                else
                {
                    nameText.text = "FILE " + (saveDataNum + 1).ToString() + ": " + myData.playerInventory.playerName;
                }
            }
            else
            {
                nameText.text = "FILE " + (saveDataNum + 1).ToString() + ": " + myData.playerInventory.playerName;
            }

            currentChapterName.text = myData.playerInventory.lastChapterName + " (" + myData.currentDarkness.ToString("F2") + "%)";
            lastSceneName.text = "Last Checkpoint: " + myData.playerInventory.lastSavePointName;
            string timeString = "Total Playtime: ";
            if (myData.playerInventory.totalPlayTimeHours < 10){
                timeString += "0" + myData.playerInventory.totalPlayTimeHours + "H ";
            }else{
                timeString += myData.playerInventory.totalPlayTimeHours + "H ";
            }
            if (myData.playerInventory.totalPlayTimeMinutes < 10)
            {
                timeString += "0" + myData.playerInventory.totalPlayTimeMinutes + "M ";
            }
            else
            {
                timeString += myData.playerInventory.totalPlayTimeMinutes + "M ";
            }
            if (myData.playerInventory.totalPlayTimeSeconds < 10)
            {
                timeString += "0" + myData.playerInventory.totalPlayTimeSeconds + "S ";
            }
            else
            {
                timeString += myData.playerInventory.totalPlayTimeSeconds + "S ";
            }
            playTimeText.text = timeString;

        }
		
	}

}
