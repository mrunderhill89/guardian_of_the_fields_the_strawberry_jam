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

	public State _parent;
	public State parent(){return _parent;}
	public State parent(State p){
		_parent = p;
		return this;
	}
	public State _initial;
	protected Func<Automata,State> _f_initial;
	public State initial(Automata a){
		if (_initial != null) return _initial;
		if (_f_initial != null) return _f_initial(a);
		return null;
	}
	public State initial(State i){
		_initial = i;
		return this;
	}
	public State initial(Func<Automata,State> f){
		_f_initial = f;
		return this;
	}
	public State initial(Func<State> f){
		_f_initial = (Automata a)=>{return f();};
		return this;
	}

	public List<StateEvent> entry_actions;
	public List<StateEvent> update_actions;
	public List<StateEvent> update_own_actions;
	public List<StateEvent> exit_actions;
	public HashSet<Automata> _visitors;
	public HashSet<Automata> visitors{
		get{return _visitors;}
		protected set{_visitors = value;}
	}
	public bool is_visited(){
		return visitors.Count > 0;
	}

	#endregion

	void Awake(){
		visitors = new HashSet<Automata>();
		entry_actions = new List<StateEvent> ();
		update_actions = new List<StateEvent> ();
		update_own_actions = new List<StateEvent> ();
		exit_actions = new List<StateEvent> ();
	}

	#region Public methods

	public int get_level(int tail = 1)
	{
		if (parent() != null){
			return parent().get_level(tail+1);
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

	public State add_child(State child, bool set_initial = false){
		child.parent(this);
		if (set_initial) {
			initial(child);
		}
		return this;
	}

	public State add_transition(Transition trans)
	{
		trans.from(this);
		if (trans.to() != null) trans.generate_path();
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

	#endregion

}

