using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

public class StateComponent : NamedBehavior {
	public StateComponent parent;
	public StateComponent child;

	public int get_level(int tail = 1){
		if (parent != null){
			return parent.get_level(tail+1);
		}
		return tail;
	}
	public HashSet<AutomataComponent> visitors;
	void Start(){
		visitors = new HashSet<AutomataComponent>();
	}
	
	public void enter_automata(AutomataComponent a){
		visitors.Add(a);
	}
	public void update_automata(AutomataComponent a){
		
	}
	public void exit_automata(AutomataComponent a){
		visitors.Remove(a);
	}
	public void handle_transition(TransitionComponent trans){
		foreach(AutomataComponent a in visitors.ToList<AutomataComponent>()){
			a.move_transition(trans);
		}
	}
	public StateComponent add_child(StateComponent child, bool set_initial = false){
		child.parent = this;
		if (set_initial) {
			this.child = child;
		}
		return this;
	}
	public StateComponent add_transition(string name, StateComponent to, Func<bool> test = null, bool autorun = true){
		TransitionComponent trans = NamedBehavior.GetOrCreateComponentByName<TransitionComponent>(gameObject, name);
		trans.name = name;
		trans.auto_run = autorun;
		trans.from_state = this;
		trans.to_state = to;
		if (test != null) {
			//To-do: update this to take multiple events.
			trans.tests.Add(test);
		}
		return this;
	}
	public StateComponent on_entry(Action act){return this;}
	public StateComponent on_update(Action act){return this;}
	public StateComponent on_exit(Action act){return this;}
}