using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomStruct
{
	public Rect room;
	public List<Vector2> connectors;

	public RoomStruct(Rect r, List<Vector2> c)
	{
		room = r;
		connectors = c;
	}

}
