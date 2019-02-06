using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSteamAchievementS : MonoBehaviour {

    public SteamStatsAndAchievements.Achievement achievementToGive;
    private SteamStatsAndAchievements statReference;
    public DifficultyS.PunishState requiredPunishment = DifficultyS.PunishState.Easy;
    public DifficultyS.SinState requiredSin = DifficultyS.SinState.Easy;

    // Use this for initialization
    void Start () {
#if !DISABLESTEAMWORKS
        if (GameObject.Find("SteamManager"))
        {
            statReference = GameObject.Find("SteamManager").GetComponent<SteamStatsAndAchievements>();
            if (requiredPunishment >= DifficultyS.PunishState.Hard && requiredSin >= DifficultyS.SinState.Hard)
            {
                // this is a difficulty-locked achievement, treat it as such
                if (DifficultyS.selectedSinState >= requiredSin && DifficultyS.selectedPunishState >= requiredPunishment)
                {
                    statReference.UnlockAchievementExternal(achievementToGive);
                }
            }
            else
            {
                statReference.UnlockAchievementExternal(achievementToGive);
            }
        }
#endif
    }
	
}
