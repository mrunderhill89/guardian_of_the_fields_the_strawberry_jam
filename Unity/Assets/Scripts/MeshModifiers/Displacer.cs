using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Vexe.Runtime.Types;

public abstract class Displacer : BetterBehaviour{
	public enum SourceVector{
		Local,World,Displaced
	}
	public Vector3 select_source_vector(SourceVector choice, Vector3 local, Vector3 world, Vector3 displaced){
		if (choice == SourceVector.World)
			return world;
		if (choice == SourceVector.Displaced)
			return displaced;
		return local;
	}
	public abstract Vector3 displace(Vector3 local, Vector3 world, Vector3 displaced);
}