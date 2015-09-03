using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using StateEvent = System.Action<StateComponent, AutomataComponent>;

public class StateComponent : NamedBehavior {
	public StateComponent parent;
	public StateComponent child;
	[DontSerialize]
	public List<StateEvent> entry_actions;
	[DontSerialize]
	public List<StateEvent> update_actions;
	[DontSerialize]
	public List<StateEvent> exit_actions;

	public int get_level(int tail = 1){
		if (parent != null){
			return parent.get_level(tail+1);
		}
		return tail;
	}
	public HashSet<AutomataComponent> visitors;

	void Awake(){
		visitors = new HashSet<AutomataComponent>();
		entry_actions = new List<StateEvent> ();
		update_actions = new List<StateEvent> ();
		exit_actions = new List<StateEvent> ();
	}
	
	public void enter_automata(AutomataComponent a){
		StartCoroutine(MultiAction.run_list(this, entry_actions.Select<StateEvent,ActionWrapper>((StateEvent evn) => {
			return new ActionWrapper (() => {evn(this,a);});
		})));
		visitors.Add(a);
		Debug.Log (instance_name + " visitor count +:" + visitors.Count);
	}
	public void update_automata(AutomataComponent a){
		StartCoroutine(MultiAction.run_list (this, update_actions.Select ((StateEvent evn) => {
			return new ActionWrapper (() => {evn(this,a);});
		})));
	}
	public void exit_automata(AutomataComponent a){
		StartCoroutine(MultiAction.run_list (this, exit_actions.Select ((StateEvent evn) => {
			return new ActionWrapper (() => {evn(this,a);});
		})));
		visitors.Remove(a);
		Debug.Log (instance_name + " visitor count -:" + visitors.Count);
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
		trans.instance_name = name;
		trans.auto_run = autorun;
		trans.from_state = this;
		trans.to_state = to;
		if (test != null) {
			//To-do: update this to take multiple events.
			trans.tests.Add(test);
		}
		return this;
	}
	public StateComponent on_entry(Action act){
		return on_entry((StateComponent s, AutomataComponent a)=>{
			act();
		});
	}
	public StateComponent on_entry(StateEvent act){
		entry_actions.Add (act);
		return this;
	}
	public StateComponent on_update(Action act){
		return on_update((StateComponent s, AutomataComponent a)=>{
			act();
		});
	}
	public StateComponent on_update(StateEvent act){
		update_actions.Add (act);
		return this;
	}
	public StateComponent on_exit(Action act){
		return on_exit((StateComponent s, AutomataComponent a)=>{
			act();
		});
	}
	public StateComponent on_exit(StateEvent act){
		exit_actions.Add (act);
		return this;
	}
}