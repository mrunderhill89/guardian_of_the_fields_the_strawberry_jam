using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameStateManager : BetterBehaviour {
	[NonSerialized]
	public StateComponent root;
	[NonSerialized]
	public InputController input;
	public CameraController camera_control;
	public CartController cart_control;
	public Vector3 gravity;
	Action lazy_log(string message){
		return () => {
			Debug.Log (message);
		};
	}
	void Start () {
		Physics.gravity = gravity;
		input = InputController.get_instance ();
		//Look
		StateComponent look_forward = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject, "look_forward")
			.on_entry(camera_control.lazy_set_target("look_forward"));
		StateComponent look_left = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject, "look_left")
			.on_entry(camera_control.lazy_set_target("look_left"));
		StateComponent look_right = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject, "look_right")
			.on_entry(camera_control.lazy_set_target("look_right"));
		StateComponent look = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject, "look")
			.add_child(look_forward, true)
			.add_child(look_left)
			.add_child(look_right)
			.on_entry(lazy_log("Looking"))
			.on_update(()=>{
					cart_control.move();
			});

		//Pick
		StateComponent pick_left = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject, "pick_left")
			.on_entry(camera_control.lazy_set_target("pick_left", 20, 20));
		StateComponent pick_right = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject, "pick_right")
			.on_entry(camera_control.lazy_set_target("pick_right", 20, 20));
		StateComponent pick = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject, "pick")
			.add_child(pick_left)
			.add_child(pick_right)
				.on_entry(()=>{
				StrawberryComponent.allow_picking_unpicked = true;
				}).on_exit(()=>{
					StrawberryComponent.allow_picking_unpicked = false;
				});

		//Pack
		StateComponent pack = NamedBehavior.GetOrCreateComponentByName<StateComponent>(gameObject, "pack")
			.on_entry(()=>{
				camera_control.lazy_set_target("pack")();
				//Draggable.calculate_delta = Draggable.xz_plane;
			}).on_exit(()=>{
				//Draggable.calculate_delta = Draggable.xy_plane;
			});

		//Transitions
		//Look Forward -> Look Left, Look Right, Pack
		look_forward.add_transition ("look_forward=>left", look_left, input.on_left)
			.add_transition ("look_forward=>right", look_right, input.on_right)
			.add_transition ("look_forward=>pack", pack, input.on_down);
		//Look Left -> Look Forward, Pick Left
		look_left.add_transition ("look_left=>forward", look_forward, input.on_right)
			.add_transition ("look_left=>pick", pick_left, input.on_down);
		//Look Right -> Look Forward, Pick Right
		look_right.add_transition ("look_right=>forward", look_forward, input.on_left)
		.add_transition ("look_right=>pick", pick_right, input.on_down);
		//Pick Left -> Look Left, Pack
		pick_left.add_transition("pick_left=>look", look_left, input.on_up)
		.add_transition("pick_left=>pack", pack, input.on_right);
		//Pick Right -> Look Left, Pack
		pick_right.add_transition("pick_right=>look", look_right, input.on_up)
		.add_transition("pick_right=>pack", pack, input.on_left);
		// Pack -> Look Forward
		pack.add_transition ("pack=>look_forward", look_forward, input.on_up);

		//Main State
		root.add_child(look, true).add_child(pick).add_child(pack);

	}
	
	// Update is called once per frame
	void Update () {
	}
}
