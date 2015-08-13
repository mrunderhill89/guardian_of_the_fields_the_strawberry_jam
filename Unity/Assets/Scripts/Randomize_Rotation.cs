using UnityEngine;
using System.Collections;

public class Randomize_Rotation : MonoBehaviour {
	public Vector3 min = new Vector3(0,0,0);
	public Vector3 max = new Vector3(0,0,0);
	// Use this for initialization
	void Start () {
		this.transform.localRotation = Quaternion.Euler (new Vector3 (
			RandomUtils.random_float (min.x, max.x),
			RandomUtils.random_float (min.y, max.y),
			RandomUtils.random_float (min.z, max.z)
		));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
