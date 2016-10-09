using UnityEngine;
using System.Collections;

public class ExplorationWallTile : ExplorationMapEntity 
{
	public Sprite sprite;

	void Awake()
	{

	}

	// Use this for initialization
	void Start () 
	{

	}

	// Update is called once per frame
	void Update () 
	{

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