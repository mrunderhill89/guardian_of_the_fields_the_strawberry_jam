﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class StateMachine : BetterBehaviour {
	protected Dictionary<string, State> _states = new Dictionary<string, State>();
	[Show]
	public Dictionary<string,State> states{
		get{return _states;}
		private set{_states=value;}
	}
	protected Dictionary<string, Transition> _transitions = new Dictionary<string, Transition>();
	[Show]
	public Dictionary<string,Transition> transitions{
		get{return _transitions;}
		private set{_transitions=value;}
	}
	protected Dictionary<string, Automata> _automatum  = new Dictionary<string, Automata>();
	[Show]
	public Dictionary<string,Automata> automatum{
		get{return _automatum;}
		private set{_automatum=value;}
	}

	//States
	public State read_state(string name){
		return states [name];
	}
	[Show]
	public State state(string name){
		if (!has_state(name)) {
			State s = NamedBehavior.GetOrCreateComponentByName<State>(gameObject,name);
			s.instance_name = name;
			return state(name,s);
		}
		return states[name];
	}
	public State state(string name, State s){
		states[name] = s;
		return s;
	}
	public StateMachine new_state(string name, Action<State> apply = null){
		State s = state(name);
		if (apply != null) {
			apply(s);
		}
		return this;
	}
	public StateMachine add_state(string name, State s){
		s.instance_name = name;
		state(name, s);
		return this;
	}
	public bool has_state(string name){
		return states.ContainsKey(name);
	}

	public bool is_state_visited(string name){
		if (!has_state (name))
			return false;
		return read_state(name).is_visited();
	}
	//Transitions
	[Show]
	public Transition transition(string name){
		if (!transitions.ContainsKey (name)) {
			Transition t = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject,name);
			return transition(name,t);
		}
		return transitions[name];
	}
	public Transition transition(string name, Transition t){
		transitions[name] = t;
		return t;
	}
	public StateMachine new_transition(string name, Action<Transition> apply = null){
		Transition t = transition(name);
		if (apply != null) {
			apply(t);
		}
		return this;
	}
	public StateMachine add_transition(string name, Transition t){
		t.instance_name = name;
		return add_transition(t);
	}
	public StateMachine add_transition(Transition t){
		transition(t.instance_name, t);
		return this;
	}
	//Automata
	[Show]
	public Automata automata(string name){
		if (!automatum.ContainsKey(name)) {
			Automata a = gameObject.AddComponent<Automata>();
			return automata(name,a);
		}
		return automatum[name];
	}
	public Automata automata(string name, Automata a){
		automatum [name] = a;
		return a;
	}
	public StateMachine new_automata(string name, Action<Automata> apply = null){
		Automata a = gameObject.AddComponent<Automata>();
		automata(name, a);
		if (apply != null) {
			apply(a);
		}
		return this;
	}
	public int count_automata(){
		return automatum.Count;
	}

	public void trigger_transition(string name){
		if (transitions.ContainsKey (name)) {
			transitions[name].trigger();
		}
	}

	public T match<T>(Dictionary<string,T> cases, T def = default(T)){
		foreach(KeyValuePair<string, T> c in cases)
		{
			if (has_state(c.Key) && read_state(c.Key).is_visited()){
				return c.Value;
			}
		}
		return def;
	}
	public T match<T>(IEnumerable<string> names, Func<State, T> value, T def = default(T)){
		foreach(string name in names){
			if (has_state(name) && read_state(name).is_visited()){
				return value(state(name));
			}
		}
		return def;
	}
	public T match<T>(IEnumerable<string> names, T value, T def = default(T)){
		foreach(string name in names){
			if (has_state(name) && read_state(name).is_visited()){
				return value;
			}
		}
		return def;
	}
}