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
		get{return GameSettingsComponent.working_rules.strawberry.density * (transform.localScale.x
			*transform.localScale.y
			*transform.localScale.z)
			;}
	}
	public bool is_under_ripe(){
		return GameSettingsComponent.working_rules.win_condition.ripeness.is_under_accept(quality);
	}
	public bool is_over_ripe(){
		return GameSettingsComponent.working_rules.win_condition.ripeness.is_over_accept(quality);
	}
	public bool is_under_size(){
		return GameSettingsComponent.working_rules.win_condition.berry_size.is_under_accept(quality);
	}
	public bool is_ineligible(){
		return !GameSettingsComponent.working_rules.win_condition.ripeness.is_accept(quality) || 
			!GameSettingsComponent.working_rules.win_condition.berry_size.is_accept(weight);
	}
	
	public DragHandle drag;
	public Automata automata;
	public ObjectVisibility visibility;

	// Use this for initialization
	void Start () {
		StrawberryStateMachine berry_state = StrawberryStateMachine.main;
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
		quality = RandomUtils.random_float(
			GameSettingsComponent.working_rules.strawberry.min_ripeness, 
			GameSettingsComponent.working_rules.strawberry.max_ripeness, "level_generation");
		StrawberryScale scale = gameObject.GetComponent<StrawberryScale>();
		if (scale != null){
			scale.Initialize();
		}
	}
}
