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
	public List<StateComponent> upswing;
	protected StateComponent pivot;
	public List<StateComponent> downswing;

	void Start(){
		tests = new List<Func<bool>>();
		upswing = new List<StateComponent>();
		downswing = new List<StateComponent>();
		generate_path();
	}
	
	void generate_path(){
		upswing.Clear();
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
					upswing.Add(up);
				} else {
					down = down.parent;
					downswing.Add(down);
				}
			}
		}
		downswing.Reverse();
		if (pivot == null){
			Debug.LogError("Unable to complete path.");
			upswing.Clear();
			downswing.Clear();
		}
	}
	
	public bool trigger(){
		foreach(Func<bool> test in tests){
			if (!test()){return false;}
		}
		from_state.handle_transition(this);
		return true;
	}
	
	void Update(){
		if (auto_run){
			trigger();
		}
	}
}