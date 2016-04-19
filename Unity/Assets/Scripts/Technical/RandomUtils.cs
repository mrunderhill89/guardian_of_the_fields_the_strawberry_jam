using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;
public class RandomUtils
{
	protected static Dictionary<string, UnityRandom> channels
		= new Dictionary<string, UnityRandom>();
	
	public static UnityRandom get_channel(string key){
		if (!channels.ContainsKey(key)){
			channels[key] = new UnityRandom();
		}
		return channels[key];
	}
	
	public static UnityRandom initialize_seed(string key){
		channels[key] = new UnityRandom();
		return channels[key];
	}
	
	public static UnityRandom initialize_seed(string key, int seed = 0){
		channels[key] = new UnityRandom(seed);
		return channels[key];
	}
	
	public static int random_int(int min, int max, string key = "default"){
		return (int)(min + (get_channel(key).Value() * (max - min)));
	}
	public static int random_int(Vector2 vec, string key = "default"){
		return random_int((int)vec.x, (int)vec.y, key);
	}
	
	public static float random_float(float min, float max, string key = "default"){
		return min + (get_channel(key).Value() * (max - min));
	}
	public static float random_float(Vector2 vec, string key = "default"){
		return random_float(vec.x,vec.y, key);
	}
	public static double random_double(double min, double max, string key = "default"){
		return min + (get_channel(key).Value() * (max - min));
	}
	
	public static Vector3 random_vec3(Vector3 min, Vector3 max, string key = "default"){
		return new Vector3(
			random_float(min.x, max.x, key),
			random_float(min.y, max.y, key),
			random_float(min.z, max.z, key)
		);
	}
	public static Vector3 random_vec3(float min, float max, string key = "default"){
		//We'll assume the floats refer to all three coordinates.
		return new Vector3(
			random_float(min, max, key),
			random_float(min, max, key),
			random_float(min, max, key)
		);
	}
	
	public static Vector3 random_direction(string key = "default"){
		Vector3 vec = random_vec3 (0.1f, 1.0f, key);
		vec.Normalize ();
		return vec;
	}

	public static int random_index(ICollection that, string key = "default"){
		return random_int (0, that.Count, key);
	}
}