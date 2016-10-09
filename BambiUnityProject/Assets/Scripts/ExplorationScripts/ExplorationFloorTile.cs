using UnityEngine;
using System.Collections;

public class ExplorationFloorTile : ExplorationMapEntity 
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
		return true;
	}

	public override void AcceptPlayerMoveCommand()
	{
		return;
	}
}
