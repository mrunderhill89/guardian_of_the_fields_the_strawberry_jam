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
		//Loading
		states["loading"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "loading");

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
			.on_entry(new StateEvent(camera_control.lazy_set_target("pick_left")));
		states["pick_right"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "pick_right")
			.on_entry(new StateEvent(camera_control.lazy_set_target("pick_right")));
		states["pick"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "pick")
			.add_child(states["pick_left"])
			.add_child(states["pick_right"]);

		//Pack
		states["pack"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "pack")
			.on_entry(new StateEvent(()=>{
				camera_control.lazy_set_target("pack")();
			}));

		//Main State
		states["root"] = NamedBehavior.GetOrCreateComponentByName<State> (gameObject, "root")
			.add_child (states["loading"], true)
			.add_child(states["look"]).add_child(states["pick"]).add_child(states["pack"]);

		//Transitions
		//Loading -> Look
		transitions ["loading=>look"] = NamedBehavior.GetOrCreateComponentByName<Transition> (gameObject, "loading=>look")
			.from (states ["loading"])
			.to (states["look"])
			.auto_run(true)
			.add_test(new TransitionTest(()=>{
					StrawberryStateMachine sb_machine = SingletonBehavior.get_instance<StrawberryStateMachine>();
					if (sb_machine != null){
						return sb_machine.finished_loading();
					}
					return false;
			}));
		//Look Forward -> Look Left, Look Right, Pack
		transitions["look_forward=>left"] = input.register_transition(gameObject, "look_forward=>left", "left")
			.from(states["look_forward"])
			.to(states["look_left"]);
		transitions["look_forward=>right"] = input.register_transition(gameObject, "look_forward=>right", "right")
			.from(states["look_forward"])
			.to(states["look_right"]);
		transitions["look_forward=>pack"] = input.register_transition(gameObject, "look_forward=>pack", 
		                                                              new string[]{"up","down"})
			.from(states["look_forward"])
			.to(states["pack"]);
		//Look Left -> Look Forward, Pick Left
		transitions["look_left=>forward"] = input.register_transition(gameObject, "look_left=>forward", "right")
			.from(states["look_left"])
			.to(states["look_forward"]);
		transitions["look_left=>pick"] = input.register_transition(gameObject, "look_left=>pick", "down")
			.from(states["look_left"])
			.to(states["pick_left"]);
		//Look Right -> Look Forward, Pick Right
		transitions["look_right=>forward"] = input.register_transition(gameObject, "look_right=>forward", "left")
			.from(states["look_right"])
			.to(states["look_forward"]);
		transitions["look_right=>pick"] = input.register_transition(gameObject, "look_right=>pick", "down")
			.from(states["look_right"])
			.to(states["pick_right"]);
		//Pick Left -> Look Left, Pack
		transitions["pick_left=>look"] = input.register_transition(gameObject, "pick_left=>look", "up")
			.from(states["pick_left"])
			.to(states["look_left"]);
		transitions["pick_left=>pack"] = input.register_transition(gameObject, "pick_left=>pack", "right")
			.from(states["pick_left"])
			.to(states["pack"]);
		//Pick Right -> Look Right, Pack
		transitions["pick_right=>look"] = input.register_transition(gameObject, "pick_right=>look", "up")
			.from(states["pick_right"])
			.to(states["look_right"]);
		transitions["pick_right=>pack"] = input.register_transition(gameObject, "pick_right=>pack", "left")
			.from(states["pick_right"])
			.to(states["pack"]);
		// Pack -> Look Forward, Pick Left, Pick Right
		transitions["pack=>look_forward"] = input.register_transition(gameObject, "pack=>look_forward", new string[] {"up","down"})
			.from(states["pack"])
			.to(states["look_forward"]);
		transitions ["pack=>pick_left"] = input.register_transition(gameObject, "pack=>pick_left", "left")
			.from (states ["pack"])
			.to (states ["pick_left"]);
		transitions["pack=>pick_right"] = input.register_transition(gameObject, "pack=>pick_right", "right")
			.from(states["pack"])
			.to(states["pick_right"]);
	}
}
