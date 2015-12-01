using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Vexe.Runtime.Types;

public class DisplaceSelf : Displacer{
	public Vector3 scale = new Vector3(1.0f,1.0f,1.0f);
	public SourceVector source_vector = SourceVector.Local;
	
	public override Vector3 displace(Vector3 local, Vector3 world, Vector3 displaced){
		Vector3 source = select_source_vector(source_vector, local, world, displaced);
		return new Vector3(
			scale.x * source.x,
			scale.y * source.y,
			scale.z * source.z
		);
	}
}