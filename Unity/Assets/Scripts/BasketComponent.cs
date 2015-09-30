﻿using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;

public class BasketComponent : BetterBehaviour {
	public State slot;
	public Transition drop;
	protected Dictionary<GameObject, Vector3> valid_positions;
	void Awake () {
		slot = NamedBehavior.GetOrCreateComponentByName<State>(gameObject, "slot");
		drop = NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "deposit");
		valid_positions = new Dictionary<GameObject, Vector3>();
	}

	void Start(){
		StrawberryStateMachine state_machine = SingletonBehavior.get_instance<StrawberryStateMachine>();
		slot.parent (state_machine.states ["basket"])
			.on_entry (new StateEvent(ParentToBasket))
			.on_exit (new StateEvent(UnparentToBasket))
			.on_update(new StateEvent(UpdatePhysics));
		drop.from(state_machine.states["fall"])
			.priority(2)
			.to(slot)
			.add_test(new TransitionTest((Automata a)=>{
				if (a.gameObject.GetComponent<StrawberryComponent>() == null){
					return false;
				}
				return true;
			}))
			.generate_path();
	}

	void ParentToBasket(Automata a){
		a.gameObject.transform.SetParent(transform, true);
	}

	void UnparentToBasket(Automata a){
		//a.gameObject.transform.SetParent(null, true);
	}

	static void UpdatePhysics(Automata a){
		Rigidbody body = a.gameObject.GetComponent<Rigidbody> ();
		if (body != null) {
			if (!SingletonBehavior.get_instance<GameStateManager>().states["pack"].is_visited()){
				body.isKinematic = true;
			} else {
				body.isKinematic = false;
			}
		}
	}

	void OnTriggerStay(Collider that) {
		GameObject obj = that.gameObject;
		Automata a = obj.GetComponent<Automata>();
		if (a != null){
			drop.trigger_single(a);
			valid_positions[obj] = obj.transform.position;
		}
	}

	void OnTriggerExit(Collider that){
		GameObject obj = that.gameObject;
		Automata a = obj.GetComponent<Automata>();
		if (a != null && a.current == slot){
			Debug.Log("Object falling out of basket:"+obj.name);
			obj.transform.position = valid_positions[obj];
			Rigidbody body = obj.GetComponent<Rigidbody>();
			body.velocity = Vector3.zero;
		} else {
			valid_positions.Remove(obj);
		}
	}
	void Update () {
	
	}
}
