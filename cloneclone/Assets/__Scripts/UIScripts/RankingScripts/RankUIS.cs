using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class RankUIS : MonoBehaviour {

	public List<RankUIItemS> uiObjPool;
	public List<RankUIItemS> activeRankObjects;

	[Header("VisualProperties")]

	[Header("Placement Properties")]
	public Vector2 topAnchoredPosition;
	public float yItemSeparation;



	private bool _initialized = false;

	// Use this for initialization
	void Start () {
	
		Initialize();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Initialize(){
		if (!_initialized){
			_initialized = true;
			activeRankObjects = new List<RankUIItemS>();
		}
	}
}
