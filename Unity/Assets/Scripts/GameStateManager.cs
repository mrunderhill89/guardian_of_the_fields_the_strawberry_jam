using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameStateManager : SingletonBehavior {
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
		//Look
		State look_forward = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "look_forward");
			look_forward.on_entry(new StateEvent(camera_control.lazy_set_target("look_forward")));
		State look_left = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "look_left");
			look_left.on_entry(new StateEvent(camera_control.lazy_set_target("look_left")));
		State look_right = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "look_right");
			look_right.on_entry(new StateEvent(camera_control.lazy_set_target("look_right")));
		State look = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "look");
			look.add_child(look_forward, true)
			.add_child(look_left)
			.add_child(look_right)
			.on_entry(new StateEvent(lazy_log("Looking")))
			.on_update(new StateEvent(()=>{
					cart_control.move();
			}));

		//Pick
		State pick_left = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "pick_left");
			pick_left.on_entry(new StateEvent(camera_control.lazy_set_target("pick_left", 20, 20)));
		State pick_right = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "pick_right");
			pick_right.on_entry(new StateEvent(camera_control.lazy_set_target("pick_right", 20, 20)));
		State pick = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "pick");
			pick.add_child(pick_left)
			.add_child(pick_right)
				.on_entry(new StateEvent(()=>{
				StrawberryComponent.allow_picking_unpicked = true;
				})).on_exit(new StateEvent(()=>{
					StrawberryComponent.allow_picking_unpicked = false;
				}));

		//Pack
		State pack = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "pack");
			pack.on_entry(new StateEvent(()=>{
				camera_control.lazy_set_target("pack")();
				//Draggable.calculate_delta = Draggable.xz_plane;
			})).on_exit(new StateEvent(()=>{
				//Draggable.calculate_delta = Draggable.xy_plane;
			}));

		//Main State
		State root = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "root");
		root.add_child(look, true).add_child(pick).add_child(pack);

		//Transitions
		look_forward
			.add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_forward=>left")
				.to(look_left)
				.register_event(input.on_dir("left"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_forward=>right")
				.to(look_right)
				.register_event(input.on_dir("right"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_forward=>pack")
				.to(pack)
				.register_event(input.on_dir("down"))
			);
		//Look Left -> Look Forward, Pick Left
		look_left.add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_left=>forward")
				.to(look_forward)
				.register_event(input.on_dir("right"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_left=>pick")
				.to(pick_left)
				.register_event(input.on_dir("down"))
			);
		//Look Right -> Look Forward, Pick Right
		look_right.add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_right=>forward")
				.to(look_forward)
				.register_event(input.on_dir("left"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_right=>pick")
				.to(pick_right)
				.register_event(input.on_dir("down"))
			);
		//Pick Left -> Look Left, Pack
		pick_left.add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pick_left=>look")
				.to(look_left)
				.register_event(input.on_dir("up"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pick_left=>pack")
				.to(pack)
				.register_event(input.on_dir("right"))
			);
		//Pick Right -> Look Right, Pack
		pick_right.add_transition (
			NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pick_right=>look")
				.to(look_right)
				.register_event(input.on_dir("up"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pick_right=>pack")
				.to(pack)
				.register_event(input.on_dir("left"))
			);
		// Pack -> Look Forward, Pick Left, Pick Right
		pack.add_transition (
			NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pack=>look_forward")
			.to(look_forward)
			.register_event(input.on_dir("up"))
		).add_transition (
			NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pack=>pick_left")
			.to(pick_left)
			.register_event(input.on_dir("left"))
		).add_transition (
			NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pack=>pick_right")
			.to(pick_right)
			.register_event(input.on_dir("right"))
		);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
