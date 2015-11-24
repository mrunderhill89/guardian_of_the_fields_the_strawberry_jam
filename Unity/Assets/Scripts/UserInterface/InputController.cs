using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;

public class InputController: BetterBehaviour{
	protected static InputController _main = null;
	public static InputController main{
		get{ return _main;}
		private set{ _main = value;}
	}

	protected Dictionary<string, bool> went = new Dictionary<string, bool> ();
	public Dictionary<string, UnityEvent> direction_events = new Dictionary<string, UnityEvent> ();
	public Dictionary<string, List<Transition>> direction_transitions = new Dictionary<string, List<Transition>> ();

	protected void read_dir(string axis_name, string dir_name, bool gt){
		if (!went.ContainsKey (dir_name)) {
			went[dir_name] = false;
		}
		double axis = Input.GetAxis(axis_name);
		if (axis != 0.0 && (axis<0.0 ^ gt)){
			if (!went[dir_name]) {
				went[dir_name] = true;
				if (direction_events.ContainsKey(dir_name)){
					direction_events[dir_name].Invoke();
				}
			}
		} else {
			went[dir_name] = false;
		};
	}

	public InputController invoke_dir(string name){
		UnityEvent evn = direction_events[name];
		if (evn != null){ evn.Invoke(); }
		return this;
	}

	public UnityEvent on_dir(string name){
		if (!direction_events.ContainsKey (name)) {
			direction_events[name] = new UnityEvent();
		}
		return direction_events [name];
	}

	public Transition register_transition(Transition t, string direction){
		return register_transition (t, new string[]{direction});
	}
	public Transition register_transition(Transition t, string[] directions){
		foreach(string dir_name in directions){
			if (!direction_transitions.ContainsKey (dir_name)) {
				direction_transitions[dir_name] = new List<Transition>();
			}
			t.add_trigger(on_dir(dir_name));
			direction_transitions [dir_name].Add (t);
		}
		return t;
	}

	void Awake(){
		main = this;
		if (direction_events == null) {
			direction_events = new Dictionary<string, UnityEvent> ();
		}
		if (direction_transitions == null) {
			direction_transitions = new Dictionary<string, List<Transition>> ();
		}
		if (went == null) {
			went = new Dictionary<string, bool> ();
		}
	}

	void Update(){
		read_dir ("Horizontal", "left", false);
		read_dir ("Horizontal", "right", true);
		read_dir ("Vertical", "up", true);
		read_dir ("Vertical", "down", false);
	}
}
