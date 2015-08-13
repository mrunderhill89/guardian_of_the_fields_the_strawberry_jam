using UnityEngine;
using Random = UnityEngine.Random;
using System;
public class RandomUtils
{
	public static int random_int(int min, int max){
		return (int)(min + (Random.value * (max - min)));
	}
	public static float random_float(float min, float max){
		return min + (Random.value * (max - min));
	}
	public static double random_double(double min, double max){
		return min + (Random.value * (max - min));
	}
}