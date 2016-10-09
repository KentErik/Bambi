using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BambiGameState
{
	Loading, Debug, Dungeon, Caretaker
}

public class GameManagerScript : MonoBehaviour 
{
	
	public static GameManagerScript instance = null;

	private BambiGameState currentGameState;

	//Awake is always called before any Start functions
	void Awake()
	{
		// Singleton setup
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);    

		currentGameState = BambiGameState.Loading;

		DontDestroyOnLoad(gameObject);

	}

	void Start()
	{
		currentGameState = BambiGameState.Debug;
	}

	public void GenerateDungeonLevel()
	{
		currentGameState = BambiGameState.Loading;

		// TODO: Ask our Dungeon Master to do some real logic;
		DungeonMaster.instance.GenerateDebugLevel ();


		currentGameState = BambiGameState.Dungeon;
	}


	void Update()
	{

	}


	/// <summary>
	/// Method for routing the input recieved by the Input Manager. Determines our game state and hands it along to the appropriate class.
	/// </summary>
	/// <param name="inp">Input recieved by the Input Manager.</param>
	public void RouteInput(BambiInput inp)
	{
		//print ("Game Manger :: Input Recieved -> " + inp.ToString ());

		if( currentGameState == BambiGameState.Dungeon )
			DungeonMaster.instance.RecieveInput(inp);
	}
}