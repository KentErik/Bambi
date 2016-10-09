using UnityEngine;
using System.Collections;

public class ExplorationPlayerAvatar : ExplorationMapEntity {

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	/// <summary>
	/// Creates a new Player Avatar at the given X, Y coordinate. Will also instantiate its own game object
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public ExplorationPlayerAvatar (int x, int y)
	{
		this.x = x;
		this.y = y;
		GameObject.Instantiate (this.gameObject, new Vector3(x, y, 0), Quaternion.identity);
	}

	public override bool CanMoveOnto()
	{
		return false;
	}

	public override void AcceptPlayerMoveCommand()
	{
		return;
	}
}
