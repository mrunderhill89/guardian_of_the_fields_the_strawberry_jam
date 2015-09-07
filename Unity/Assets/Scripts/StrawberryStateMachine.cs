using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrawberryStateMachine : MonoBehaviour {
	public Dictionary<string,StateComponent> states;
	public Dictionary<string,TransitionComponent> transitions;
	void Awake () {
		states = new Dictionary<string,StateComponent>();
		transitions = new Dictionary<string,TransitionComponent>();
		states["root"] = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject,"root");
		states["init"] = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject,"init");
		states["field"] = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject,"field");
		states["drag"] = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject,"drag");
		states["hold"] = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject,"hold");
		states["fall"] = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject,"fall");
		states["basket"] = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject,"basket");
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
			.on_entry(field_enter);
		transitions["field_drag"] = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject,"field_drag");
	}
	void init_enter(StateComponent state, AutomataComponent automata){
	}
	void init_exit(StateComponent state, AutomataComponent automata){
	}
	void field_enter(StateComponent state, AutomataComponent automata){
	}
	void Start () {
		
	}
}
