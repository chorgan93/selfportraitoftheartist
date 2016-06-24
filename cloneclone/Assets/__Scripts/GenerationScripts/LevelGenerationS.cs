using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerationS : MonoBehaviour {

	public const string itemSeperator = ",";
	public const string rowSeperator = "\n";
	public const string roomSeperator = ";";

	public List<TileGeneratorS> tileGenerators;
	private bool generatorsComplete;

	private List<Vector2> tilePositions;
	private List<int> tileIDs;

	public static string currentWorldString;

	public GameObject debugRoomPrefab;
	public string nextSceneName;

	// Use this for initialization
	void Start () {

		generatorsComplete = false;
		StartCoroutine(GenerateWorldMap());
	
	}

	private IEnumerator GenerateWorldMap(){

		tilePositions = new List<Vector2>();
		tileIDs = new List<int>();

		foreach (TileGeneratorS generator in tileGenerators){
			generator.StartGenerator();
		}
		
		int numComplete;

		while(!generatorsComplete){

			numComplete = 0;

			foreach (TileGeneratorS generator in tileGenerators){
				if (generator.CompletedGeneration()){
					numComplete++;
				}
			}

			if (numComplete >= tileGenerators.Count){
				generatorsComplete = true;
			}
			else{
				yield return null;
			}

		}

		OutputBoard();
		GoToGame();

	}

	private void CompileRoomDictionaryList(){
		
		foreach (TileGeneratorS generator in tileGenerators){
			for (int i = 0; i < generator.generatedCoordinates.Count; i++){
				tilePositions.Add(generator.generatedCoordinates[i]);
				tileIDs.Add(generator.generatedIds[i]);
			}
			
			// delete duplicates, keeping last found tile
			for (int j = 0; j < tilePositions.Count; j++){

				for (int k = 0; k < tilePositions.Count; k++){
					// TODO bandaid please fix
					if (k < tilePositions.Count && j < tilePositions.Count){
						if (tilePositions[k] == tilePositions[j] && k!=j){
							RemoveTile(k);
						}
					}
				}

			}
		}

		currentWorldString = "";
		int tileNum = 0;
		foreach(Vector2 tilePos in tilePositions){
			currentWorldString += tilePos.x + itemSeperator;
			currentWorldString += tilePos.y + itemSeperator;
			currentWorldString += tileIDs[tileNum] + roomSeperator;
			tileNum++;
		}

	}

	private void BoardVisualization(){

		GameObject tile;

		Vector3 spawnPos = Vector3.zero;

		int i = 0;
		foreach(Vector2 tilePos in tilePositions){

			spawnPos.x = tilePos.x;
			spawnPos.y = tilePos.y;

			tile = Instantiate(debugRoomPrefab, spawnPos, Quaternion.identity) as GameObject;

			tile.GetComponent<RoomDataS>().roomID = tileIDs[i];

			i++;
		}

	}

	private void RemoveTile(int tileNum){
		// Debug.Log("Removed Tile at " + tilePositions[tileNum].x + "," + tilePositions[tileNum].y);
		tilePositions.RemoveAt(tileNum);
		tileIDs.RemoveAt(tileNum);
	}

	private void OutputBoard(){

		CompileRoomDictionaryList();
		//BoardVisualization();
		// Debug.Log(currentWorldString);

	}

	private void GoToGame(){

		Application.LoadLevel(nextSceneName);

	}
}
