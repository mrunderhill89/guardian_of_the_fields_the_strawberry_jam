using Vexe.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using StateChain = System.Collections.Generic.List<StateComponent>;

public class StateComponent : NamedBehavior {
	public StateComponent parent;
	public StateComponent initial;
	public StateComponent current;
	
	public UnityEvent on_update;
	public UnityEvent on_enter;
	public UnityEvent on_exit;

	public int get_level(int tail = 1){
		if (parent != null){
			return parent.get_level(tail+1);
		}
		return tail;
	}
	
	public bool is_active(){
		if (parent != null){
			if (parent.current != this){
				return false;
			} else {
				return parent.is_active();
			}
		}
		return true;
	}
	
	private List<ActionWrapper> add_event(List<ActionWrapper> actions, UnityEvent evn){
		if (evn != null){
			actions.Add(new ActionWrapper(evn));
		}
		return actions;
	}
	public List<ActionWrapper> get_actions(List<ActionWrapper> actions = null){
		if (actions == null){
			actions = new List<ActionWrapper>();
		}
		if (this.current == null) {
			if (this.initial != null){
				this.current = this.initial;
				actions = add_event(actions, this.current.on_enter);
			} else {
				actions = add_event(actions, this.on_update);
			}
		} else {
			actions = add_event(actions, this.current.on_update);
			return this.current.get_actions(actions);
		}
		return actions;
	}
	
	void Start(){
		StartCoroutine(coroutine());
	}
	
	public IEnumerator coroutine(){
		List<ActionWrapper> actions;
		while(get_level()<=1){
			actions = get_actions();
			if (actions.Count > 0){
				foreach(ActionWrapper act in actions){
					yield return StartCoroutine(act.run());
				}
			}
			yield return null;
		}
	}
}