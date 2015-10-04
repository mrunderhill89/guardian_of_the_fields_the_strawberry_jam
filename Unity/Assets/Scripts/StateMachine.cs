using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class StateMachine : BetterBehaviour {
	public Dictionary<string, State> states;
	public Dictionary<string, Transition> transitions;
	public Dictionary<string, Automata> automatum;

	void Awake(){
		if (states == null) states = new Dictionary<string, State>();
		if (transitions == null) transitions = new Dictionary<string, Transition>();
		if (automatum == null) automatum = new Dictionary<string, Automata>();
	}
	//States
	public State state(string name){
		if (!states.ContainsKey (name)) {
			State s = gameObject.AddComponent<State>();
			s.instance_name = name;
			return state(name,s);
		}
		return states[name];
	}
	public State state(string name, State s){
		states [name] = s;
		return s;
	}
	public StateMachine new_state(string name, Action<State> apply = null){
		State s = gameObject.AddComponent<State> ();
		add_state(name, s);
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
	//Transitions
	public Transition transition(string name){
		if (!transitions.ContainsKey (name)) {
			Transition t = gameObject.AddComponent<Transition>();
			t.instance_name = name;
			return transition(name,t);
		}
		return transitions[name];
	}
	public Transition transition(string name, Transition t){
		transitions[name] = t;
		return t;
	}
	public StateMachine new_transition(string name, Action<Transition> apply = null){
		Transition t = gameObject.AddComponent<Transition> ();
		add_transition(name, t);
		if (apply != null) {
			apply(t);
		}
		return this;
	}
	public StateMachine add_transition(string name, Transition t){
		t.instance_name = name;
		transition(name, t);
		return this;
	}
	//Automata
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

}
