﻿using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		slot.chain_parent (state_machine.fsm.state("basket"))
			.on_entry (new StateEvent(ParentToBasket))
			.on_exit (new StateEvent(UnparentToBasket))
			.on_update(new StateEvent(UpdatePhysics));
		drop.chain_from(state_machine.fsm.state("fall"))
			.chain_auto_run(false)
			.chain_priority(2)
			.chain_to(slot)
			.add_test(new TransitionTest((Automata a)=>{
				if (a.gameObject.GetComponent<StrawberryComponent>() == null){
					return false;
				}
				return true;
			}));
	}

	void ParentToBasket(Automata a){
		a.gameObject.transform.SetParent(transform, true);
	}

	void UnparentToBasket(Automata a){
		a.gameObject.transform.SetParent(null, true);
	}

	public float get_berry_weight(){
		if (slot == null)
			return 0.0f;
		return slot.visitors.Select((Automata a) => {
			StrawberryComponent sb = a.GetComponent<StrawberryComponent> ();
			if (sb == null) return 0.0f;
			return sb.weight;
		}).Aggregate<float,float>(0.0f, (total, next) => {
			return total + next;
		});
	}

	[Show]
	public float total_weight{
		get{return this.get_berry_weight();}
	}

	static void UpdatePhysics(Automata a){
		Rigidbody body = a.gameObject.GetComponent<Rigidbody> ();
		if (body != null) {
			if (!SingletonBehavior.get_instance<GameStateManager>().basket_physics_enabled()){
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
			GameMessages.Log("Uh-oh, a strawberry fell out of your basket!");
			obj.transform.position = valid_positions[obj];
			Rigidbody body = obj.GetComponent<Rigidbody>();
			body.velocity = Vector3.zero;
		} else {
			valid_positions.Remove(obj);
		}
	}
}
