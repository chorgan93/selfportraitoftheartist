using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VerseDisplayS : MonoBehaviour {

	public Image verseBorder;
	public Image verseIcon;
	public Text verseTitle;
	private string currentVerse;

	public float fadeRate = 0.5f;
	private Color currentCol;

	private bool fadingIn = false;
	private bool fadingOut = false;

	public static VerseDisplayS V;
	private bool _isShowing = true;

	void Awake(){

		if (V != null){
			Destroy(gameObject);
		}else{
			V = this;
		}

	}

	// Use this for initialization
	void Start () {

		currentCol = verseBorder.color;
		currentCol.a = 0f;
		verseBorder.color = verseIcon.color = verseTitle.color = currentCol;
		verseIcon.enabled = false;
		verseBorder.enabled = false;
		verseTitle.text = "";

		if (!PlayerController.equippedUpgrades.Contains(3)){
			_isShowing  = false;
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_isShowing){
		if (verseBorder.enabled){
			if (fadingOut){
				currentCol = verseBorder.color;
				currentCol.a -= Time.deltaTime*fadeRate;
				if (currentCol.a <= 0){
					fadingOut = false;
					currentCol.a = 0;
					verseIcon.enabled = false;
					verseBorder.enabled = false;
					verseTitle.text = "";
				}
				verseBorder.color = verseIcon.color = verseTitle.color = currentCol;
			}
			if (fadingIn){
				currentCol = verseBorder.color;
				currentCol.a += Time.deltaTime*fadeRate;
				if (currentCol.a >= 1f){
					fadingIn = false;
					currentCol.a = 1f;
				}
				verseBorder.color = verseIcon.color = verseTitle.color = currentCol;
			}
		}
		}
	
	}

	public void EndVerse(){
		StartCoroutine(EndVerseCoroutine());
	}
	private IEnumerator EndVerseCoroutine(){

		currentVerse = "";
		yield return new WaitForSeconds(1.6f);
		fadingIn = false;
		fadingOut = true;
	}

	public void NewVerse(string verseString){
		verseTitle.text = currentVerse = verseString;
		fadingIn = true;
		fadingOut = false;
		currentCol = verseBorder.color;
		currentCol.a = 0f;
		verseBorder.color = verseIcon.color = verseTitle.color = currentCol;
		if (_isShowing){
			verseIcon.enabled = true;
			verseBorder.enabled = true;
		}else{
			verseTitle.text = "";
		}
	}

	public void Show(){
		_isShowing = true;
		if (currentVerse != ""){
			verseIcon.enabled = true;
			verseBorder.enabled = true;
			verseTitle.text = currentVerse;
		}
	}
	public void Hide(){
		_isShowing = false;
		if (currentVerse != ""){
			verseIcon.enabled = false;
			verseBorder.enabled = false;
			verseTitle.text = "";
		}
	}
}
