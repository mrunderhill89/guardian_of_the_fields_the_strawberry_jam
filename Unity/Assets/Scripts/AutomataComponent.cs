using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using StateMove = Vexe.Runtime.Types.Tuple<StateComponent,StateComponent>;
public class AutomataComponent : BetterBehaviour {
	public StateComponent current;
	protected List<StateComponent> stack;
	
	void Awake(){
		stack = new List<StateComponent>();
	}
	void Start(){
		if (current != null){
			stack.Add(current);
			current.enter_automata(this);
		}
	}
	public void move_direct(StateComponent to){
		if (to.parent == current || current.parent == to){
			if (to.parent == current){
				//Parent->Child
				stack.Add(to);
				to.enter_automata(this);
			} else { //(current.parent == to)
				//Child->Parent
				stack.Remove(current);
				current.exit_automata(this);
			}
			current = to;
		} else if (current != to){
			Debug.LogError("Attempted to move directly between non-connected states.");
		}
	}
	public void move_transition(TransitionComponent trans){
		// if the from_state is above ours, we need to get there first
		// transition.on_start(this);
		while (current != trans.pivot) {
			if (current == null){
				Debug.LogError("Automata not on transition path.");
				return;
			}
			move_direct (current.parent);
		}
		// transition.on_transfer(this);
		foreach(StateComponent down in trans.downswing){
			move_direct(down);
		}
		// transition.on_finish(this);
	}
	void Update () {
		if (current != null){
			if(current.child != null){
				move_direct(current.child);
			} else {
				foreach(StateComponent state in stack){
					//state.update.OnNext(this);
					on_state(state);
				}
			}
		}
	}
	public void on_state(StateComponent state){
		state.update_automata (this);
	}
}