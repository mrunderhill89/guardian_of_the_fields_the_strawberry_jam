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

	public State _from;
	public State _to;
	public bool _auto_run;
	public State _pivot;
	public State pivot{
		get{return _pivot;}
		protected set{_pivot = value;}
	}
	public List<State> _downswing;
	public List<State> downswing{
		get{return _downswing;}
		protected set{_downswing = value;}
	}
	protected int _priority;
	public List<TransitionTest> tests;
	public List<TransitionEvent> entry_actions;
	public List<TransitionEvent> transfer_actions;
	public List<TransitionEvent> exit_actions;

	#endregion

	void Awake(){
		_downswing = new List<State>();
		tests = new List<TransitionTest>();
		entry_actions = new List<TransitionEvent>();
		transfer_actions = new List<TransitionEvent>();
		exit_actions = new List<TransitionEvent>();
	}
	void Start(){
		//generate_path();
	}
	void Update(){
		if (auto_run()){
			trigger();
		}
	}
	#region Public methods

	public State from()
	{
		return _from;
	}

	public Transition from(State state)
	{
		_from = state;
		return this;
	}

	public State to()
	{
		return _to;
	}

	public Transition to(State state)
	{
		_to = state;
		return this;
	}

	public bool auto_run()
	{
		return _auto_run;
	}

	public Transition auto_run(bool value)
	{
		_auto_run = value;
		return this;
	}

	public Transition generate_path()
	{
		
		State up = from();
		State down = to();
		pivot = null;
		List<State> upswing = new List<State>();
		downswing.Clear();
		upswing.Add(up);
		downswing.Add(down);
		while(pivot == null && up != null && down != null){
			if (up == down){
				pivot = up;
			} else {
				if (up.get_level() > down.get_level() || down.parent() == null){
					up = up.parent();
					upswing.Add(up);
				} else {
					down = down.parent();
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

	public int priority(){
		return _priority;
	}

	public Transition priority(int value){
		_priority = value;
		return this;
	}

	public int CompareTo(Transition that){
		//The values are in reverse so that a default Sort() puts high priority first.
		return that._priority.CompareTo(this._priority);
	}

	public Transition add_test(TransitionTest test)
	{
		tests.Add(test);
		return this;
	}

	public Transition register_event(UnityEvent ue){
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
		foreach(TransitionTest test in tests){
			if (!test.run(a,this)) return false;
		}
		return true;
	}

	public void trigger()
	{
		foreach(Automata a in from().visitors){
			a.add_transition(this);
		}
	}

	public void trigger_single(Automata a)
	{
		a.add_transition(this);
	}

	#endregion


}

