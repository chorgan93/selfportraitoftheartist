using UnityEngine;
using System.Collections;

public class DifficultyS : MonoBehaviour {
	
	public enum SinState {Easy, Normal, Hard, Challenge, None};
	public static SinState selectedSinState = SinState.Normal;
	public static SinState sinStateOverride = SinState.None;
	
	public enum PunishState {Easy, Normal, Hard, Challenge, None};
	public static PunishState selectedPunishState = PunishState.Normal;
	public static PunishState punishStateOverride = PunishState.None;

	public const float sinMultEasy = 0.75f;
	public const float sinMultNormal = 0.9f;
	public const float sinMultHard = 1.1f;
	public const float sinMultChallenge = 1.25f;


	public const float punishMultEasy = 0.7f;
	public const float punishMultNormal = 1.1f;
	public const float punishMultHard = 2f;
	public const float punishMultChallenge = 1000f;

	public static float GetSinMult(){

		if (sinStateOverride != SinState.None){
			if (sinStateOverride == SinState.Easy){
				return sinMultEasy;
			}
			else if (sinStateOverride == SinState.Normal){
				return sinMultNormal;
			}
			else if (sinStateOverride == SinState.Hard){
				return sinMultHard;
			}
			else if (sinStateOverride == SinState.Challenge){
				return sinMultChallenge;
			}
			else{
				return sinMultNormal;
			}
		}else{
		if (selectedSinState == SinState.Easy){
			return sinMultEasy;
		}
		else if (selectedSinState == SinState.Normal){
			return sinMultNormal;
		}
		else if (selectedSinState == SinState.Hard){
			return sinMultHard;
		}
		else if (selectedSinState == SinState.Challenge){
			return sinMultChallenge;
		}
		else{
			return sinMultNormal;
		}
		}
	}

	public static int GetSinInt(){


		if (sinStateOverride != SinState.None){
			if (sinStateOverride == SinState.Easy){
				return 0;
			}
			else if (sinStateOverride == SinState.Normal){
				return 1;
			}
			else if (sinStateOverride == SinState.Hard){
				return 2;
			}
			else if (sinStateOverride == SinState.Challenge){
				return 3;
			}
			else{
				return 1;
			}
		}else{
		if (selectedSinState == SinState.Easy){
			return 0;
		}
		else if (selectedSinState == SinState.Normal){
			return 1;
		}
		else if (selectedSinState == SinState.Hard){
			return 2;
		}
		else if (selectedSinState == SinState.Challenge){
			return 3;
		}
		else{
			return 1;
		}
		}
	}

	public static float GetPunishMult(){

		if (punishStateOverride != PunishState.None){
			if (punishStateOverride == PunishState.Easy){
				return punishMultEasy;
			}
			else if (punishStateOverride == PunishState.Normal){
				return punishMultNormal;
			}
			else if (punishStateOverride == PunishState.Hard){
				return punishMultHard;
			}
			else if (punishStateOverride == PunishState.Challenge){
				return punishMultChallenge;
			}
			else{
				return punishMultNormal;
			}
		}
		else{
		if (selectedPunishState == PunishState.Easy){
			return punishMultEasy;
		}
		else if (selectedPunishState == PunishState.Normal){
			return punishMultNormal;
		}
		else if (selectedPunishState == PunishState.Hard){
			return punishMultHard;
		}
		else if (selectedPunishState == PunishState.Challenge){
			return punishMultChallenge;
		}
		else{
			return punishMultNormal;
		}
		}
	}

	public static int GetPunishInt(){
		if (punishStateOverride != PunishState.None){
			if (punishStateOverride == PunishState.Easy){
				return 0;
			}
			else if (punishStateOverride == PunishState.Normal){
				return 1;
			}
			else if (punishStateOverride == PunishState.Hard){
				return 2;
			}
			else if (punishStateOverride == PunishState.Challenge){
				return 3;
			}
			else{
				return 1;
			}
		}
		else{
		if (selectedPunishState == PunishState.Easy){
			return 0;
		}
		else if (selectedPunishState == PunishState.Normal){
			return 1;
		}
		else if (selectedPunishState == PunishState.Hard){
			return 2;
		}
		else if (selectedPunishState == PunishState.Challenge){
			return 3;
		}
		else{
			return 1;
		}
		}
	}

	public static void SetDifficultiesFromInt(int sinSelect, int punishSelect){
		if (sinSelect == 0){
			DifficultyS.selectedSinState = DifficultyS.SinState.Easy;
		}
		if (sinSelect == 1){
			DifficultyS.selectedSinState = DifficultyS.SinState.Normal;
		}
		if (sinSelect == 2){
			DifficultyS.selectedSinState = DifficultyS.SinState.Hard;
		}
		if (sinSelect == 3){
			DifficultyS.selectedSinState = DifficultyS.SinState.Challenge;
		}

		if (punishSelect == 0){
			DifficultyS.selectedPunishState = DifficultyS.PunishState.Easy;
		}
		if (punishSelect == 1){
			DifficultyS.selectedPunishState = DifficultyS.PunishState.Normal;
		}
		if (punishSelect == 2){
			DifficultyS.selectedPunishState = DifficultyS.PunishState.Hard;
		}
		if (punishSelect == 3){
			DifficultyS.selectedPunishState = DifficultyS.PunishState.Challenge;
		}
	}
	
}
