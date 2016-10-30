using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class LevelGenerator
{

	/// <summary>
	/// Abstract class for generating levels. Can be subclassed to do different verions.
	/// </summary>



	/// <summary>
	/// Abstract function to generate a level
	/// </summary>
	/// <returns>The random level.</returns>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	public abstract Level GenerateRandomLevel(int width, int height);



}
