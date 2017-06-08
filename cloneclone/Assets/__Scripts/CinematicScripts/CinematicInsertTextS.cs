using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CinematicInsertTextS : MonoBehaviour {

	public Text myText;
	public Text subText;
	public string startString;
	public string insertString;
	public int insertPosition;
	public int[] jumpInserts;
	private string displayString;

	public float scrollRate;
	private float scrollCountdown;
	private int currentChar;

	public float readTime = 1.4f; // time after completion of scroll before destroying
	private bool _doneScrolling = false;


	[Header("Sound Prefab")]
	public GameObject sfxObj;
	public int soundRate = 1;
	private int soundCountdown;

	// Use this for initialization
	void Start () {

		myText.text = displayString = startString;
		if (subText){
			subText.text = myText.text;
		}
		scrollCountdown = scrollRate;
		currentChar = 0;


	
	}
	
	// Update is called once per frame
	void Update () {


		if (!_doneScrolling){
			
			scrollCountdown -= Time.deltaTime;
			if (scrollCountdown <= 0){
				scrollCountdown = scrollRate;
				displayString = displayString.Insert(insertPosition, insertString[currentChar].ToString());
				insertPosition++;
				CheckForJumps();
				currentChar++;
				myText.text = displayString;
				if (subText){
					subText.text = myText.text;
				}
				if (currentChar >= insertString.Length){
					_doneScrolling = true;
				}
				if (sfxObj){
					soundCountdown--;
					if (soundCountdown <= 0){
						Instantiate(sfxObj);
						soundCountdown = soundRate;
					}
				}
			}
			
		}else{
			readTime -= Time.deltaTime;
			if (readTime <= 0){
				myText.text = "";
				if (subText){
					subText.text = "";
				}
				Destroy(gameObject);
			}
		}
	
	}

	void CheckForJumps(){
		if (jumpInserts.Length > 0){
			for (int i = 0; i < jumpInserts.Length; i++){
				if (jumpInserts[i] == currentChar){
					insertPosition++;
				}
			}
		}
	}
}
