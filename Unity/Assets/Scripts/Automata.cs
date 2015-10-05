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
			current.invoke_entry(this);
		}
	}

	void Update(){
		if (current != null){
			if (transitions.Count > 0){
				List<Transition> sorted = transitions.ToList<Transition>();
				sorted.Sort();
				foreach(Transition trans in sorted){
					if (visiting(trans.to())){
						//Debug.LogWarning(name+" is already in destination state for transition:"+trans.instance_name);
					} else if(!visiting(trans.from())){
						/*Debug.LogWarning(name+" is not in starting state for transition:"+trans.instance_name
						+"\n Current:"+current.instance_name+" Needs:"+trans.from().instance_name);*/
					} else if (!trans.test_single(this)){
						//Debug.LogWarning(name+" failed tests for transition:"+trans.instance_name);
					} else {
						if (trans.priority() > 1){
							Debug.Log("Priority "+trans.priority()+" transition:"+trans.instance_name);
						}
						move_transition(trans);
						break;
					}
				}
				transitions.Clear();
			} else {
				State initial = current.initial(this);
				if(initial != null){
					move_direct(initial);
				} else {
					_current.invoke_update_own(this);
					foreach(State state in stack.ToArray()){
						state.invoke_update(this);
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
			if (current == null || to.parent() == current || current.parent() == to){
				if (current == null || to.parent() == current){
					//Parent->Child
					stack.Add(to);
					current = to;
					to.invoke_entry(this);
				} else { //(current.parent == to)
					//Child->Parent
					stack.Remove(current);
					current.invoke_exit(this);
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

	public Automata eject(int steps = 1){
		while (steps > 0 && _current.parent() != null){
			move_direct(_current.parent());
			steps--;
		}
		if (steps > 0){
			Debug.LogWarning("Automata.Eject: Could not eject completely. Steps remaining:"+steps);
		}
		return this;
	}

	protected Automata move_transition(Transition trans)
	{
		// if the from_state is above ours, we need to get to the pivot first
		trans.invoke_entry(this);
		while (current != trans.pivot) {
			if (current == null){
				Debug.LogError("Automata not on transition path.");
				return this;
			}
			move_direct (current.parent());
		}
		trans.invoke_transfer(this);
		foreach(State down in trans.downswing){
			move_direct(down);
		}
		trans.invoke_exit(this);
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

	public bool visiting_own(State state)
	{
		return current == state;
	}
	
	public bool is_travelling(){
		return current.initial(this) != null;
	}
	#endregion

}

