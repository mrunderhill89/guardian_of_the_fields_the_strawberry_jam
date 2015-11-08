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

	public DragHandle drag = null;
	public StrawberryStateMachine berry_state;
	public Automata automata;
	public ObjectVisibility visibility;
	new protected Renderer renderer;

	// Use this for initialization
	void Start () {
		berry_state = SingletonBehavior.get_instance<StrawberryStateMachine> ();
		automata = gameObject.GetComponent<Automata> ();
		visibility = gameObject.GetComponent<ObjectVisibility>();
		renderer = transform.Find("Strawberry_Mesh/Cube").GetComponent<Renderer>();
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
