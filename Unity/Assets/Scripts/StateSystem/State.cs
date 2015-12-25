using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

public class State : NamedBehavior
{

	#region Attributes

	[Show]
	protected State _parent;
	public State parent{
		get{return _parent;}
		private set{ chain_parent(value); }
	}
	public State chain_parent(State p){
		if (p != this)	_parent = p;
		return this;
	}
	[Show]
	protected State _initial;
	protected Func<State> _initial_f;
	public State initial{
		get{
			if (_initial == null && _initial_f != null)
				return _initial_f();
			return _initial;
		}
		set{ chain_initial(value);}
	}
	public State chain_initial(State i){
		_initial = i;
		return this;
	}
	public State initial_function(Func<State> f){
		_initial_f = f;
		return this;
	}

	protected List<StateEvent> entry_actions = new List<StateEvent>();
	protected List<StateEvent> update_actions = new List<StateEvent>();
	protected List<StateEvent> update_own_actions = new List<StateEvent>();
	protected List<StateEvent> exit_actions = new List<StateEvent>();
	protected HashSet<Automata> _visitors = new HashSet<Automata>();
	[Show]
	public HashSet<Automata> visitors{
		get{return _visitors;}
		protected set{_visitors = value;}
	}
	public bool is_visited(){
		return visitors.Count > 0;
	}

	#endregion

	#region Public methods

	public int get_level(int tail = 1)
	{
		if (parent != null){
			return parent.get_level(tail+1);
		}
		return tail;
	}

	public void invoke_entry(Automata a){
		visitors.Add(a);
		foreach (StateEvent e in entry_actions) {
			e.run(a,this);
		}
	}

	public void invoke_update(Automata a){
		foreach (StateEvent e in update_actions) {
			e.run(a,this);
		}
	}

	public void invoke_update_own(Automata a){
		foreach (StateEvent e in update_own_actions) {
			e.run(a,this);
		}
	}
	
	public void invoke_exit(Automata a){
		visitors.Remove(a);
		foreach (StateEvent e in exit_actions) {
			e.run(a,this);
		}
	}

	public int count(){
		return visitors.Count;
	}

	public IEnumerable<Automata> own_visitors(){
		return visitors.Where((Automata a)=>{
			return a.visiting_own(this);
		});
	}

	public int count_own(){
		return visitors.Count((Automata a)=>{
			return a.visiting_own(this);
		});
	}
	public bool is_own_visited(){
		return visitors.ToList().Find((Automata a)=>{
			return a.visiting_own(this);
		}) != null;
	}

	public int count_travellers(){
		return visitors.Count((Automata a)=>{
			return a.is_travelling();
		});
	}

	public bool has_travellers(){
		return visitors.ToList().Find((Automata a)=>{
			return a.is_travelling();
		}) != null;
	}
	
	public State eject_all(int steps = 1){
		foreach(Automata a in visitors.ToArray()){
			a.eject(steps);
		}
		return this;
	}

	public State add_child(State child, bool set_initial = false){
		child.chain_parent(this);
		if (set_initial) {
			chain_initial(child);
		}
		return this;
	}

	public State add_transition(Transition trans)
	{
		trans.chain_from(this);
		return this;
	}

	public State on_entry(StateEvent evn)
	{
		entry_actions.Add(evn);
		return this;
	}

	public State on_update(StateEvent evn)
	{
		update_actions.Add(evn);
		return this;
	}

	public State on_update_own(StateEvent evn)
	{
		update_own_actions.Add(evn);
		return this;
	}

	public State on_exit(StateEvent evn)
	{
		exit_actions.Add(evn);
		return this;
	}

	public static State GetState(GameObject obj, State existing = null){
		if (existing != null)
			return existing;
		if (obj.GetComponent<State>() == null)
			return obj.AddComponent<State>();
		return obj.GetComponent<State>();
	}
	
	#endregion

}

