using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using TestEventFull = System.Func<AutomataComponent, StateComponent,StateComponent, bool>;
using TestEventA = System.Func<AutomataComponent, bool>;
using TestEventNeutral = System.Func<bool>;
public class TransitionComponent : NamedBehavior {
	public StateComponent from_state;
	public TransitionComponent set_from_state(StateComponent state){
		from_state = state;
		return this;
	}
	public StateComponent to_state;
	public TransitionComponent set_to_state(StateComponent state){
		to_state = state;
		return this;
	}
	
	public List<TestEventFull> tests;
	public TransitionComponent test(TestEventFull t){
		tests.Add (t);
		return this;
	}
	public TransitionComponent test(TestEventA t){
		return test ((AutomataComponent a, StateComponent from, StateComponent to) => {
			return t(a);
		});
	}
	public TransitionComponent test(TestEventNeutral t){
		return test ((AutomataComponent a, StateComponent from, StateComponent to) => {
			return t();
		});
	}
	
	public bool auto_run = false;
	public TransitionComponent set_auto_run(bool value){
		this.auto_run = value;
		return this;
	}
	
	public StateComponent pivot;
	public List<StateComponent> downswing;
	public TransitionComponent generate_path(){
		downswing.Clear();
		StateComponent up = from_state;
		StateComponent down = to_state;
		pivot = null;
		downswing.Add(down);
		while(pivot == null && up != null && down != null){
			if (up == down){
				pivot = up;
			} else {
				if (up.get_level() > down.get_level() || down.parent == null){
					up = up.parent;
				} else {
					down = down.parent;
					downswing.Add(down);
				}
			}
		}
		downswing.Reverse();
		if (pivot == null){
			Debug.LogError("Unable to complete path.");
			foreach(StateComponent state in downswing){
				if (state != null){
					Debug.LogWarning(state.instance_name);
				} else {
					Debug.LogWarning("null");
				}
			}
			downswing.Clear();
		}
		return this;
	}

	public void trigger(){
		if (this.pivot != null) {
			List<AutomataComponent> list = from_state.visitors.ToList<AutomataComponent>();
			foreach(AutomataComponent a in list){
				trigger_single(a);
			}
		} else {
			Debug.LogError("Attempting to trigger an incomplete transition:"+instance_name);
		}
	}
	public bool trigger_single(AutomataComponent a){
		if (a.visiting(from_state)){return _trigger_single(a);}
		return false;
	}
	protected bool _trigger_single(AutomataComponent a){
		foreach(TestEventFull t in tests){
			if (!t(a,from_state,to_state)) return false;
		}
		a.move_transition(this);
		return true;
	}

	void Awake(){
		if (tests == null) {
			tests = new List<TestEventFull> ();
		}
		downswing = new List<StateComponent> ();
	}

	void Start(){
		generate_path();
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