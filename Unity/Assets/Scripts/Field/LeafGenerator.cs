using UnityEngine;
using Vexe.Runtime.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class LeafGenerator : BetterBehaviour {
	public Vector2 num_leaves = new Vector2(5,20);
	public Vector2 distance = new Vector2(0.1f,0.25f);
	public float crown_height = 0.25f;
	
	public Vector3 twist_min = new Vector3(0.0f,0.0f,0.0f);
	public Vector3 twist_max = new Vector3(0.0f,0.0f,0.0f);
	
	List<GameObject> leaves;
	// Use this for initialization
	void Start () {
		leaves = new List<GameObject>();
		for (int l = RandomUtils.random_int(num_leaves); l >= 0; l--){
			generate_leaf();
		}
	}

	GameObject generate_leaf(){
		GameObject leaf = GameObject.Instantiate(Resources.Load ("stem")) as GameObject;
		leaves.Add(leaf);
		leaf.transform.SetParent(transform);
		float r = RandomUtils.random_float(0.0f,360.0f);
		leaf.transform.localPosition = CylinderToCube(
			r * Mathf.Deg2Rad,
			RandomUtils.random_float(distance),
			crown_height
		);
		leaf.transform.localRotation = Quaternion.Euler(
			RandomUtils.random_vec3(twist_min,twist_max)+
			new Vector3(0.0f, r, 0.0f)
		);
		return leaf;
	}

	// Update is called once per frame
	public void Remove () {
		foreach(GameObject leaf in leaves){
			Destroy(leaf);
		}
	}
	
	public static Vector3 CylinderToCube(float r, float d, float h){
		return new Vector3(
			Mathf.Cos(r) * d,
			h,
			Mathf.Sin(r) * d
		);
	}
}
