using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
public class GameStartData : BetterBehaviour {
	public static int rng_seed = Random.seed;

	//Strawberry Settings
	public static int max_berries_in_field = 40;
	public static float min_ripeness = 0.0f;
	public static float max_ripeness = 2.0f;
	public static float min_size = 0.06f;
	public static float max_size = 0.06f;

	//Rows and Breaks
	public static int break_distance = 100;
	//Break data goes here
	
	//Time of Day Effects
	public float morning_length = 0.0f;
	public float noon_length = 2400.0f;
	public float evening_length = 0.0f;
	
	//Hazard Data goes here
}
