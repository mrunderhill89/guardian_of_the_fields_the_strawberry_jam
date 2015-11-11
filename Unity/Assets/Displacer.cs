using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Vexe.Runtime.Types;

public abstract class Displacer : BetterBehaviour{
	public abstract Vector3 displace(Vector3 local, Vector3 world);
}