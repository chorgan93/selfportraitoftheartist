using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomSpawner : MonoBehaviour {

	private string worldString = "";

	private List<Vector2> roomCoordinates;
	private List<int> roomIds;

	public GameObject startRoom;
	public List<GameObject> bigListOfRooms;

	private Vector2 currentCoordinate;
	private List<Vector2> spawnedCoordinates;
	private List<Vector3> spawnedWorldPositions;
	private List<RoomS> spawnedRooms;

	private Vector3 currentWorldCoordinate;
	private RoomS currentRoomHandler;

	// Use this for initialization
	void Awake () {

		worldString = LevelGenerationS.currentWorldString;

		ParseWorldString();

		SpawnStartRoom();

	
	}

	private void ParseWorldString(){

		roomCoordinates = new List<Vector2>();
		roomIds = new List<int>();

		// add start room info
		roomCoordinates.Add(Vector2.zero);
		roomIds.Add(-1);

		string[] worldTiles = worldString.Split(LevelGenerationS.roomSeperator[0]);

		string[] roomSplit = new string[0];
		Vector2 roomPos = Vector2.zero;

		foreach (string room in worldTiles){
			if (room != ""){
				roomSplit = room.Split(LevelGenerationS.itemSeperator[0]);
				roomPos.x = int.Parse(roomSplit[0]);
				roomPos.y = int.Parse(roomSplit[1]);
	
				roomCoordinates.Add(roomPos);
				roomIds.Add(int.Parse(roomSplit[2]));
			}
		}

	}

	private void SpawnStartRoom(){

		spawnedCoordinates = new List<Vector2>();
		spawnedWorldPositions = new List<Vector3>();
		spawnedRooms = new List<RoomS>();

		currentCoordinate = Vector2.zero;
		spawnedCoordinates.Add(currentCoordinate);

		GameObject newRoom = Instantiate(startRoom, Vector3.zero, Quaternion.identity)
			as GameObject;

		currentWorldCoordinate = Vector3.zero;
		spawnedWorldPositions.Add(currentWorldCoordinate);

		currentRoomHandler = newRoom.GetComponent<RoomS>();

		bool hasUp = roomCoordinates.Contains(new Vector2(currentCoordinate.x, currentCoordinate.y+1));
		bool hasDown = roomCoordinates.Contains(new Vector2(currentCoordinate.x, currentCoordinate.y-1));
		bool hasRight = roomCoordinates.Contains(new Vector2(currentCoordinate.x+1, currentCoordinate.y));
		bool hasLeft = roomCoordinates.Contains(new Vector2(currentCoordinate.x-1, currentCoordinate.y));

		currentRoomHandler.SetSpawner(this, currentCoordinate, hasUp, hasDown, hasRight, hasLeft);
		spawnedRooms.Add (currentRoomHandler);

	}

	public void MoveRoomCheck(Vector2 current, int dir, Transform playerTransform){

		Vector2 targetCoord = current;
		switch (dir){
		case (0): // right
			targetCoord.x++;
			break;
		case (1): // down
			targetCoord.y--;
			break;
		case (2): // left
			targetCoord.x--;
			break;
		case (3): // up
			targetCoord.y++;
			break;
		}

		if (!spawnedCoordinates.Contains(targetCoord)){
			// spawn new room and move player to that room
			spawnedCoordinates.Add(targetCoord);

			GameObject newRoom = Instantiate(bigListOfRooms[roomIds[roomCoordinates.IndexOf(targetCoord)]],
			                                 Vector3.zero, Quaternion.identity)as GameObject;
			spawnedWorldPositions.Add(Vector3.zero);

			RoomS newRoomHandler = newRoom.GetComponent<RoomS>();
			spawnedRooms.Add (newRoomHandler);

			Vector3 newRoomPos = currentWorldCoordinate;
			
			newRoomPos.x += (currentRoomHandler.spawnRadius+newRoomHandler.spawnRadius)*(targetCoord.x-currentCoordinate.x);
			newRoomPos.y += (currentRoomHandler.spawnRadius+newRoomHandler.spawnRadius)*(targetCoord.y-currentCoordinate.y);
			newRoomHandler.transform.position = newRoomPos;

			bool hasUp = roomCoordinates.Contains(new Vector2(targetCoord.x, targetCoord.y+1));
			bool hasDown = roomCoordinates.Contains(new Vector2(targetCoord.x, targetCoord.y-1));
			bool hasRight = roomCoordinates.Contains(new Vector2(targetCoord.x+1, targetCoord.y));
			bool hasLeft = roomCoordinates.Contains(new Vector2(targetCoord.x-1, targetCoord.y));

			newRoomHandler.SetSpawner(this, targetCoord, hasUp, hasDown, hasRight, hasLeft);

			newRoomHandler.SpawnPlayer(dir, playerTransform);

			currentCoordinate = targetCoord;
			currentWorldCoordinate = newRoomPos;

			currentRoomHandler.StartTurnOff();
			currentRoomHandler = newRoomHandler;
		}else{
			// move player to existing room
			currentRoomHandler.StartTurnOff();

			currentRoomHandler = spawnedRooms[spawnedCoordinates.IndexOf(targetCoord)];
			currentRoomHandler.SpawnPlayer(dir, playerTransform);

			currentWorldCoordinate = currentRoomHandler.transform.position;
			currentCoordinate = targetCoord;
		}

	}
}
