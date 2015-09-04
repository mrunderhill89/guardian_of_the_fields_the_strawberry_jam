using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

public class TransitionComponent : NamedBehavior {
	public StateComponent from_state;
	public StateComponent to_state;
	public List<Func<bool>> tests;
	public bool auto_run = false;
	public StateComponent _pivot;
	public StateComponent pivot { get; private set;}
	public List<StateComponent> downswing;

	void Awake(){
		tests = new List<Func<bool>> ();
		downswing = new List<StateComponent> ();
	}

	void Start(){
		generate_path();
	}
	
	void generate_path(){
		downswing.Clear();
		StateComponent up = from_state;
		StateComponent down = to_state;
		pivot = null;
		downswing.Add(down);
		while(pivot == null && up != null && down != null){
			if (up == down){
				pivot = up;
			} else {
				if (up.get_level() > down.get_level()){
					up = up.parent;
				} else {
					down = down.parent;
					downswing.Add(down);
				}
			}
		}
		downswing.Reverse();
		if (pivot == null){
			//Debug.LogError("Unable to complete path.");
			downswing.Clear();
		}
	}
	
	public bool trigger(){
		foreach(Func<bool> test in tests){
			if (!test()){return false;}
		}
		if (this.pivot != null) {
			from_state.handle_transition (this);
		} else {
			Debug.LogError("Attempting to trigger an incomplete transition.");
		}
		return true;
	}
	
	void Update(){
		if (this.pivot != null) {
			generate_path();
		}
		if (auto_run){
			trigger();
		}
	}
}