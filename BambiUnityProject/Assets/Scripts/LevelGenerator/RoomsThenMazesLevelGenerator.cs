using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Direction
{
	North, East, South, West, None
}

public class RoomsThenMazesLevelGenerator : LevelGenerator
{

	/*
	 *	Random Notes 
	 *
	 *	First, we'll store the data as a 2D Array of Bools. False means Wall, True means floor
	 *  
	 * 	Second, rather than kep a handle on each door, that seems ridiculously unuseful!
	 * 		Instead, we'll store the rooms as a Tuple pair of Rectangles paired with a List<Vec2>s
	 * 		The List<Vec2> is a list of doors that enter that room.
	 * 
	 */



	public int RoomAttempts = 1000;
	public int WindingPercentage = 50;
	public int ConnectivityChance = 0;


	/// <summary>
	/// The level, represented as a 2 dimensional array of bools. False means Wall, True means Floor.
	/// </summary>
	private bool[,] levelBoolArray;

	/// <summary>
	/// The list of all of the RoomStructs. A RoomStruct is a Rectangle plus a list of Connectors.
	/// </summary>
	private List<RoomStruct> roomStructList;

	/// <summary>
	/// A flag for each spot in the level (floor or wall) as to what region it belongs to.
	/// </summary>
	private int[,] cellRegions;

	private int currentRegion;




	private int levelWidth;
	private int levelHeight;


	/// <summary>
	/// Initializes a new instance of the <see cref="RoomsThenMazesLevelGenerator"/> class.
	/// </summary>
	/// <param name="roomAttempts">Room attempts.</param>
	/// <param name="windingPercentage">Winding percentage. Needs to be between 1 and 100, or it will be set to 50.</param>
	public RoomsThenMazesLevelGenerator(int roomAttempts, int windingPercentage)
	{
		RoomAttempts = roomAttempts;
		if (windingPercentage > 0 && windingPercentage < 101)
			WindingPercentage = windingPercentage;
		else
			WindingPercentage = 50;
	}

	/// <summary>
	/// Generates a random level of the given dimensions. Heavily inspired by the algorithm put forth by Bob Nystrom.
	/// NOTE: Requires the dungeon to be Odd x Odd dimension size!!
	/// </summary>
	/// <param name="width">Width of the overall dungeon. MUST BE ODD.</param>
	/// <param name="height">Height of the overall dungeon. MUST BE ODD</param>
	public override Level GenerateRandomLevel(int width, int height)
	{
		levelWidth = width;
		levelHeight = height;
		if (levelWidth % 2 == 0 || levelHeight % 2 == 0)
		{
			Debug.Log("" +
				"");
			Debug.Log ("ERROR :: Level Generator :: Must be odd dimension sized. Adding 1 to the dimensions");
			++levelWidth;
			++levelHeight;
//			break;
		}

		// initialize our variables
		currentRegion = -1;
		cellRegions = new int[levelWidth, levelHeight];
		for (int x = 0; x < levelWidth; ++x)
			for (int y = 0; y < levelHeight; ++y)
				cellRegions [x, y] = -1;

		roomStructList = new List<RoomStruct> ();


		// set our level as entirely walls
		levelBoolArray = new bool[levelWidth,levelHeight];
		for( int x = 0; x < levelWidth; ++x)
			for( int y = 0; y < levelHeight; ++y)
				levelBoolArray[x,y] = false;


		// create the Rooms
		createRooms();


		// Fill with mazes
		mazeFill ();


		// connect each maze and room to the neighbor region
		connectRegions();


		// trim dead ends
		trimDeadEnds();


		// Optional: Fill in each room with special sauce
		foreach (RoomStruct r in roomStructList)
		{
			gamifyRoom (r);
		}

		Level l = new Level (levelBoolArray, roomStructList, levelWidth, levelHeight);
		return l;

	}

	private void gamifyRoom(RoomStruct r)
	{
		// meant to be overridden by base classes, in case they want to do something to each room!
	}


