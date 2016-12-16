using UnityEngine;
using System.Collections;

public class DifficultyS : MonoBehaviour {
	
	public enum SinState {Easy, Normal, Hard, Challenge};
	public static SinState selectedSinState = SinState.Normal;
	
	public enum PunishState {Easy, Normal, Hard, Challenge};
	public static PunishState selectedPunishState = PunishState.Normal;
	
}
