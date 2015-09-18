using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class StrawberryRowState : BetterBehaviour{
	[DontSerialize]
	public static List<StrawberryRowState> rows;
	static StrawberryRowState(){
		rows = new List<StrawberryRowState>();
	}
	public Vector3 min_position = new Vector3(0,0,0);
	public Vector3 max_position = new Vector3(0,0,0);
	public Transition recycle;
	public State state;
	// Use this for initialization
	void Start (){
		StrawberryStateMachine globalSM = SingletonBehavior.get_instance<StrawberryStateMachine>();
		rows.Add(this);
		state = NamedBehavior.GetOrCreateComponentByName<State>(gameObject, "state")
			.parent(globalSM.states["field"])
			.on_entry (new StateEvent(distribute));
		recycle = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject,"recycle")
			.from(state)
			.to(globalSM.states["init"])
			.add_test(new TransitionTest(optimize));
	}

	public Func<Automata,bool> optimize = (Automata a)=>{
		return false; //Don't optimize yet.
	};

	void distribute(Automata a){
		Transform t = a.gameObject.transform;
		t.position = getNextPosition ();
	}

	Vector3 getNextPosition(){
		Vector3 cameraZonly = new Vector3(
			0.0f,
			0.0f,
			Camera.main.transform.position.z
		);
		return this.transform.position 
		+cameraZonly
		+this.generation_strategy(min_position,max_position);
	}

	Func<Vector3, Vector3, Vector3> generation_strategy = (Vector3 min, Vector3 max) => {
		return new Vector3 (
			RandomUtils.random_float (min.x, max.x),
			RandomUtils.random_float (min.y, max.y),
			RandomUtils.random_float (min.z, max.z)
			);
	};
}
