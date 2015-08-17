using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFSM_Result{
	protected List<Action> actions;
	public I_HFSM_Transition trans = null;
	public int level = 0;
	public HFSM_Result(){
		actions = new List <Action> ();
	}
	public HFSM_Result add_action(Action action){
		if (action != null) {
			actions.Add (action);
		}
		return this;
	}
	public HFSM_Result add_actions(List<Action> _actions){
		_actions.ForEach ((action) => {
			this.add_action(action);
		});
		return this;
	}
	public HFSM_Result run(){
		actions.ForEach ((action) => {
			action();
		});
		return this;
	}
}

public interface I_HFSM_Transition{
	bool test ();
	Action action();
	HFSM_State from_state{ get;}
	HFSM_State to_state{ get;}
	HFSM_Result trigger(HFSM_Result r);
	int level{get;}
}

public class HFSM_Transition : I_HFSM_Transition{
	//Remember previous state?
	protected bool _memory = false;
	public bool memory(){
		return _memory;
	}
	public HFSM_Transition memory(bool m){
		_memory = m;
		return this;
	}
	//Condition
	protected Func<bool> _condition = null;
	public Func<bool> condition(){
		return _condition;
	}
	public HFSM_Transition condition(Func<bool> c){
		_condition = c;
		return this;
	}
	public bool test(){
		if (this._condition != null) {
			bool result = this._condition ();
			return result;
		}
		return true;
	}
	//Action
	protected Action _action = null;
	public Action action(){
		return _action;
	}
	public HFSM_Transition action(Action a){
		_action = a;
		return this;
	}
	//To and From states
	protected HFSM_State _to_state = null;
	public HFSM_State to_state{
		get{ return _to_state;}
		private set{ _to_state = value; _level = -1;}
	}
	protected HFSM_State _from_state = null;
	public HFSM_State from_state{
		get{ return _from_state;}
		private set{ _from_state = value; _level = -1;}
	}
	//Level
	protected int _level = -1;
	public int level {
		get{
			return from_state.level - to_state.level;
		}
	}
	//Trigger
	public HFSM_Result trigger(HFSM_Result result = null){
		if (result == null) {
			result = new HFSM_Result ();
		}
		if (test () && from_state.is_active()) {
			result.trans = this;
			//Find common ancestor between both states and the paths between them.
			List<HFSM_State> up_path = new List<HFSM_State> ();
			List<HFSM_State> down_path = new List<HFSM_State> ();
			HFSM_State up = from_state;
			HFSM_State down = to_state;
			down_path.Add (down);
			while (up != down) {
				if (up.level > down.level || down == null) {
					up = up.parent;
					up_path.Add (up);
				} else {
					down = down.parent;
					down_path.Add (down);
				}
			}
			for (int u = 0; u < up_path.Count - 1; u++) {
				up = up_path [u];
				if (up.current != null) {
					result.add_action (up.current.on_exit ());
					up.current = null;
				}
			}
			result.add_action(action());
			for (int d = down_path.Count - 1; d > 0; d--) {
				down = down_path [d];
				if (down.current != down_path [d - 1]) {
					if (down.current != null) {
						result.add_action (down.current.on_exit ());
					}
					down.current = down_path [d - 1];
					result.add_action (down_path [d - 1].on_entry ());
				}
			}
		}
		return result;
	}
	//Constructor
	public HFSM_Transition(HFSM_State _from, HFSM_State _to, Func<bool> _c = null){
		this.to_state = _to;
		this.from_state = _from;
		this.condition(_c);
	}
}

public class HFSM_State {
	//Constructor
	public HFSM_State (){
		this.transitions = new List<I_HFSM_Transition> ();
	}

	//Transitions
	protected List<I_HFSM_Transition> _transitions;
	public List<I_HFSM_Transition> transitions {
		get{ return _transitions; }
		private set{ _transitions = value; }
	}
	//Level
	protected int _level = 1;
	public int level {
		get{ return _level; }
		private set{ _level = value; }
	}
	//Parent
	protected HFSM_State _parent = null;
	public HFSM_State parent {
		get{ return _parent;}
		set{
			if (_parent != value){
				if (_parent != null){
					if (_parent.initial == this) _parent.initial = null;
					if (_parent.current == this) _parent.current = null;
				}
				_parent = value;
				if (value != null){
					if (value.initial == null) {
						value.initial = this;
					}
					level = value.level+1;
				} else {
					level = 1;
				}
			}
		}
	}
	//Initial
	protected HFSM_State _initial = null;
	public HFSM_State initial {
		get { return _initial;}
		set { 
			HFSM_State next = value;
			if (next == null || this.has_child (next)) {
				_initial = next;
			} else {
				throw(new Exception("HFSM_State.initial: Attempted to assign a non-child state."));
			}
		}
	}
	//Current
	protected HFSM_State _current = null;
	public HFSM_State current {
		get { return _current;}
		set { 
			HFSM_State next = value;
			if (next == null || this.has_child (next)) {
				_current = next;
			} else {
				throw(new Exception("HFSM_State.current: Attempted to assign a non-child state."));
			}
		}
	}
	//Actions - On Entry
	protected Action _on_entry = null;
	public Action on_entry(){
		return _on_entry;
	}
	public HFSM_State on_entry(Action action){
		_on_entry = action;
		return this;
	}
	//Actions - On Update
	protected Action _on_update = null;
	public Action on_update(){
		return _on_update;
	}
	public HFSM_State on_update(Action action){
		_on_update = action;
		return this;
	}
	//Actions - On Exit
	protected Action _on_exit = null;
	public Action on_exit(){
		return _on_exit;
	}
	public HFSM_State on_exit(Action action){
		_on_exit = action;
		return this;
	}
	//Add/Check/Remove Children
	public HFSM_State add_child(HFSM_State that){
		that.parent = this;
		return this;
	}
	public bool has_child(HFSM_State that){
		return that.parent == this;
	}
	public HFSM_State remove_child(HFSM_State child){
		child.parent = null;
		return this;
	}
	//Add/Check/Remove Transitions
	public HFSM_State add_transition(I_HFSM_Transition trans){
		this.transitions.Add(trans);
		return this;
	}
	public bool has_transition(I_HFSM_Transition trans){
		return this.transitions.Contains(trans);
	}
	public HFSM_State remove_transition(I_HFSM_Transition trans){
		this.transitions.Remove (trans);
		return this;
	}
	//Check Activity
	public bool is_active(){
		if (parent != null) {
			if (parent.current != this) {
				return false;
			}
			return parent.is_active();
		}
		return true;
	}
	//Update
	protected HFSM_Result _update(HFSM_Result result){
		if (this.current == null) {
			if (this.initial != null){
				this.current = this.initial;
				result.add_action (this.current.on_entry ());
			} else {
				result.add_action (this.on_update ());
			}
		} else {
			I_HFSM_Transition trig = null;
			trig = this.current.transitions.Find ((t) => {
				result = t.trigger(result);
				return result.trans != null;
			});
			if (result.trans == null) {
				result.add_action(this.on_update ());
				result = this.current._update (result);
			}
			result.trans = null;
		}
		return result;
	}
	public HFSM_Result update(){
		return this._update (new HFSM_Result ());
	}
	public HFSM_State run(){
		this.update().run();
		return this;
	}
}