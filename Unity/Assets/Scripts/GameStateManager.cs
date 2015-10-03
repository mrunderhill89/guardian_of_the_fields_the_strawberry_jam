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
	public StateMachine player_state;
	Action lazy_log(string message){
		return () => {
			Debug.Log (message);
		};
	}
	new void Awake(){
		base.Awake();
		player_state = gameObject.AddComponent<StateMachine> ();
	}
	void Start () {
		Physics.gravity = gravity;
		//Set up States
		player_state.state ("root")
		.add_child (
			player_state.state("loading"),true
		).add_child (
			player_state.state ("look")
				.on_update(new StateEvent(cart_control.move))
				.add_child(player_state.state("look_forward")
		           .on_entry(new StateEvent(camera_control.lazy_set_target("look_forward"))),true
		        ).add_child(player_state.state("look_left")
		            .on_entry(new StateEvent(camera_control.lazy_set_target("look_left")))
		        ).add_child(player_state.state("look_right")
					.on_entry(new StateEvent(camera_control.lazy_set_target("look_right")))
			    )
		).add_child (
			player_state.state ("pick")
			.add_child(player_state.state("pick_left")
	    		.on_entry(new StateEvent(camera_control.lazy_set_target("pick_left")))
	        ).add_child(player_state.state("pick_right")
		        .on_entry(new StateEvent(camera_control.lazy_set_target("pick_right")))
	        )
		).add_child (
			player_state.state ("pack")
				.on_entry(new StateEvent(camera_control.lazy_set_target("pack")))
		);
		//Set up Transitions
		//Loading -> Look
		player_state.new_transition("loading=>look", (t)=>{
			t.from(player_state.state("loading"))
			.to (player_state.state("look"))
	  		.auto_run(true)
			.add_test(new TransitionTest(()=>{
						StrawberryStateMachine sb_machine = SingletonBehavior.get_instance<StrawberryStateMachine>();
						if (sb_machine != null){
							return sb_machine.finished_loading();
						}
						return false;
			}));
		//Look Forward -> Look Left, Look Right, Pack
		}).new_transition("look_forward=>left", (t)=>{
			input.register_transition(t,"left")
				.from(player_state.state("look_forward"))
					.to(player_state.state("look_left"));
		}).new_transition("look_forward=>right", (t)=>{
			input.register_transition(t,"right")
				.from(player_state.state("look_forward"))
					.to(player_state.state("look_right"));
		}).new_transition("look_forward=>pack", (t)=>{
			input.register_transition(t,new string[]{"up","down"})
				.from(player_state.state("look_forward"))
					.to(player_state.state("pack"));
		//Look Left -> Look Forward, Pick Left
		}).new_transition("look_left=>pick", (t)=>{
			input.register_transition(t,"down")
				.from(player_state.state("look_left"))
					.to(player_state.state("pick_left"));
		}).new_transition("look_left=>forward", (t)=>{
			input.register_transition(t,"right")
				.from(player_state.state("look_left"))
					.to(player_state.state("look_forward"));
		//Look Right -> Look Forward, Pick Right
		}).new_transition("look_right=>pick", (t)=>{
			input.register_transition(t,"down")
				.from(player_state.state("look_right"))
					.to(player_state.state("pick_right"));
		}).new_transition("look_right=>forward", (t)=>{
			input.register_transition(t,"left")
				.from(player_state.state("look_right"))
					.to(player_state.state("look_forward"));	
		//Pick Left -> Look Left, Pack
		}).new_transition("pick_left=>look_left", (t)=>{
			input.register_transition(t,"up")
				.from(player_state.state("pick_left"))
					.to(player_state.state("look_left"));
		}).new_transition("pick_left=>pack", (t)=>{
			input.register_transition(t,"right")
				.from(player_state.state("pick_left"))
					.to(player_state.state("pack"));
		//Pick Right -> Look Right, Pack
		}).new_transition("pick_right=>look_right", (t)=>{
			input.register_transition(t,"up")
				.from(player_state.state("pick_right"))
					.to(player_state.state("look_right"));
		}).new_transition("pick_right=>pack", (t)=>{
			input.register_transition(t,"left")
				.from(player_state.state("pick_right"))
					.to(player_state.state("pack"));
		// Pack -> Look Forward, Pick Left, Pick Right
		}).new_transition("pack=>look_forward", (t)=>{
			input.register_transition(t,new string[]{"up","down"})
				.from(player_state.state("pack"))
					.to(player_state.state("look_forward"));
		}).new_transition("pack=>pick_left", (t)=>{
			input.register_transition(t,"left")
				.from(player_state.state("pack"))
					.to(player_state.state("pick_left"));
		}).new_transition("pack=>pick_right", (t)=>{
			input.register_transition(t,"right")
				.from(player_state.state("pack"))
					.to(player_state.state("pick_right"));
		});
		//Now add a player automata.
		player_state.new_automata ("player", (a) => {
			a.move_direct(player_state.state("root"));
		});
	}
	public bool can_pick_anywhere(){
		return player_state.state ("pick").is_visited ();
	}
	public bool basket_physics_enabled(){
		return player_state.state("pack").is_visited();
	}
}
