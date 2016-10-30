using UnityEngine;
using System.Collections;

public class ExplorationObjectFactory : MonoBehaviour 
{
	public static ExplorationObjectFactory instance; // Needed

	public ExplorationMapEntity ExplorationFloorTilePrefab;
	public ExplorationMapEntity ExplorationWallTilePrefab;
	public ExplorationMapEntity ExplorationPlayerAvatarPrefab;

	void Start()
	{
		instance = this;
	}


	public ExplorationMapEntity CreateFloorTile(int x, int y)
	{
		ExplorationMapEntity floorTile;
		floorTile = Instantiate (instance.ExplorationFloorTilePrefab, new Vector3 (x, y, 0f), Quaternion.identity) as ExplorationMapEntity;
		floorTile.x = x;
		floorTile.y = y;
		return floorTile;
	}


	public ExplorationMapEntity CreateWallTile(int x, int y)
	{
		ExplorationMapEntity wallTile;
		wallTile = Instantiate (instance.ExplorationWallTilePrefab, new Vector3 (x, y, 0f), Quaternion.identity) as ExplorationMapEntity;
		wallTile.x = x;
		wallTile.y = y;
		return wallTile;
	}

	public ExplorationMapEntity CreatePlayer(int x, int y)
	{
		ExplorationMapEntity player;
		player = Instantiate (instance.ExplorationPlayerAvatarPrefab, new Vector3 (x, y, 0f), Quaternion.identity) as ExplorationMapEntity;
		player.x = x;
		player.y = y;
		return player;
	}

//
//	public static ExplorationMapEntity CreateSith(string name, int health, string weapon, bool forceUser)
//	{
//		var enemy = Object.Instantiate(instance.sithPrefab, Vector3.zero, Quaternion.identity).GetComponent<SpecialEnemy>();
//		enemy.Initialize(name, health, weapon, forceUser);
//		return enemy;
//	}


}
