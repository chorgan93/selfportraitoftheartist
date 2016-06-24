using UnityEngine;
using System.Collections;

public class RoomTeleporterS : MonoBehaviour {

	private RoomS myRoom;
	private int direction = 0;

	private bool _isActive = true;

	public RoomClearCheck enemyClear;

	void Start(){

		if (enemyClear == null){
			_isActive = true;
		}else{
			_isActive = false;
		}

	}

	void Update(){

		if (!_isActive){
			if (enemyClear.cleared){
				_isActive = true;
			}
		}

	}

	public void SetDirection(int dir, RoomS parent){
		myRoom = parent;
		direction = dir;
	}

	void OnTriggerEnter(Collider other){

		if (_isActive){

			if (other.gameObject.tag == "Player"){
				myRoom.spawnerReference.MoveRoomCheck(myRoom.roomCoordinate, direction, other.gameObject.transform);
			}

		}

	}
}
