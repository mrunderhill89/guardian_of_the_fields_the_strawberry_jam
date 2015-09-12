using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

public class Automata : BetterBehaviour
{

	#region Attributes

	public State _current;
	public State current{
		get{return _current;}
		protected set{_current = value;}
	}
	public List<State> stack;
	public HashSet<Transition> transitions;

	#endregion

	#region Unity Behavior Methods

	void Awake(){
		stack = new List<State>();
		transitions = new HashSet<Transition>();
	}

	void Start(){
		if (current != null){
			stack.Add(current);
			current.enter_automata(this);
		}
	}

	void Update(){
		if (current != null){
			if (transitions.Count > 0){
				Debug.Log("Firing Transitions:"+transitions.Count);
				List<Transition> sorted = transitions.ToList<Transition>();
				sorted.Sort();
				foreach(Transition trans in sorted){
					if (current == trans.from() && trans.test_single(this)){
						move_transition(trans);
					}
				}
				transitions.Clear();
			} else {
				if(current.initial != null){
					move_direct(current.initial);
				} else {
					foreach(State state in stack.ToArray()){
						state.update_automata(this);
					}
				}
			}
		}
	}

	#endregion

	#region Public methods

	public Automata move_direct(State to)
	{
		if (to != null){
			if (current == null || to.parent == current || current.parent == to){
				if (current == null || to.parent == current){
					//Parent->Child
					stack.Add(to);
					current = to;
					to.enter_automata(this);
				} else { //(current.parent == to)
					//Child->Parent
					stack.Remove(current);
					current.exit_automata(this);
					current = to;
				}
			} else if (current != to){
				Debug.LogError("Attempted to move directly between non-connected states:\n"+
							   current.instance_name+"=>"+to.instance_name);
			}
		} else {
			Debug.LogError("Automata should never be moved out of the state system once placed. Currently in "+
				current.instance_name+".");
		}
		return this;
	}

	protected Automata move_transition(Transition trans)
	{
		// if the from_state is above ours, we need to get there first
		// transition.on_start(this);
		while (current != trans.pivot) {
			if (current == null){
				Debug.LogError("Automata not on transition path.");
				return this;
			}
			move_direct (current.parent);
		}
		// transition.on_transfer(this);
		foreach(State down in trans.downswing){
			move_direct(down);
		}
		// transition.on_finish(this);
		return this;
	}

	public Automata add_transition(Transition trans)
	{
		transitions.Add(trans);
		return this;
	}

	public bool visiting(State state)
	{
		return stack.Contains(state);
	}

	#endregion

}

