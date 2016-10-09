using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ExplorationMap
{

	public string MapName;

	public List<ExplorationMapEntity>[][] Entities;

	public ExplorationMapEntity PlayerEntity;

	public int Rows;
	public int Cols;

	public ExplorationMap(string name, int rows, int cols)
	{
		this.MapName = name;
		this.Rows = rows;
		this.Cols = cols;

		Entities = new List<ExplorationMapEntity>[rows][];

		for (int x = 0; x < rows; ++x)
		{
			Entities [x] = new List<ExplorationMapEntity>[cols];
			for (int y = 0; y < cols; ++y)
			{
				Entities [x] [y] = new List<ExplorationMapEntity> ();
			}
		}


	}

	public void AddEntity(ExplorationMapEntity me)
	{
//		Debug.Log ("ME's x,y: + " + me.x.ToString () + "," + me.y.ToString());
		if (me != null)
			Entities [me.x] [me.y].Add (me);
		else
			Debug.Log ("MapEntity is null!");
	}

	public void RemoveEntity(ExplorationMapEntity me)
	{
		Entities [me.x] [me.y].Remove (me);
	}

	public void MoveEntity(ExplorationMapEntity me, int newX, int newY)
	{
		RemoveEntity (me);
		Entities [newX] [newY].Add (me);
		me.x = newX;
		me.y = newY;
	}
}
