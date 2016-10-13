using UnityEngine;
using System.Collections;

public class LevelUpMenu : MonoBehaviour {

	public GameObject levelMenuProper;
	private bool onLevelMenu = false;

	// Use this for initialization
	void Start () {
	
		levelMenuProper.gameObject.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TurnOn(){
		levelMenuProper.gameObject.SetActive(false);
	}

	private void TurnOff(){
		levelMenuProper.gameObject.SetActive(false);
	}
}
