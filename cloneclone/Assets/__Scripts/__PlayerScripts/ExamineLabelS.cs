using UnityEngine;
using System.Collections;

public class ExamineLabelS : MonoBehaviour {

	private PlayerController myRef;
	private TextMesh myMesh;
	private string startString;

	// Use this for initialization
	void Start () {

		myRef = GetComponentInParent<PlayerController>();
		myMesh = GetComponent<TextMesh>();
		startString = myMesh.text;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (myRef.examining && !myRef.talking){
			if (myRef.overrideExamineString != ""){
				myMesh.text = myRef.overrideExamineString;
			}else{
				myMesh.text = startString;
			}
		}else{
			myMesh.text = "";
		}
	
	}
}
