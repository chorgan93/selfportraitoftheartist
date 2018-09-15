using UnityEngine;
using System.Collections;

public class NatalieOverrideS : MonoBehaviour {

	private PlayerController playerToOverride;
	public PlayerWeaponS natalieMantra;
	public RuntimeAnimatorController natalieAnimatorController; 

	// Use this for initialization
	void Awake () {

		playerToOverride = GameObject.Find("Player").GetComponent<PlayerController>();
		playerToOverride.isNatalie = true;
        playerToOverride.disableTransformInScene = true;
		playerToOverride.equippedWeapons[0] = natalieMantra;
		playerToOverride.subWeapons[0] = natalieMantra;
        playerToOverride.GetComponent<PlayerAugmentsS>().RefreshAll();
        playerToOverride.myRenderer.GetComponent<Animator>().runtimeAnimatorController = natalieAnimatorController;
	
	}

}
