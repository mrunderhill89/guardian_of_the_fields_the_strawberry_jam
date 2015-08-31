using UnityEngine;
using System.Collections;

public class StrawberryScale : MonoBehaviour {
	public Vector3 min = new Vector3(1,1,1);
	public Vector3 max = new Vector3(1,1,1);
	public Vector3 row1 = new Vector4 (1, 0, 0);
	public Vector3 row2 = new Vector4 (0, 1, 0);
	public Vector3 row3 = new Vector4 (0, 0, 1);
	// Use this for initialization
	void Start () {
		Vector3 random = new Vector3 (
			RandomUtils.random_float (min.x, max.x),
			RandomUtils.random_float (min.y, max.y),
			RandomUtils.random_float (min.z, max.z)
		);
		gameObject.transform.localScale = new Vector3 (
			Vector3.Dot(random, row1),
			Vector3.Dot(random, row2),
			Vector3.Dot(random, row3)
        );
	}

	// Update is called once per frame
	void Update () {

	}
}
