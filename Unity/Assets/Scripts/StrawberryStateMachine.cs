using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrawberryStateMachine : SingletonBehavior {
	public Dictionary<string,StateComponent> states;
	public Dictionary<string,TransitionComponent> transitions;
	public int field_strawberries = 100;
	new void Awake () {
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
			.on_entry(init_enter)
			.on_exit(init_exit);
		states["field"]
			.on_entry(distribute);
		transitions["field_drag"] = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject,"field_drag")
			.set_from_state(states["field"])
			.set_to_state(states["drag"])
			.set_auto_run(false)
			.generate_path();
		transitions["hold_drag"] = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject,"hold_drag")
			.set_from_state(states["hold"])
			.set_to_state(states["drag"])
			.set_auto_run(false)
			.generate_path();
		transitions["fall_drag"] = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject,"fall_drag")
			.set_from_state(states["fall"])
			.set_to_state(states["drag"])
			.set_auto_run(false)
			.generate_path();
		transitions["drag_fall"] = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject,"drag_fall")
			.set_from_state(states["drag"])
			.set_to_state(states["fall"])
			.set_auto_run(false)
			.generate_path();
		transitions["basket_drag"] = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject,"basket_drag")
			.set_from_state(states["basket"])
			.set_to_state(states["drag"])
			.set_auto_run(false)
			.generate_path();
		transitions["recycle"] = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject,"recycle")
			.set_to_state(states["init"])
			.set_from_state(states["field"])
			.set_auto_run(false)
			.generate_path();
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
			berry.GetComponent<AutomataComponent>().move_direct(states["init"]);
		}
	}
}
