using Vexe.Runtime.Types;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
public class RandomUtils
{
	public static int random_int(int min, int max){
		return (int)(min + (Random.value * (max - min)));
	}
	public static int random_int(Vector2 vec){
		return random_int((int)vec.x, (int)vec.y);
	}
	public static float random_float(float min, float max){
		return min + (Random.value * (max - min));
	}
	public static float random_float(Vector2 vec){
		return random_float(vec.x,vec.y);
	}
	public static double random_double(double min, double max){
		return min + (Random.value * (max - min));
	}
	public static Vector3 random_vec3(Vector3 min, Vector3 max){
		return new Vector3(
			random_float(min.x, max.x),
			random_float(min.y, max.y),
			random_float(min.z, max.z)
		);
	}
	public static Vector3 random_vec3(float min, float max){
		//We'll assume the floats refer to all three coordinates.
		return new Vector3(
			random_float(min, max),
			random_float(min, max),
			random_float(min, max)
		);
	}
	public static Vector3 random_direction(){
		Vector3 vec = random_vec3 (0.1f, 1.0f);
		vec.Normalize ();
		return vec;
	}
}