using UnityEngine;
using System.Collections;

public class RoboBoyScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyUp (KeyCode.Space))
		{
			GetComponent<Animator> ().SetTrigger ("Jump");
		}
	}
}
