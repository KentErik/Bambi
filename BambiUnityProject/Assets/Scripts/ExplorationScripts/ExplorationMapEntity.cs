using UnityEngine;
using System.Collections;

public abstract class ExplorationMapEntity : MonoBehaviour
{
	public abstract bool CanMoveOnto ();

	public abstract void AcceptPlayerMoveCommand();

	[HideInInspector]
	public int x;

	[HideInInspector]
	public int y;

}
