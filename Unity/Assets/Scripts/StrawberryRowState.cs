using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class StrawberryRowState : StateComponent {
	[DontSerialize]
	public static List<StrawberryRowState> rows;
	static StrawberryRowState(){
		rows = new List<StrawberryRowState>();
	}
	// Use this for initialization
	void Start (){
		Debug.Log("Adding "+this.ToString()+" to rows list.");
		rows.Add(this);
		parent = SingletonBehavior.get_instance<StrawberryStateMachine>().states["field"];
		recycle = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject,"recycle");
		recycle.from_state = this;
		recycle.test(optimize);
		on_entry (distribute);
	}

	public Vector3 min_position = new Vector3(0,0,0);
	public Vector3 max_position = new Vector3(0,0,0);
	public TransitionComponent recycle;
	public Func<AutomataComponent, bool> optimize = (AutomataComponent a)=>{
		return false; //Don't optimize yet.
	};

	void distribute(StateComponent state, AutomataComponent a){
		Transform t = a.gameObject.transform;
		t.position = getNextPosition ();
	}

	Vector3 getNextPosition(){
		return this.transform.position + this.generation_strategy(min_position,max_position);
	}

	Func<Vector3, Vector3, Vector3> generation_strategy = (Vector3 min, Vector3 max) => {
		return new Vector3 (
			RandomUtils.random_float (min.x, max.x),
			RandomUtils.random_float (min.y, max.y),
			RandomUtils.random_float (min.z, max.z)
			);
	};
}
