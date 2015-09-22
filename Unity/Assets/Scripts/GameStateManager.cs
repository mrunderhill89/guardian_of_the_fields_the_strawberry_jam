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
	public Dictionary<string,State> states;
	public Dictionary<string,Transition> transitions;
	Action lazy_log(string message){
		return () => {
			Debug.Log (message);
		};
	}
	new void Awake(){
		base.Awake();
		states = new Dictionary<string,State> ();
		transitions = new Dictionary<string,Transition> ();
	}
	void Start () {
		Physics.gravity = gravity;
		//Look
		states["look_forward"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "look_forward")
			.on_entry(new StateEvent(camera_control.lazy_set_target("look_forward")));
		states["look_left"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "look_left")
			.on_entry(new StateEvent(camera_control.lazy_set_target("look_left")));
		states["look_right"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "look_right")
			.on_entry(new StateEvent(camera_control.lazy_set_target("look_right")));
		states["look"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "look")
			.add_child(states["look_forward"], true)
			.add_child(states["look_left"])
			.add_child(states["look_right"])
			.on_entry(new StateEvent(lazy_log("Looking")))
			.on_update(new StateEvent(()=>{
					cart_control.move();
			}));

		//Pick
		states["pick_left"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "pick_left")
			.on_entry(new StateEvent(camera_control.lazy_set_target("pick_left", 20, 20)));
		states["pick_right"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "pick_right")
			.on_entry(new StateEvent(camera_control.lazy_set_target("pick_right", 20, 20)));
		states["pick"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "pick")
			.add_child(states["pick_left"])
			.add_child(states["pick_right"]);

		//Pack
		states["pack"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "pack")
			.on_entry(new StateEvent(()=>{
				camera_control.lazy_set_target("pack")();
				//Draggable.calculate_delta = Draggable.xz_plane;
			})).on_exit(new StateEvent(()=>{
				//Draggable.calculate_delta = Draggable.xy_plane;
			}));

		//Main State
		states["root"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "root")
			.add_child(states["look"], true).add_child(states["pick"]).add_child(states["pack"]);

		//Transitions
		states["look_forward"]
			.add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_forward=>left")
				.to(states["look_left"])
				.register_event(input.on_dir("left"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_forward=>right")
				.to(states["look_right"])
				.register_event(input.on_dir("right"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_forward=>pack")
				.to(states["pack"])
				.register_event(input.on_dir("up"))
				.register_event(input.on_dir("down"))
			);
		//Look Left -> Look Forward, Pick Left
		states["look_left"].add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_left=>forward")
				.to(states["look_forward"])
				.register_event(input.on_dir("right"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_left=>pick")
				.to(states["pick_left"])
				.register_event(input.on_dir("down"))
			);
		//Look Right -> Look Forward, Pick Right
		states["look_right"].add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_right=>forward")
				.to(states["look_forward"])
				.register_event(input.on_dir("left"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "look_right=>pick")
				.to(states["pick_right"])
				.register_event(input.on_dir("down"))
			);
		//Pick Left -> Look Left, Pack
		states["pick_left"].add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pick_left=>look")
				.to(states["look_left"])
				.register_event(input.on_dir("up"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pick_left=>pack")
				.to(states["pack"])
				.register_event(input.on_dir("right"))
			);
		//Pick Right -> Look Right, Pack
		states["pick_right"].add_transition (
			NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pick_right=>look")
				.to(states["look_right"])
				.register_event(input.on_dir("up"))
			).add_transition (
				NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pick_right=>pack")
				.to(states["pack"])
				.register_event(input.on_dir("left"))
			);
		// Pack -> Look Forward, Pick Left, Pick Right
		states["pack"].add_transition (
			NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pack=>look_forward")
			.to(states["look_forward"])
			.register_event(input.on_dir("up"))
			.register_event(input.on_dir("down"))
		).add_transition (
			NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pack=>pick_left")
			.to(states["pick_left"])
			.register_event(input.on_dir("left"))
		).add_transition (
			NamedBehavior.GetOrCreateComponentByName<Transition>(gameObject, "pack=>pick_right")
			.to(states["pick_right"])
			.register_event(input.on_dir("right"))
		);
	}
}
