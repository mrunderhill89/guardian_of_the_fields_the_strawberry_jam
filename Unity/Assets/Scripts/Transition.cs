using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

public class Transition : NamedBehavior, IComparable<Transition>
{

	#region Attributes

	protected State _from;
	[Show]
	public State from{
		get{return _from;}
		protected set{ chain_from(value); }
	}

	protected State _to;
	[Show]
	public State to{
		get{return _to;}
		protected set{ chain_to(value); }
	}
	protected bool _auto_run;
	[Show]
	public bool auto_run{
		get{return _auto_run;}
		protected set{ chain_auto_run(value); }
	}
	protected State _pivot;
	[Show]
	public State pivot{
		get{return _pivot;}
		protected set{_pivot = value;}
	}

	protected List<State> _downswing = new List<State>();
	[Show]
	public List<State> downswing{
		get{return _downswing;}
		protected set{_downswing = value;}
	}
	protected int _priority = 1;
	[Show]
	public int priority{
		get{return _priority;}
		protected set{ chain_priority(value); }
	}
	
	protected List<TransitionTest> tests = new List<TransitionTest>();
	protected List<TransitionEvent> entry_actions = new List<TransitionEvent>();
	protected List<TransitionEvent> transfer_actions = new List<TransitionEvent>();
	protected List<TransitionEvent> exit_actions = new List<TransitionEvent>();

	#endregion

	void Update(){
		if (auto_run && is_visited()){
			trigger();
		}
	}
	#region Public methods

	public Transition chain_from(State state)
	{
		_from = state;
		if (_to != null){
			return this.generate_path();
		}
		return this;
	}
	public Transition chain_to(State state)
	{
		_to = state;
		if (_from != null){
			return this.generate_path();
		}
		return this;
	}

	public Transition chain_auto_run(bool value)
	{
		_auto_run = value;
		return this;
	}

	public Transition generate_path()
	{
		
		State up = from;
		State down = to;
		pivot = null;
		List<State> upswing = new List<State>();
		downswing.Clear();
		upswing.Add(up);
		downswing.Add(down);
		while(pivot == null && up != null && down != null){
			if (up == down){
				pivot = up;
			} else {
				if (up.get_level() > down.get_level() || down.parent == null){
					up = up.parent;
					upswing.Add(up);
				} else {
					down = down.parent;
					downswing.Add(down);
				}
			}
		}
		downswing.Reverse();
		if (pivot == null){
			Debug.LogError("Unable to complete path:"+instance_name);
			Debug.LogWarning(
				upswing.Aggregate("Upswing:", (String str, State state)=>{
					if (state == null) return str+" null";
					return str+" "+state.instance_name;
				})
			);
			Debug.LogWarning(
				downswing.Aggregate("Downswing:", (String str, State state)=>{
					if (state == null) return str+" null";
					return str+" "+state.instance_name;
				})
			);
			downswing.Clear();
		}
		return this;
	}

	public Transition chain_priority(int value){
		_priority = value;
		return this;
	}

	public int CompareTo(Transition that){
		//The values are in reverse so that a default Sort() puts high priority first.
		return this._priority.CompareTo(that._priority);
	}

	public Transition add_test(TransitionTest test)
	{
		tests.Add(test);
		return this;
	}

	public Transition add_trigger(UnityEvent ue){
		ue.AddListener(trigger);
		return this;
	}

	public Transition on_entry(TransitionEvent evn){
		entry_actions.Add(evn);
		return this;
	}

	public Transition on_transfer(TransitionEvent evn){
		entry_actions.Add(evn);
		return this;
	}

	public Transition on_exit(TransitionEvent evn){
		entry_actions.Add(evn);
		return this;
	}

	public Transition invoke_entry(Automata a){
		foreach (TransitionEvent evn in entry_actions){
			evn.run(a,this);
		}
		return this;
	}

	public Transition invoke_transfer(Automata a){
		foreach (TransitionEvent evn in transfer_actions){
			evn.run(a,this);
		}
		return this;
	}

	public Transition invoke_exit(Automata a){
		foreach (TransitionEvent evn in exit_actions){
			evn.run(a,this);
		}
		return this;
	}

	public bool test_single(Automata a){
		if (!a.visiting (_from))
			return false;
		foreach(TransitionTest test in tests){
			if (!test.run(a,this)) return false;
		}
		return true;
	}

	public bool test_any(){
		if (_from != null){
			foreach(Automata a in _from.visitors){
				if (test_single(a)) return true;
			}
		}
		return false;
	}

	public bool test_all(){
		if (_from != null && _from.is_visited()){
			foreach(Automata a in _from.visitors){
				if (!test_single(a)) return false;
			}
			return true;
		}
		return false;
	}

	public void trigger()
	{
		if (_from != null){
			foreach(Automata a in _from.visitors){
				a.add_transition(this);
			}
		}
	}

	public void trigger_single(Automata a)
	{
		a.add_transition(this);
	}

	public bool is_visited(){
		return _from.is_visited();
	}

	#endregion


}

