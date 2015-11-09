using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
public class StrawberryComponent : BetterBehaviour {
	public float quality = 1.00f;
	[Show]
	public float weight{
		get{return GameStartData.berry_density * (transform.localScale.x
			*transform.localScale.y
			*transform.localScale.z)
			;}
	}
	public enum BerryPenalty
	{
		Accepted,
		None,
		Small,
		Medium,
		Big
	}
	public BerryPenalty get_penalty_type(bool dropped = false){
		if (dropped) {
			if (is_under_ripe ()) {
				return BerryPenalty.Medium; //Dropped an underripe berry.
			} else {
				if (!is_over_ripe ()) {
					if (is_under_size ()) {
						return BerryPenalty.Medium; //Dropped a ripe but undersized berry.
					} else {
						return BerryPenalty.Big; //Dropped a perfectly good berry!
					}
				}
			}
			return BerryPenalty.None; //No penalty for dropping overripe berries.
		} else {
			if (is_over_ripe()){
				return BerryPenalty.Medium; //Held onto an overripe berry
			} else {
				if (is_under_ripe()){
					return BerryPenalty.Small; //Held onto an underripe berry, regardless of size
				} else if (is_under_size()){
					return BerryPenalty.None; //Held onto a berry that was ripe but undersized
				}
			}
			return BerryPenalty.Accepted;
		}
	}
	public float get_penalty_value(bool dropped = false){
		BerryPenalty penalty = get_penalty_type (dropped);
		if (!GameStartData.penalty_values.ContainsKey (penalty))
			return 0.0f;
		return GameStartData.penalty_values[penalty];
	}
	public bool is_under_ripe(){
		return quality < GameStartData.min_accepted_ripeness;
	}
	public bool is_over_ripe(){
		return quality > GameStartData.max_accepted_ripeness;
	}
	public bool is_under_size(){
		return weight < GameStartData.min_berry_weight;
	}
	public DragHandle drag;
	public Automata automata;
	public ObjectVisibility visibility;

	// Use this for initialization
	void Start () {
		StrawberryStateMachine berry_state = SingletonBehavior.get_instance<StrawberryStateMachine> ();
		automata = gameObject.GetComponent<Automata> ();
		visibility = gameObject.GetComponent<ObjectVisibility>();
		Initialize();
		drag = gameObject.GetComponent<DragHandle>();
		drag.register_incoming (berry_state.fsm.transition("field_drag"))
			.register_incoming (berry_state.fsm.transition("fall_drag"))
			.register_incoming (berry_state.fsm.transition("hand_drag"))
			.register_state (berry_state.fsm.state("drag"))
			.register_outgoing(berry_state.fsm.transition("drag_fall"));
		foreach(BasketComponent basket in BasketComponent.baskets){
			drag.register_incoming(basket.remove);
		}
	}

	public void Initialize(){
		visibility.visible = false;
		quality = RandomUtils.random_float(GameStartData.min_ripeness, GameStartData.max_ripeness);
		StrawberryScale scale = gameObject.GetComponent<StrawberryScale>();
		if (scale != null){
			scale.Initialize();
		}
	}
}
