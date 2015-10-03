using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class StrawberryRowState : BetterBehaviour{
	[DontSerialize]
	public static List<StrawberryRowState> rows;
	public static float recycle_dist = 5.0f;
	static StrawberryRowState(){
		rows = new List<StrawberryRowState>();
	}
	public static State random_row(){
		if (rows.Count > 0){
			State state = rows[RandomUtils.random_int(0,rows.Count)].state;
			Debug.Log(state);
			return state;
		}
		return null;
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
			.parent(globalSM.fsm.state("field"))
			.on_entry (new StateEvent(distribute))
			.on_update (new StateEvent((Automata a)=>{
				if (optimize(a)) {
					Debug.Log("Optimizing...");
				}
			}));
	}

	public Func<Automata,bool> optimize = (Automata a)=>{
		if (a.transform.position.z < Camera.main.transform.position.z-recycle_dist){
			return true;
		}
		return false; //Don't optimize yet.
	};

	void distribute(Automata a){
		Transform t = a.gameObject.transform;
		t.position = getNextPosition ();
		StrawberryComponent sb = a.gameObject.GetComponent<StrawberryComponent> ();
		if (sb != null) {
			sb.hidden(false);
		}
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
