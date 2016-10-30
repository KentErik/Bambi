using UnityEngine;
using System.Collections;

public enum DungeonState
{
	AwaitingInput, Executing, Debug
}

public class DungeonMaster : MonoBehaviour {

	public static DungeonMaster instance = null;

	private ExplorationMap currentDungeonMap;

	private DungeonState currentState;

	private ExplorationMapGenerator mapGenerator;

	private GameObject mainCamera;

	public System.Random BambiRandom;

	//Awake is always called before any Start functions
	void Awake()
	{
		// Singleton setup
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);    

		DontDestroyOnLoad(gameObject);

		currentState = DungeonState.Debug;

		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");

		int bambiRandomSeed = System.DateTime.Now.Millisecond;
		bambiRandomSeed = 263;
		print (" &&& Random Seed &&&");
		print (bambiRandomSeed.ToString ());
		print (" &&& Random Seed &&&");
		BambiRandom = new System.Random (bambiRandomSeed);

//		explorationLevelGenerator = GetComponent<ExplorationLevelGeneratorScript> ();
		mapGenerator = new ExplorationMapGenerator();

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GenerateDebugLevel()
	{
		currentDungeonMap = mapGenerator.GenerateLevel(51, 51);


		mainCamera.transform.position =
			new Vector3(currentDungeonMap.PlayerEntity.x, currentDungeonMap.PlayerEntity.y, mainCamera.transform.position.z);
		currentState = DungeonState.AwaitingInput;
	}

	public void GenerateRoomsThenMazes()
	{

	}

	public void RecieveInput(BambiInput inp)
	{
//		print ("Dungeon Master :: Input Recieved -> " + inp.ToString ());
		if (currentState == DungeonState.AwaitingInput)
		{
			currentState = DungeonState.Executing;

			// *********** NOTE:: We MUST return the Dungeon State to Awaiting Input after executing the command! ***********
			switch (inp)
			{
				case BambiInput.Down:
				case BambiInput.Left:
				case BambiInput.Right:
				case BambiInput.Up:
					movePlayer (inp);
					break;
				default:
					break;
			}
		}
		// TODO: Upon switching the state back, we shoudl then re-show the buttons. All of the state switching shoudl be abstracted to methods.
	}

	private void movePlayer(BambiInput inp)
	{
		
		ExplorationMapEntity p = currentDungeonMap.PlayerEntity;
//		if(p.x < currentDungeonMap


		// first, we need to get the tile in the direction of the player, and make sure it's valid.
		int tarX = -8;
		int tarY = -8;
		switch (inp)
		{
			case BambiInput.Up:
				tarX = p.x;
				tarY = p.y + 1;
				break;
			case BambiInput.Down:
				tarX = p.x;
				tarY = p.y - 1;
				break;
			case BambiInput.Left:
				tarX = p.x - 1;
				tarY = p.y;
				break;
			case BambiInput.Right:
				tarX = p.x + 1;
				tarY = p.y;
				break;
			default:
				print ("ERROR: DungeonMaster.MovePlayer() input: " + inp.ToString ());
				break;
		}

		// make sure the target point is within our bounds
		if (tarX >= 0 && tarX < currentDungeonMap.Rows && tarY >= 0 && tarY < currentDungeonMap.Cols)
		{
			bool canWeMove = true;
			foreach (ExplorationMapEntity eme in currentDungeonMap.Entities[tarX][tarY])
			{
				eme.AcceptPlayerMoveCommand ();
				if (!eme.CanMoveOnto ())
					canWeMove = false;
			}
			if (canWeMove)
			{
				// MOVE THE PLAYER
				p.x = tarX;
				p.y = tarY;
				p.transform.position = new Vector3 (p.x, p.y, 0f);

				mainCamera.transform.position =
					new Vector3(p.x, p.y, mainCamera.transform.position.z);

//				print ("player moved");
			} else
			{
				//DONT MOVE THE PLAYER
				// TODO: Play a sound for bumping into something? or should the tile piece know that?
			}
		} else
		{
			// player tried to move out of bounds
			// do nothing for now

			// TODO: Play error sound for trying to move out of bounds; shoudl never happen with walls.
			print("ERROR: DungeonMaster.MovePlayer() tried to move out of bounds: " + tarX.ToString() + ", " + tarY.ToString());
		}

		currentState = DungeonState.AwaitingInput;
	}

}
