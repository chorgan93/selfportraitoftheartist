using UnityEngine;
using System.Collections;

public class RoomS : MonoBehaviour {

	public bool hasCombat = true;

	private const float turnOffTimeMax = 1f;

	private RoomSpawner _spawnerReference;
	public RoomSpawner spawnerReference { get { return _spawnerReference; } }

	public float spawnRadius = 20f;

	private Vector2 _roomCoordinate;
	public Vector2 roomCoordinate { get { return _roomCoordinate; } }

	public RoomTeleporterS rightPortal;
	public RoomTeleporterS leftPortal;
	public RoomTeleporterS downPortal;
	public RoomTeleporterS upPortal;

	public Transform leftSpawn;
	public Transform upSpawn;
	public Transform rightSpawn;
	public Transform downSpawn;

	private bool _shouldTurnOff = false;
	private float _turnOffTime;

	private bool _initialized = false;


	private void Initialize(){

		if (!_initialized){
			_initialized = true;

			rightPortal.SetDirection (0, this);
			downPortal.SetDirection (1, this);
			leftPortal.SetDirection (2, this);
			upPortal.SetDirection (3, this);
		}

	}

	private void Update(){

		if (_shouldTurnOff){
			_turnOffTime -= Time.deltaTime;
			if (_turnOffTime <= 0){
				gameObject.SetActive(false);
			}
		}

	}

	public void SetSpawner(RoomSpawner mySpawner, Vector2 coordinate, bool hasUp,
	                       bool hasDown, bool hasRight, bool hasLeft){

		_spawnerReference = mySpawner;
		_roomCoordinate = coordinate;
		Initialize();

		leftPortal.gameObject.SetActive(hasLeft);
		rightPortal.gameObject.SetActive(hasRight);
		upPortal.gameObject.SetActive(hasUp);
		downPortal.gameObject.SetActive(hasDown);

	}

	public void SpawnPlayer(int fromDirection, Transform playerTransform){

		CancelTurnOff();

		switch (fromDirection){
		case(0): // from right
			playerTransform.position = leftSpawn.position;
			break;
		case(1): // from down
			playerTransform.position = upSpawn.position;
			break;
		case(2): // from left
			playerTransform.position = rightSpawn.position;
			break;
		case(3): // from up
			playerTransform.position = downSpawn.position;
			break;
		}

		playerTransform.GetComponent<PlayerController>().SetCombat(hasCombat);

	}

	public void CancelTurnOff(){
		_shouldTurnOff = false;
		gameObject.SetActive(true);
	}

	public void StartTurnOff(){

		_shouldTurnOff = true;
		_turnOffTime = turnOffTimeMax;

	}
}
