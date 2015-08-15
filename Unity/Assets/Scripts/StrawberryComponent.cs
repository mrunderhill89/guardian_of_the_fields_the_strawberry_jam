using UnityEngine;
using System.Collections;

public class StrawberryComponent : MonoBehaviour {
	public double quality = 1.00;
	public bool is_picked = false;
	public GameObject container = null;
	// Use this for initialization
	void Start () {
		quality = RandomUtils.random_double(0.5, 1.25);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
