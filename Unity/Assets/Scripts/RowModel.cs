using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RowModel : MonoBehaviour {
	public int num_berries = 100;
	public float row_length = 10.0f;
	public float row_width = 0.5f;
	public Vector3 min_size;
	public Vector3 max_size;
	public int random_int(int min, int max){
		return (int)(min + (Random.value * (max - min)));
	}
	public float random_float(float min, float max){
		return min + (Random.value * (max - min));
	}
	public double random_double(double min, double max){
		return min + (Random.value * (max - min));
	}
	// Use this for initialization
	void Start () {
		GameObject strawberry_prefab = Resources.Load ("Strawberry") as GameObject;
		//Generate a set of random strawberries.
		for (int b = 0; b < num_berries; b++){
			Instantiate(strawberry_prefab, new Vector3(
				this.transform.position.x + random_float(-row_width,row_width),
				0.0f,
				this.transform.position.z + random_float(-row_length,row_length)
			), Quaternion.identity);
		}
	}
}
