using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Vexe.Runtime.Types;

public class DisplaceTerrain : Displacer{
	public Vector3 direction = new Vector3(0.0f,1.0f,0.0f);
	public Terrain terrain;
	public override Vector3 displace(Vector3 local, Vector3 world, Vector3 displaced){
		float height = terrain.SampleHeight(world) + terrain.transform.position.y;
		return direction * height;
	}
}