using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;

public class Randomize_Scale : BetterBehaviour {
	public Vector3 min = new Vector3(1,1,1);
	public Vector3 max = new Vector3(1,1,1);
	// Use this for initialization
	void Start () {
		run();
	}
	
	void run(){
		this.transform.localScale = new Vector3 (
			RandomUtils.random_float (min.x, max.x),
			RandomUtils.random_float (min.y, max.y),
			RandomUtils.random_float (min.z, max.z)
		);
	}
}
