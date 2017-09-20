using UnityEngine;
using System.Collections;

public class ChapterTrigger : MonoBehaviour {

	public string chapterString;
	private InstructionTextS chapterRef;
	private bool isShowing = false;

	public float delayShowTime = 2f;
	public float showingTime = 4f;

	private bool activated = false;

	public int addToProgression = -1;

	void Start(){

		chapterRef = GameObject.Find("ChapterText").GetComponent<InstructionTextS>();
		chapterString = chapterString.Replace("NEWLINE", "\n");


	}

	void Update(){
		if (isShowing){
			if (delayShowTime > 0){
				delayShowTime -= Time.deltaTime;
			}else{
				if (chapterRef.isShowing){
			showingTime -= Time.deltaTime;
			if (showingTime <= 0){
				chapterRef.SetShowing(false);
				isShowing = false;
			}
				}else{
					chapterRef.SetShowing(true, chapterString);
				}
			}
		}
	}

	void OnDisable(){
		if (isShowing){
			chapterRef.SetShowing(false);
		}
	}

	void OnTriggerEnter(Collider other){
		Debug.Log(other.gameObject.name);
		if (other.gameObject.tag == "Player" && !activated){
		
				//chapterRef.SetShowing(true, chapterString);

			isShowing = true;
			activated = true;
			if (addToProgression > -1){
				StoryProgressionS.SetStory(addToProgression);
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player"){
			chapterRef.SetShowing(false);
			isShowing = false;
		}
	}
}
