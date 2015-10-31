using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
public class GameStartData : BetterBehaviour {
	public static int rng_seed;

	void Awake(){
		rng_seed = Random.seed;
	}

	//Strawberry Settings
	public static int max_berries_in_field = 40;
	public static float min_ripeness = 0.0f;
	public static float max_ripeness = 2.0f;
	public static float min_size = 0.06f;
	public static float max_size = 0.06f;

	//Rows and Breaks
	public static int break_distance = 100;
	//Break data goes here
	
	//Time of Day & Game Length
	public static float game_length = 2400.0f; //In Seconds
	public static float start_hour = 6.0f;//In Hours
	public static float end_hour = 18.0f; //In Hours
	
	//Hazard Data goes here
}
