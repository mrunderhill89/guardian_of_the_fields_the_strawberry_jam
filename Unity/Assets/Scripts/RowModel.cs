using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RowModel : MonoBehaviour {
	public int num_berries = 100;
	public int num_stems = 100;
	public float row_length = 10.0f;
	public float row_width = 0.5f;
	public Vector3 stem_bend;
	public Vector3 min_berry_size = new Vector3(1.0f,1.0f,1.0f);
	public Vector3 max_berry_size = new Vector3(1.0f,1.0f,1.0f);
	public Vector3 min_stem_size = new Vector3(1.0f,1.0f,1.0f);
	public Vector3 max_stem_size = new Vector3(1.0f,1.0f,1.0f);
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
		GameObject stem_prefab = Resources.Load ("stem") as GameObject;
		//Generate a set of random strawberries.
		GameObject berry;
		for (int b = 0; b < num_berries; b++){
			berry = Instantiate(strawberry_prefab, new Vector3(
				this.transform.position.x + random_float(-row_width,row_width),
				0.0f,
				this.transform.position.z + random_float(-row_length,row_length)
			), Quaternion.identity) as GameObject;
			berry.GetComponent<Transform>().localScale = new Vector3(
				random_float(min_berry_size.x, max_berry_size.x),
				random_float(min_berry_size.y, max_berry_size.y),
				random_float(min_berry_size.z, max_berry_size.z)
			);
		}
		//Generate a set of random stems.
		//I'll probably come up with a more realistic way to build strawberry plants later.
		GameObject stem;
		for (int s = 0; s < num_stems; s++){
			stem = Instantiate (stem_prefab, new Vector3 (
				this.transform.position.x + random_float (-row_width, row_width),
				0.0f,
				this.transform.position.z + random_float (-row_length, row_length)
			), Quaternion.Euler (
				random_float (-stem_bend.x, stem_bend.x),
				random_float (-stem_bend.y, stem_bend.y),
				random_float (-stem_bend.z, stem_bend.z)
			)) as GameObject;
			stem.GetComponent<Transform>().localScale = new Vector3(
				random_float(min_stem_size.x, max_stem_size.x),
				random_float(min_stem_size.y, max_stem_size.y),
				random_float(min_stem_size.z, max_stem_size.z)
			);
		}
	}
}
