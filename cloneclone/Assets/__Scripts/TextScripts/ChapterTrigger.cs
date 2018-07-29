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

    public static string lastChapterTriggered = "Track -1 ~ Cradle";
    public int chapterNum = 1;

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
		if (other.gameObject.tag == "Player" && !activated){
		
				//chapterRef.SetShowing(true, chapterString);

			isShowing = true;
			activated = true;
            if (chapterString.Contains("~")){
                lastChapterTriggered = chapterString;
            }
			if (addToProgression > -1){
				StoryProgressionS.SetStory(addToProgression);
			}

            // if new game plus, save darkness %
            if (PlayerAugmentsS.MARKED_AUG){
                if (PlayerInventoryS.I.revertDarknessNums.Count <= chapterNum){
                    PlayerInventoryS.I.revertDarknessNums.Add(PlayerStatsS._currentDarkness);
                }else{
                    if (!PlayerController.killedFamiliar && PlayerInventoryS.I.revertDarknessNums[chapterNum] > PlayerStatsS._currentDarkness){
                        PlayerInventoryS.I.revertDarknessNums[chapterNum] = PlayerStatsS._currentDarkness;
                    }
                }
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