	#region Primary Creation Functions
	private void createRooms()
	{
		for(int i = 0; i < RoomAttempts; ++i)
		{
			// create a rectangle of variable size
			int x, y, width, height;

			width = (DungeonMaster.instance.BambiRandom.Next (1, 5) * 2) + 1;
			height = (DungeonMaster.instance.BambiRandom.Next (1, 5) * 2) + 1;

			x = DungeonMaster.instance.BambiRandom.Next (0, ((levelWidth - width) / 2)) * 2 + 1;
			y = DungeonMaster.instance.BambiRandom.Next (0, ((levelHeight - height) / 2)) * 2 + 1;

			Rect room = new Rect(x, y, width, height);

			// check the rectangle against all the others we have
			bool roomOverlaps = false;
			foreach( RoomStruct r in roomStructList)
			{
				if( DoRectsOverlap(r.room, room) )
				{
					roomOverlaps = true;
					break;
				}
			}

			// if they don't overlap, let's make this a room.
			if(!roomOverlaps)
			{
				// levelBoolArray,roomStructList, cellRegions -> all need to be updated; the first and last are covered by carveHole()


				// if the rectangle is clear, start a new region, and carve it out!

				// increment our region count, as we are adding a new one.
				++currentRegion;


				roomStructList.Add (new RoomStruct (room, new List<Vector2> ()));
				for(int iX = x; iX < x+width; ++iX)
					for(int iY = y; iY < y+height; ++iY)
						CarveAndAssignRegion (new Vector2 (iX, iY));
			}


		}

	}


	/// <summary>
	/// Fills the empty areas with mazes. Uses the Growing Tree Algorithm Found Here:
	/// http://www.astrolog.org/labyrnth/algrithm.htm
	/// </summary>
	private void mazeFill()
	{
		// Loop through every potential spot; notice we only go to odd spaces

		for (int y = 1; y < levelHeight; y += 2)
		{
			for (int x = 1; x < levelWidth; x += 2)
			{
				if (!levelBoolArray [x, y])
					growMaze (x, y);
			}
		}

	}

	private void growMaze(int startX, int startY)
	{
		List<Vector2> cells = new List<Vector2> ();
		Direction lastDirection = Direction.None;

		++currentRegion;
		CarveAndAssignRegion (new Vector2 (startX, startY));

		cells.Add (new Vector2 (startX, startY));

		while (cells.Count > 0)
		{
			Vector2 cell = cells [cells.Count - 1];

			// which directions have open cells
			List<Direction> availableDirections = new List<Direction>();

			foreach (Direction dir in Enum.GetValues(typeof(Direction)))
			{
				if(dir != Direction.None)
					if (CanCarveInDirection (cell, dir))
						availableDirections.Add (dir);
			}

			if (availableDirections.Count > 0)
			{
				Direction dir;
				if (availableDirections.Contains (lastDirection) && 
					DungeonMaster.instance.BambiRandom.Next (1, 100) > WindingPercentage)
				{
					dir = lastDirection;
				} else
				{
					dir = availableDirections[DungeonMaster.instance.BambiRandom.Next(0,availableDirections.Count-1)];
				}
				CarveAndAssignRegion (cell + D2V2 (dir));
				CarveAndAssignRegion (cell + D2V2 (dir) + D2V2 (dir));
				cells.Add(cell + D2V2 (dir) + D2V2 (dir));
				lastDirection = dir;

			} else
			{
				// there were no available directions to carve
				cells.RemoveAt(cells.Count-1);
				lastDirection = Direction.None;
			}
		}

	}

