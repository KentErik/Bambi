using UnityEngine;
using System.Collections;

public class SandboxScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}


	void OnGUI () 
	{
		// Make a background box
		GUI.Box(new Rect(10,10,140,90), "Sandbox Menu");

		// Make the buttons
		if(GUI.Button(new Rect(20,40,100,20), "Make Level 1")) 
		{
			GameManagerScript.instance.GenerateDungeonLevel ();
		}

		// Make the second button.
		if(GUI.Button(new Rect(20,70,100,20), "Level 2")) 
		{
			print("Button 2! Does Nothing!");
		}
	}
}
