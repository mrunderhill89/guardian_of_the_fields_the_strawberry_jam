using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFSM_Result{
	protected List<Action> actions;
	public HFSM_Transition trans = null;
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

public class HFSM_Transition{
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
		return false;
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
		this.children = new HashSet<HFSM_State> ();
		this.transitions = new List<HFSM_Transition> ();
	}

	//Children
	protected HashSet<HFSM_State> _children;
	public HashSet<HFSM_State> children {
		get{ return _children; }
		private set{ _children = value; }
	}
	public int size{
		get{ return this.children.Count;}
	}
	//Transitions
	protected List<HFSM_Transition> _transitions;
	public List<HFSM_Transition> transitions {
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
			HFSM_State prev = this.parent;
			HFSM_State next = value;
			if (prev != next) {
				if (prev != null && prev.has_child (this)) {
					prev.remove_child (this);
				}
				this._parent = next;
				if (next != null) {
					this.level = next.level + 1;
					if (!next.has_child (this)) {
						next.add_child (this);
					}
				} else {
					this.level = 1;
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
		this.children.Add(that);
		if (this.initial == null) {
			this.initial = that;
		}
		that.parent = this;
		return this;
	}
	public bool has_child(HFSM_State that){
		return this.children.Contains(that);
	}
	public HFSM_State remove_child(HFSM_State child){
		this.children.Remove(child);
		child.parent = null;
		return this;
	}
	//Add/Check/Remove Transitions
	public HFSM_State add_transition(HFSM_Transition trans){
		this.transitions.Add(trans);
		return this;
	}
	public bool has_transition(HFSM_Transition trans){
		return this.transitions.Contains(trans);
	}
	public HFSM_State remove_transition(HFSM_Transition trans){
		this.transitions.Remove (trans);
		return this;
	}
	//Update
	protected HFSM_Result _update(HFSM_Result result){
		if (this.current == null) {
			if (this.size == 0) {
				result.add_action (this.on_update ());
			} else if (this.initial != null){
				this.current = this.initial;
				result.add_action (this.current.on_entry ());
			}
		} else {
			HFSM_Transition trig = null;
			trig = this.current.transitions.Find ((t) => {
				return t.test();
			});
			if (trig != null) {
				result.trans = trig;
				result.level = trig.level;
			} else {
				result.add_action(this.on_update ());
				result = this.current._update (result);
			}
			if (result.trans != null) {
				HFSM_State target = result.trans.to_state;
				//Find common ancestor between both states and the paths between them.
				List<HFSM_State> up_path = new List<HFSM_State>();
				List<HFSM_State> down_path = new List<HFSM_State>();
				HFSM_State up = current;
				HFSM_State down = target;
				down_path.Add(down);
				while (up != down) {
					if (up.level > down.level || down == null) {
						up = up.parent;
						up_path.Add(up);
					} else {
						down = down.parent;
						down_path.Add(down);
					}
				};
				for (int u = 0; u < up_path.Count-1; u++) {
					up = up_path [u];
					if (up.current != null) {
						result.add_action(up.current.on_exit());
						up.current = null;
					}
				};
				for (int d = down_path.Count-1; d > 0; d--){
					down = down_path [d];
					if (down.current != down_path [d - 1]) {
						if (down.current != null) {
							result.add_action (down.current.on_exit ());
						}
						down.current = down_path [d - 1];
						result.add_action(down_path[d-1].on_entry());
					}
				};
				result.trans = null;
			}
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