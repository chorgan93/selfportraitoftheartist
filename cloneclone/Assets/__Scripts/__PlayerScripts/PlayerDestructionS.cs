using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDestructionS : MonoBehaviour {

	// keep track of enemies defeated, blood, debris, etc
	private List<int> _enemiesDefeated;
	public List<int> enemiesDefeated { get { return _enemiesDefeated; } }

	// Use this for initialization
	void Awake () {
	
		_enemiesDefeated = new List<int>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddEnemyDefeated(int i){
		_enemiesDefeated.Add(i);
	}
	public void ClearAll(){
		_enemiesDefeated.Clear();
	}
}
