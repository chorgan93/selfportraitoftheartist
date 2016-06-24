using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGeneratorS : MonoBehaviour {

	public int numTilesToGenerate;
	private int _numTilesGenerated;
	public int numTilesGenerated { get { return _numTilesGenerated; } }

	public int direction; // 0 = right, 1 = down, 2 = left, 3 = up
	public float chanceToRotate = 0.5f;
	public float chanceToReverse = 0.25f;

	public List<Vector2> generatedCoordinates;
	public List<int> generatedIds;
	private Vector2 currentCoordinate;

	public float difficultyOffsetMultiplier = 2f;

	public List<int> roomIDsLv01;
	public List<int> roomIDsLv02;
	public List<int> roomIDsLv03;
	public List<int> roomIDsLv04;
	public List<int> roomIDsLv05;
	public List<int> roomIDsLv06;
	public List<int> roomIDsLv07;
	public List<int> roomIDsLv08;
	public List<int> roomIDsLv09;
	public List<int> roomIDsLv10;

	public void StartGenerator(){

		// Debug.Log("Started Placing!!");
		
		generatedCoordinates = new List<Vector2>();
		generatedIds = new List<int>();
		currentCoordinate = Vector2.zero;
		_numTilesGenerated = 0;
		StartCoroutine(TilePlacing());

	}

	private IEnumerator TilePlacing(){

		float chanceToRotateNum = 0;
		float rotationDirection = 0;
		float chanceToReverseNum = 0;

		int roomID = 0;


		while (_numTilesGenerated < numTilesToGenerate){

			// go out in direction, then chance to rotate/reverse

			switch(direction){
			case(0):
				currentCoordinate.x++;
				break;
			case(1):
				currentCoordinate.y--;
				break;
			case(2):
				currentCoordinate.x--;
				break;
			case(3):
				currentCoordinate.y++;
				break;
			}

			if (currentCoordinate == Vector2.zero){
				yield return null;
			}
			else{

				generatedCoordinates.Add(currentCoordinate);

				int difficultyNum = Mathf.CeilToInt((10f*_numTilesGenerated)/(numTilesToGenerate*1f)
				                                    +Random.insideUnitCircle.x*difficultyOffsetMultiplier);

				if (difficultyNum > 2+difficultyOffsetMultiplier && difficultyNum < 9-difficultyOffsetMultiplier){
					// add some difficulty variance in middle
					difficultyNum = Mathf.CeilToInt(difficultyNum + difficultyOffsetMultiplier*Random.insideUnitCircle.x);

				}

				if (difficultyNum < 1){
					difficultyNum = 1;
				}
				if (difficultyNum > 10){
					difficultyNum = 10;
				}


				chanceToReverseNum = Random.Range(0f,1f);
				if (chanceToReverseNum < chanceToReverse){
					direction += 2;
					if (direction >= 4){
						direction -= 4;
					}
				}

				chanceToRotateNum = Random.Range(0f,1f);
				if (chanceToRotateNum < chanceToRotate){
					rotationDirection = Random.Range(0f,1f);
					if (rotationDirection <= 0.5f){
						direction--;
						if (direction < 0){
							direction = 3;
						}
					}
					else{
						direction++;
						if (direction > 3){
							direction = 0;
						}
					}
				}

				switch(difficultyNum){

					default:
						roomID = roomIDsLv01[Mathf.RoundToInt(Random.Range(0,roomIDsLv01.Count))];
						break;
					case 2:
						roomID = roomIDsLv02[Mathf.RoundToInt(Random.Range(0,roomIDsLv02.Count))];
						break;
						
					case 3:
						roomID = roomIDsLv03[Mathf.RoundToInt(Random.Range(0,roomIDsLv03.Count))];
						break;
						
					case 4:
						roomID = roomIDsLv04[Mathf.RoundToInt(Random.Range(0,roomIDsLv04.Count))];
						break;
						
					case 5:
						roomID = roomIDsLv05[Mathf.RoundToInt(Random.Range(0,roomIDsLv05.Count))];
						break;
						
					case 6:
						roomID = roomIDsLv06[Mathf.RoundToInt(Random.Range(0,roomIDsLv06.Count))];
						break;
						
					case 7:
						roomID = roomIDsLv07[Mathf.RoundToInt(Random.Range(0,roomIDsLv07.Count))];
						break;
						
					case 8:
						roomID = roomIDsLv08[Mathf.RoundToInt(Random.Range(0,roomIDsLv08.Count))];
						break;
						
					case 9:
						roomID = roomIDsLv09[Mathf.RoundToInt(Random.Range(0,roomIDsLv09.Count))];
						break;
						
					case 10:
						roomID = roomIDsLv10[Mathf.RoundToInt(Random.Range(0,roomIDsLv10.Count))];
						break;

				}

				generatedIds.Add(roomID);

				_numTilesGenerated++;
				yield return null;
			}

		}

	}

	public bool CompletedGeneration(){

		return (_numTilesGenerated >= numTilesToGenerate);

	}
}
