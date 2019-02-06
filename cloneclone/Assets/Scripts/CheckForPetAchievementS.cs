using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForPetAchievementS : MonoBehaviour {

    public BuddyNPCS[] allBuddies;
    private SteamStatsAndAchievements statReference;
    bool giveAchievement = false;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < allBuddies.Length; i++){
            allBuddies[i].SetAchievementRefs(this);
        }
	}

    public void CheckRequirements()
    {
        if (!giveAchievement)
        {
            if (!statReference)
            {
                if (GameObject.Find("SteamManager"))
                {
                    statReference = GameObject.Find("SteamManager").GetComponent<SteamStatsAndAchievements>();
                }
            }
            giveAchievement = true;
            for (int i = 0; i < allBuddies.Length; i++) {
                if (!allBuddies[i].hasBeenPet) { giveAchievement = false; }
            }
            if (giveAchievement && statReference != null){
#if !DISABLESTEAMWORKS
                statReference.UnlockAchievementExternal(SteamStatsAndAchievements.Achievement.ACH_GARDEN);
#endif
            }else{
                giveAchievement = false;
            }
        }
    }
}
