using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Level
{
	public int width;
	public int height;

	/// <summary>
	/// The level, represented as a 2 dimensional array of bools. False means Wall, True means Floor.
	/// </summary>
	public bool[,] levelFloorData;

	public List<RoomStruct> rooms;

	public Level(bool[,] floorData, List<RoomStruct> rooms, int width, int height)
	{
		levelFloorData = floorData;
		this.rooms = rooms;
		this.width = width;
		this.height = height;
	}

	public void AssignRooms(List<RoomStruct> rooms)
	{
		this.rooms = rooms;
	}

	public void AssignFloorData(bool[,] floorData)
	{
		levelFloorData = floorData;
	}

}