	private void connectRegions()
	{
		// Dictionary mapping points to the regions they conjoin

		Dictionary<Vector2, HashSet<int>> connectorsAndTheirRegions = new Dictionary<Vector2, HashSet<int>> ();

//		Debug.Log ("cellRegions size x,y : " + cellRegions.GetLength (0).ToString () + "," + cellRegions.GetLength (1).ToString ());
//		Debug.Log ("level Width, level Height : " + levelWidth.ToString() + "," + levelHeight.ToString());
		// iterate over each potential position in the level. This equates to 1 to width-1, and 1 to height-1
		// we are scanning for Potential Connectors (Wall Tiles that have a Floor Tile belonging to 2 different regions on opposite sides.
		for (int x = 1; x < levelWidth - 1; ++x)
		{
			for (int y = 1; y < levelHeight - 1; ++y)
			{
				// Potential Connectors must be Wall tiles! False flags are Wall tiles (true are floors)
				if (levelBoolArray [x, y] == false)
				{
					HashSet<int> regions = new HashSet<int> ();
//					Debug.Log ("=== x,y we're trying is === " + x.ToString () + "," + y.ToString ());
					// this is sadly still our best way to iterate over my custom Direction Enum.
					foreach (Direction dir in Enum.GetValues(typeof(Direction)))
					{
						// ignore the None direction...move along...
						if (dir != Direction.None)
						{
							// grab the region of this target cell. (using a temp v2 due to vector+direction math).
							Vector2 targetCell = new Vector2(x,y) + D2V2(dir);
//							Debug.Log ("target x,y : " + ((int)targetCell.x).ToString () + "," + ((int)targetCell.y).ToString ());
							int region = cellRegions [(int)targetCell.x, (int)targetCell.y];

							// finally, don't add the region if it's -1, that means it's regionless!
							if (region != -1)
							{
								regions.Add (region);
							}
						}
					}

					if (regions.Count > 1)
					{
						connectorsAndTheirRegions.Add (new Vector2 (x, y), regions);
					}

				}
			}
		}

		// now we just want the Vec2's that is the list of 
		HashSet<Vector2> eligibleConnectors = new HashSet<Vector2>();
		foreach (Vector2 v2 in connectorsAndTheirRegions.Keys)
		{
			eligibleConnectors.Add (v2);
		}

		List<int> merged = new List<int> ();
		HashSet<int> openRegions = new HashSet<int> ();

		for (int i = 0; i <= currentRegion; ++i)
		{
			merged.Add (i);
			openRegions.Add (i);
		}

		while (openRegions.Count > 1)
		{
			// get a random connector
			Vector2[] connArray = new Vector2[eligibleConnectors.Count];
			eligibleConnectors.CopyTo(connArray);
			Vector2 connector = connArray[DungeonMaster.instance.BambiRandom.Next (0, connArray.Length - 1)];

			// open it up.
//			Debug.Log("carving a connection");
			CarveWithNoRegion(connector);
			// TODO: Mark this as a potential Door. OR use the AddIntersection() stubbed method.


			// merge our newly connected regions! we'll pick one, and map all of its regions to the new region.

			List<int> regions = new List<int> ();
			foreach (int i in connectorsAndTheirRegions[connector])
			{
				regions.Add (merged[i]);
			}
//			var regions = connectorRegions[connector]
//				.map((region) => merged[region]);

			int destinationRegion = regions [0];
			regions.Remove (0);
			int[] sourcesAsArray = new int[regions.Count];
			regions.CopyTo (sourcesAsArray);
			regions.Insert (0, destinationRegion);

			List<int> sources = new List<int> ();
			foreach (int i in sourcesAsArray)
				sources.Add (i);

			// merge all of the affected regions now.
			for (int i = 0; i <= currentRegion; ++i)
			{
				if (sources.Contains (merged [i]))
					merged [i] = destinationRegion;
			}

			// sources aren't being used anymore.
			foreach (int i in sources)
				openRegions.Remove (i);

			// remove any connectors that aren't needed anymore

			eligibleConnectors.RemoveWhere (delegate (Vector2 pos)
			{
				// dont allow nearby connectors
				if( Vector2.Distance(pos,connector) < 2) return true;


				// take the regions we have at connectorRegions[pos].
				// create an iterable over them based on this function ( merged[region] )
				// and rid it of Duplicates (by adding to a HashSet)

				// if the connector isn't spannign different regions, then we don't need it.
				HashSet<int> remainingRegions = new HashSet<int> ();
				// merge our newly connected regions! we'll pick one, and map all of its regions to the new region.
				foreach (int i in connectorsAndTheirRegions[pos])
				{
					remainingRegions.Add (merged[i]);
				}
				if(remainingRegions.Count > 1)
					return false;

				// this connector isn't needed, so we'll remove it; but, we'll make a chance to extra connect
				int randomRollForConnection = DungeonMaster.instance.BambiRandom.Next(1,100);
//				Debug.Log("roll for connection: " + randomRollForConnection.ToString());
				if( randomRollForConnection < ConnectivityChance )
				{
					Debug.Log("EXTRA EXTRA :: dice roll carving");
					CarveWithNoRegion(pos);
				}
				return true;
			});

		}
	}

	private void trimDeadEnds()
	{
		bool doneTrimming = false;

		while (!doneTrimming)
		{
			doneTrimming = true;

			// let's scan the whole place again, but inside by 1
			for (int x = 1; x < levelWidth - 1; ++x)
			{
				for (int y = 1; y < levelHeight - 1; ++y)
				{
					// we're only interested in floors
					if (levelBoolArray [x, y])
					{
						// let's find out if it has more than 1 exit. (1 exit means 3 sides are walls, means dead end, trim!)
						int numExits = 0;

						// this is sadly still our best way to iterate over my custom Direction Enum.
						foreach (Direction dir in Enum.GetValues(typeof(Direction)))
						{
							// ignore the None direction...move along... >_>.
							// TODO: Find a better way to write my own iterator over my custom Direction enum.
							if (dir != Direction.None)
							{
								// grab the region of this target cell. (using a temp v2 due to vector+direction math).
								Vector2 targetCell = new Vector2(x,y) + D2V2(dir);
//								Debug.Log ("target x,y : " + ((int)targetCell.x).ToString () + "," + ((int)targetCell.y).ToString ());

								// if the target cell is an exit (True!), then increment our num exits
								if (levelBoolArray [(int)targetCell.x, (int)targetCell.y])
								{
									++numExits;
								}
							}
						}

						if (numExits == 1)
						{
							doneTrimming = false;
							FillHoleToWall(new Vector2(x,y));
						}
					}
				}
			}
		}
	}
	#endregion


