using UnityEngine;
using System.Collections;

public enum BambiInput
{
	Up, Down, Left, Right, Select, Back, Debug
}

public class InputManager : MonoBehaviour {

	private bool inputReceived;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
		inputReceived = false;
		Touch t;

		bool moveLeft, moveRight, moveUp, moveDown;
		moveLeft = moveRight = moveUp = moveDown = false;


		foreach (Touch touch in Input.touches)
		{
			if( touch.phase == TouchPhase.Began )
			{
				t = touch;
				inputReceived = true;
				float tx = t.position.x;
				float ty = t.position.y;
				float tH = Screen.height;
				float tW = Screen.width;

				bool A1, A2;
				A1 = A2 = false;

				if (ty > ((tH / tW) * tx))
					A1 = true;
				if (ty > (tH - ((tH / tW) * tx)))
					A2 = true;

				if (A1)
				{
					if (A2)
						moveUp = true;
					else
						moveLeft = true;
				} else
				{
					if (A2)
						moveRight = true;
					else
						moveDown = true;
				}


				break;
			}
		}

		if (inputReceived == false)
		{
			if(Input.GetButtonUp("Up"))
			{
				moveUp = true;
				inputReceived = true;

			}
			if(Input.GetButtonUp("Down"))
			{
				moveDown = true;
				inputReceived = true;
			}
			if(Input.GetButtonUp("Left"))
			{
				moveLeft = true;
				inputReceived = true;
			}
			if(Input.GetButtonUp("Right"))
			{
				moveRight = true;
				inputReceived = true;
			}
		}

		if (inputReceived)
		{
			/* **** DEBUG OF INPUT RECIEVED ****
			string inp = "";
			if (moveUp)
				inp += "up ";
			if (moveDown)
				inp += "down ";
			if (moveLeft)
				inp += "left ";
			if (moveRight)
				inp += "right ";
			print("input received: " + inp);
			*/
			BambiInput inp = BambiInput.Debug ;
			if (moveUp)
				inp = BambiInput.Up;
			if (moveDown)
				inp = BambiInput.Down;
			if (moveLeft)
				inp = BambiInput.Left;
			if (moveRight)
				inp = BambiInput.Right;

			GameManagerScript.instance.RouteInput (inp);
		}
	}

}
