using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KeypadKeys : MonoBehaviour {

	public Image selectOutline;
	public Text buttonText;
	private Color startCol;
	private Color selectedCol = Color.white;
	private bool isSelected = false;
	public GameObject pressSound;

	// Use this for initialization
	void Start () {
	
		selectOutline.enabled = false;
        startCol = new Color(0.75f, 0.75f, 0.75f, 1f);
        buttonText.color = startCol;

	}

	public void SetSelect(bool selected = false){
		isSelected = selected;	
		if (isSelected){
			buttonText.color = selectedCol;
			selectOutline.enabled = true;
        }else{
            startCol.a = 1f;
			buttonText.color = startCol;
			selectOutline.enabled = false;
            Debug.Log(startCol);
		}
	}

	public void Press(){
		StartCoroutine(PressButton());
	}

	IEnumerator PressButton(){
		if (pressSound){
			Instantiate(pressSound);
		}
		selectOutline.enabled = false;
		buttonText.color = startCol;
		yield return new WaitForSeconds(0.12f);
		if (isSelected){
			buttonText.color = selectedCol;
			selectOutline.enabled = true;
		}
	}
}