	#region Helper Functions
	private void startRegion()
	{
		++currentRegion;
	}

	/// <summary>
	/// Determines whether or not we can carve in the target direction
	/// Checks for out of bounds and for 
	/// </summary>
	private bool CanCarveInDirection(Vector2 pos, Direction dir)
	{
		// we really only need to test against two aways, since everything is Odd-set
		Vector2 twoAway = pos + (D2V2(dir)*2);

		twoAway.x = (int)twoAway.x;
		twoAway.y = (int)twoAway.y;
		// is the position out of bounds?
		if (twoAway.x >= levelWidth
			|| twoAway.x < 0
			|| twoAway.y >= levelHeight
			|| twoAway.y < 0)
			return false;

//		Debug.Log ("twoAway x,y :: " + twoAway.x.ToString () + "," + twoAway.y.ToString ());
		// is the next two positions Walls?
		if (!levelBoolArray [(int)twoAway.x, (int)twoAway.y])
			return true;

		return false;
	}

	/// <summary>
	/// Joins the two regions by opening this section.
	/// TODO: Flag this tile as a "possible door" in case the game wants to do something with it.
	/// </summary>
	/// <param name="position">Position.</param>
	private void AddIntersection(Vector2 position)
	{

	}

	/// <summary>
	/// Converts a Direction Enum to a Vector 2 (Vector2.up for North, et cetera).
	/// If an improper Direction is supplied, will return 0,0
	/// </summary>
	/// <returns>The Vector2.</returns>
	/// <param name="dir">Input Direction (North/South/East/West).</param>
	private Vector2 D2V2(Direction dir)
	{
		switch (dir)
		{
			case Direction.North:
				return Vector2.up;
			case Direction.East:
				return Vector2.right;
			case Direction.South:
				return Vector2.down;
			case Direction.West:
				return Vector2.left;
			default:
				return Vector2.zero;
		}
	}


	/// <summary>
	/// Creates a passable gamespace at the target position, but does NOT assign it to the current region.
	/// Primarily used by the Connector step.
	/// </summary>
	/// <param name="position">Position of the hole created.</param>
	private void CarveWithNoRegion(Vector2 position)
	{
		levelBoolArray[(int)position.x, (int)position.y] =  true;
	}

	/// <summary>
	/// Creates a passable gamespace at the target position, and assigns it to the Current Region.
	/// Primarily used by the Rooms and Maze Fill steps.
	/// </summary>
	/// <param name="position">Position of the hole created.</param>
	private void CarveAndAssignRegion(Vector2 position)
	{
		levelBoolArray[(int)position.x, (int)position.y] =  true;
		cellRegions [(int)position.x, (int)position.y] = currentRegion;
	}

	/// <summary>
	/// Turns a passable gamespace into a Wall. Used by the Dead End Trimming Step.
	/// </summary>
	/// <param name="position">Position.</param>
	private void FillHoleToWall(Vector2 position)
	{
		levelBoolArray[(int)position.x, (int)position.y] =  false;
	}

	/// <summary>
	/// Checks to see if Two Rectangles overlap. Primarily used by the Fill Rooms function.
	/// </summary>
	/// <returns><c>true</c>, if rects do overlap <c>false</c> otherwise.</returns>
	/// <param name="rect1">Rect1.</param>
	/// <param name="rect2">Rect2.</param>
	private bool DoRectsOverlap(Rect rect1, Rect rect2)
	{
		// two checks; if one is on the left of the other, or if one is above the other

		// one to the left?
		if (rect1.xMin > rect2.xMax || rect2.xMin > rect1.xMax)
			return false;

		// one to the right?
		if (rect1.yMax < rect2.yMin || rect2.yMax < rect1.yMin)
			return false;
		
		return true;
	}

	#endregion
}
