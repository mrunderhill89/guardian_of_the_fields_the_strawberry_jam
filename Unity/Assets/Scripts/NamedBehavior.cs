using Vexe.Runtime.Types;
using UnityEngine;

public abstract class NamedBehavior : BetterBehaviour {
	public string _instance_name = "HFSM";
	[Serialize]
	public string instance_name {
		get {return _instance_name;}
		set {_instance_name = value;}
	}
}