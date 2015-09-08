﻿using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrawberryStateMachine : SingletonBehavior {
	public Dictionary<string,StateComponent> states;
	public Dictionary<string,TransitionComponent> transitions;
	public int field_strawberries = 100;
	void Awake () {
		base.Awake();
		states = new Dictionary<string,StateComponent> ();
		transitions = new Dictionary<string,TransitionComponent> ();
		states ["root"] = NamedBehavior.GetOrCreateComponentByName<StateComponent> (gameObject, "root");
		states ["init"] = NamedBehavior.GetOrCreateComponentByName<StateComponent> (gameObject, "init");
		states ["field"] = NamedBehavior.GetOrCreateComponentByName<StateComponent> (gameObject, "field");
		states ["drag"] = NamedBehavior.GetOrCreateComponentByName<StateComponent> (gameObject, "drag");
		states ["hold"] = NamedBehavior.GetOrCreateComponentByName<StateComponent> (gameObject, "hold");
		states ["fall"] = NamedBehavior.GetOrCreateComponentByName<StateComponent> (gameObject, "fall");
		states ["basket"] = NamedBehavior.GetOrCreateComponentByName<StateComponent> (gameObject, "basket");
	}
	void Start(){
		states["root"]
			.add_child(states["init"], true)
			.add_child(states["field"])
			.add_child(states["drag"])
			.add_child(states["hold"])
			.add_child(states["fall"])
			.add_child(states["basket"]);
		states["init"]
			.on_entry(init_enter)
			.on_exit(init_exit);
		states["field"]
			.on_entry(distribute);
		transitions["field_drag"] = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject,"field_drag");
		transitions["recycle"] = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject,"recycle");
		transitions["recycle"].from_state = states["field"];
		transitions["recycle"].to_state = states["init"];
		transitions["recycle"].auto_run = false;
	}
	void init_enter(StateComponent state, AutomataComponent automata){
		//Turn off any renderable elements of the strawberry
	}
	void init_exit(StateComponent state, AutomataComponent automata){
		//Turn on any renderable elements of the strawberry
	}
	void distribute(StateComponent state, AutomataComponent automata){
		//Select a random field object and distribute to it.
		int num_rows = StrawberryRowState.rows.Count;
		if (num_rows > 0) {
			int random_row = RandomUtils.random_int (0, num_rows);
			StrawberryRowState row = StrawberryRowState.rows [random_row];
			row.parent = states ["field"];
			automata.move_direct (row);
		} else {
			//Move the strawberry back to the init state.
			transitions["recycle"].trigger_single(automata);
		}
	}
	void Update () {
		for (int u = states["init"].visitors.Count + states["field"].visitors.Count;
		     u < field_strawberries;
		     u++
		){
			Debug.Log("New Strawberry Generated");
			GameObject.Instantiate(Resources.Load ("Strawberry"));
		}
	}
}
