using UnityEngine;
using System.Collections;

public class BasicScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        print("Hello!");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyUp (KeyCode.Space))
		{
			GetComponent<Animator> ().SetTrigger ("JumpPressed");
		}
	}
}
