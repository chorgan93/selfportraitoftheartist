using UnityEngine;
using System.Collections;

public class TransformTutorialHandler : MonoBehaviour {

	private PlayerController playerRef;
	public SpriteRenderer blackScreen;

	public ActivateOnExamineS activateItems;

	private bool playerTransformed = false;

	// Use this for initialization
	void Start () {
	
		playerRef = GameObject.Find("Player").GetComponent<PlayerController>();
		playerRef.SetTalking(true,false,true);

	}
	
	// Update is called once per frame
	void Update () {

		if (!playerTransformed){
			if (!playerRef.talking){
				playerRef.SetTalking(true, false, true);
			}
			if (playerRef.isTransformed){
				playerTransformed = true;
				playerRef.SetTalking(false);
				blackScreen.gameObject.SetActive(false);
				activateItems.TurnOn();
			}
		}else{
			gameObject.SetActive(false);
		}
	
	}
}
