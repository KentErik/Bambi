using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ExplorationMapGenerator 
{
	
	// public variables

	public int columns = 8;
	public int rows = 16;

//	private Transform levelHolder;

	public ExplorationObjectFactory EOFactory;

	/// <summary>
	/// Initializes a new instance of the <see cref="ExplorationLevelGeneratorScript"/> class.
	/// </summary>
	public ExplorationMapGenerator()
	{
		EOFactory = ExplorationObjectFactory.instance;
	}

//	private List<ExplorationMapEntity> mapEntities;

	/// <summary>
	/// Performs the operation of generating a level.
	/// -Will use its variables of cols/rows to determine cols/rows
	/// -currently just instantiates the "FloorTile" prefab for every single tile
	/// </summary>
	public ExplorationMap GenerateLevel()
	{
		LevelGenerator generator = new RoomsThenMazesLevelGenerator(1000, 50);
		Level l = generator.GenerateRandomLevel (rows, columns);

		ExplorationMap map = new ExplorationMap ("map1DEBUG", rows, columns);


		//Instantiate Board and set boardHolder to its transform.
		//TODO: Figure out how to clean up the heirarchy of all the Prefabs being generated (setting their transform parent)
//		levelHolder = new GameObject ("Board").transform;
		EOFactory = ExplorationObjectFactory.instance;

		EOFactory.CreateFloorTile (1, 1);

		// loop to generate our floor tiles; for now, this is just everything
		for(int x = 0; x < columns; ++x)
		{
			for(int y = 0; y < rows; ++y)
			{

				if (l.levelFloorData [x, y])
					map.AddEntity (EOFactory.CreateFloorTile (x, y));
				else
					map.AddEntity (EOFactory.CreateWallTile (x, y));
				
			}
		}

		Rect playerRoom = l.rooms [0].room;
		int playerX = (int)playerRoom.center.x;
		int playerY = (int)playerRoom.center.y;

		// *** PLAYER ***
		ExplorationMapEntity player = EOFactory.CreatePlayer(playerX,playerY);
		map.AddEntity(player);
		map.PlayerEntity = player;

//		levelHolder.position = new Vector3 (-5.0f, -5.0f, 0f);
		return map;
	}

	/// <summary>
	/// Calls the GenerateLevel function, but first will set cols/rows as fed
	/// </summary>
	/// <param name="rows">Number of Rows to create.</param>
	/// <param name="cols">Number of Columns to create.</param>
	public ExplorationMap GenerateLevel(int rows, int cols)
	{
		this.rows = rows;
		this.columns = cols;
		return GenerateLevel ();
	}
}

//
//public class BoardManager : MonoBehaviour
//{
//	// Using Serializable allows us to embed a class with sub properties in the inspector.
//	[Serializable]
//	public class Count
//	{
//		public int minimum;             //Minimum value for our Count class.
//		public int maximum;             //Maximum value for our Count class.
//
//
//		//Assignment constructor.
//		public Count (int min, int max)
//		{
//			minimum = min;
//			maximum = max;
//		}
//	}
//
//
//	public int columns = 8;                                         //Number of columns in our game board.
//	public int rows = 8;                                            //Number of rows in our game board.
//	public Count wallCount = new Count (5, 9);                      //Lower and upper limit for our random number of walls per level.
//	public Count foodCount = new Count (1, 5);                      //Lower and upper limit for our random number of food items per level.
//	public GameObject exit;                                         //Prefab to spawn for exit.
//	public GameObject[] floorTiles;                                 //Array of floor prefabs.
//	public GameObject[] wallTiles;                                  //Array of wall prefabs.
//	public GameObject[] foodTiles;                                  //Array of food prefabs.
//	public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
//	public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.
//
//	private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
//	private List <Vector3> gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.
//
//
//	//Clears our list gridPositions and prepares it to generate a new board.
//	void InitialiseList ()
//	{
//		gridPositions.Clear ();
//
//		//Loop through x axis (columns).
//		for(int x = 1; x < columns-1; x++)
//		{
//			//Within each column, loop through y axis (rows).
//			for(int y = 1; y < rows-1; y++)
//			{
//				//At each index add a new Vector3 to our list with the x and y coordinates of that position.
//				gridPositions.Add (new Vector3(x, y, 0f));
//			}
//		}
//	}
//
//
//	//Sets up the outer walls and floor (background) of the game board.
//	void BoardSetup ()
//	{
//		//Instantiate Board and set boardHolder to its transform.
//		boardHolder = new GameObject ("Board").transform;
//
//		//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
//		for(int x = -1; x < columns + 1; x++)
//		{
//			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
//			for(int y = -1; y < rows + 1; y++)
//			{
//				//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
//				GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
//
//				//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
//				if(x == -1 || x == columns || y == -1 || y == rows)
//					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
//
//				//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
//				GameObject instance =
//					Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
//
//				//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
//				instance.transform.SetParent (boardHolder);
//			}
//		}
//	}
//
//
//	//RandomPosition returns a random position from our list gridPositions.
//	Vector3 RandomPosition ()
//	{
//		//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
//		int randomIndex = Random.Range (0, gridPositions.Count);
//
//		//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
//		Vector3 randomPosition = gridPositions[randomIndex];
//
//		//Remove the entry at randomIndex from the list so that it can't be re-used.
//		gridPositions.RemoveAt (randomIndex);
//
//		//Return the randomly selected Vector3 position.
//		return randomPosition;
//	}
//
//
//	//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
//	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
//	{
//		//Choose a random number of objects to instantiate within the minimum and maximum limits
//		int objectCount = Random.Range (minimum, maximum+1);
//
//		//Instantiate objects until the randomly chosen limit objectCount is reached
//		for(int i = 0; i < objectCount; i++)
//		{
//			//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
//			Vector3 randomPosition = RandomPosition();
//
//			//Choose a random tile from tileArray and assign it to tileChoice
//			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
//
//			//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
//			Instantiate(tileChoice, randomPosition, Quaternion.identity);
//		}
//	}
//
//
//	//SetupScene initializes our level and calls the previous functions to lay out the game board
//	public void SetupScene (int level)
//	{
//		//Creates the outer walls and floor.
//		BoardSetup ();
//
//		//Reset our list of gridpositions.
//		InitialiseList ();
//
//		//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
//		LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
//
//		//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
//		LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
//
//		//Determine number of enemies based on current level number, based on a logarithmic progression
//		int enemyCount = (int)Mathf.Log(level, 2f);
//
//		//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
//		LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
//
//		//Instantiate the exit tile in the upper right hand corner of our game board
//		Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
//	}
//}
//}