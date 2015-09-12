using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrawberryStateMachine : SingletonBehavior {
	public Dictionary<string,State> states;
	public Dictionary<string,Transition> transitions;
	public int field_strawberries = 100;
	new void Awake () {
		base.Awake();
		states = new Dictionary<string,State> ();
		transitions = new Dictionary<string,Transition> ();
		states ["root"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "root");
		states ["init"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "init");
		states ["field"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "field");
		states ["drag"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "drag");
		states ["hold"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "hold");
		states ["fall"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "fall");
		states ["basket"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "basket");
		states["root"]
			.add_child(states["init"], true)
			.add_child(states["field"])
			.add_child(states["drag"])
			.add_child(states["hold"])
			.add_child(states["fall"])
			.add_child(states["basket"]);
	}
	void Start(){
		states["init"]
			.on_entry(new StateEvent(init_enter))
			.on_exit(new StateEvent(init_exit));
		states["field"]
			.on_entry(new StateEvent(distribute));
		transitions["init_field"] = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject,"init_field")
			.from(states["init"])
			.to(states["field"])
			.auto_run(true)
			.generate_path();
		transitions["field_drag"] = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject,"field_drag")
			.from(states["field"])
			.to(states["drag"])
			.auto_run(false)
			.generate_path();
		transitions["hold_drag"] = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject,"hold_drag")
			.from(states["hold"])
			.to(states["drag"])
			.auto_run(false)
			.generate_path();
		transitions["fall_drag"] = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject,"fall_drag")
			.from(states["fall"])
			.to(states["drag"])
			.auto_run(false)
			.generate_path();
		transitions["drag_fall"] = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject,"drag_fall")
			.from(states["drag"])
			.to(states["fall"])
			.auto_run(false)
			.generate_path();
		transitions["basket_drag"] = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject,"basket_drag")
			.from(states["basket"])
			.to(states["drag"])
			.auto_run(false)
			.generate_path();
		transitions["recycle"] = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject,"recycle")
			.from(states["init"])
			.to(states["field"])
			.auto_run(false)
			.generate_path();
	}
	void init_enter(Automata automata,State state){
		//Turn off any renderable elements of the strawberry
	}
	void init_exit(Automata automata,State state){
		//Turn on any renderable elements of the strawberry
	}
	void distribute(Automata automata,State state){
		//Select a random field object and distribute to it.
		int num_rows = StrawberryRowState.rows.Count;
		if (num_rows > 0) {
			int random_row = RandomUtils.random_int(0, num_rows);
			StrawberryRowState row = StrawberryRowState.rows[random_row];
			if (row != null){
				row.parent = states["field"];
				automata.move_direct(row);
			} else {
				Debug.LogError("Row number "+random_row+" doesn't exist. Current row count is "+num_rows);
			}
		} else {
			//Move the strawberry back to the init state.
			Debug.Log("Rows not initialized. Moving back to init.");
			transitions["recycle"].trigger_single(automata);
		}
	}
	void Update () {
		GameObject berry;
		for (int u = states["init"].visitors.Count + states["field"].visitors.Count;
		     u < field_strawberries;
		     u++
		){
			berry = GameObject.Instantiate(Resources.Load ("Strawberry")) as GameObject;
			berry.GetComponent<Automata>().move_direct(states["root"]);
		}
	}
}
