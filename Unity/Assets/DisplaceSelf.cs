using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Vexe.Runtime.Types;

public class DisplaceSelf : Displacer{
	public Vector3 scale = new Vector3(1.0f,1.0f,1.0f);
	public override Vector3 displace(Vector3 local, Vector3 world){
		return new Vector3(
			scale.x * local.x,
			scale.y * local.y,
			scale.z * local.z
		);
	}
}