using UnityEngine;
using System.Collections;

public class SceneManagerS : MonoBehaviour {

	public static bool inInfiniteScene = false;

	public bool lockBuddy = false;
	public bool preMenuScene = false;
	public bool lockMenus = false;
	public bool allowFastTravel = true;
	public bool resetDoCountup = false;

	[Header("Infinite Scene Properties")]
	public bool isInfiniteScene = false;
	public bool preventEXPGain = false;
	public DifficultyS.SinState overrideSinState = DifficultyS.SinState.None;
	public DifficultyS.PunishState overridePunishState = DifficultyS.PunishState.None;

	void Awake(){
		InGameCinematicS.turnOffBuddies = lockBuddy;
		if (preMenuScene){
			if (InGameMenuManagerS.hasUsedMenu){
				InGameMenuManagerS.allowMenuUse = true;
			}
		}else{
			InGameMenuManagerS.allowMenuUse = !lockMenus;
		}
		InGameMenuManagerS.allowFastTravel = allowFastTravel;
		inInfiniteScene = isInfiniteScene;
		PlayerCurrencyDisplayS.CanGetXP = !preventEXPGain;

		if (resetDoCountup){
			PlayerStatsS.dontDoCountUp = false;
		}

		DifficultyS.sinStateOverride = overrideSinState;
		DifficultyS.punishStateOverride = overridePunishState;
	}
}
