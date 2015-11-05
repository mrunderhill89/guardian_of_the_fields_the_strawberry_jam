using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
public class GameStateManager : SingletonBehavior {
	public InputController input;
	public CameraController camera_control;
	public PaceManager cart_control;
	public Vector3 gravity;
	public StateMachine fsm;
	public List<string> drag_states = new List<string>();
	Action lazy_log(string message){
		return () => {
			Debug.Log (message);
		};
	}
	new void Awake(){
		base.Awake();
		fsm = gameObject.AddComponent<StateMachine> ();
	}
	void Start () {
		Physics.gravity = gravity;
		//Set up States
		fsm.state ("root")
		.add_child (
			fsm.state("loading"),true
		).add_child (
			fsm.state ("look")
				.add_child(fsm.state("look_forward")
					.on_entry(new StateEvent(camera_control.lazy_set_target("look_forward"))),true
				).add_child(fsm.state("look_left")
					.on_entry(new StateEvent(camera_control.lazy_set_target("look_left")))
				).add_child(fsm.state("look_right")
					.on_entry(new StateEvent(camera_control.lazy_set_target("look_right")))
				).add_child(fsm.state("look_behind")
					.on_entry(new StateEvent(camera_control.lazy_set_target("look_behind")))
			    )
		).add_child (
			fsm.state ("pick")
			.add_child(fsm.state("pick_left")
	    		.on_entry(new StateEvent(camera_control.lazy_set_target("pick_left")))
	        ).add_child(fsm.state("pick_right")
		        .on_entry(new StateEvent(camera_control.lazy_set_target("pick_right")))
	        ).add_child(fsm.state("pick_behind")
		        .on_entry(new StateEvent(camera_control.lazy_set_target("pick_behind")))
	        )
		).add_child (
			fsm.state ("pack")
				.on_entry(new StateEvent(camera_control.lazy_set_target("pack")))
		).add_child (
			fsm.state ("game_end")
				.add_child(
					fsm.state("remove_ineligible_berries")
					.on_entry(new StateEvent(()=>{
						camera_control.set_target("pack");
						int under_ripe = 0, over_ripe = 0, under_size = 0;
						bool ineligible;
						foreach (StrawberryComponent berry in BasketComponent.get_all_strawberries().ToList()){
							ineligible = false;
							//Underripe
							if (berry.quality < GameStartData.min_accepted_ripeness){
								under_ripe++;
								ineligible = true;
							}
							//Overripe
							if (berry.quality > GameStartData.max_accepted_ripeness){
								over_ripe++;
								ineligible = true;
							}
							//Undersize
							if (berry.weight < GameStartData.min_berry_weight){
								under_size++;
								ineligible = true;
							}
							if (ineligible){
								Destroy(berry.gameObject);
							}
						}
						GameMessages.Log("Underripe Berries:"+under_ripe);
						GameMessages.Log("Overripe Berries:"+over_ripe);
						GameMessages.Log("Undersized Berries:"+under_size);
					}))
					,true
				).add_child(
					fsm.state("weigh_baskets")
				).add_child(
					fsm.state("second_chance")
				).add_child(
					fsm.state("final_tally")
				)
		);
		//Set up Transitions
		//Loading -> Look
		fsm.new_transition("loading=>look", (t)=>{
			t.chain_from(fsm.state("loading"))
			.chain_to (fsm.state("look"))
	  		.chain_auto_run(true)
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
				.chain_from(fsm.state("look_forward"))
					.chain_to(fsm.state("look_left"));
		}).new_transition("look_forward=>right", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("look_forward"))
					.chain_to(fsm.state("look_right"));
		}).new_transition("look_forward=>pack", (t)=>{
			input.register_transition(t,new string[]{"up","down"})
				.chain_from(fsm.state("look_forward"))
					.chain_to(fsm.state("pack"));
		//Look Left -> Look Forward, Look Behind, Pick Left
		}).new_transition("look_left=>pick", (t)=>{
			input.register_transition(t,"down")
				.chain_from(fsm.state("look_left"))
					.chain_to(fsm.state("pick_left"));
		}).new_transition("look_left=>forward", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("look_left"))
					.chain_to(fsm.state("look_forward"));
		}).new_transition("look_left=>behind", (t)=>{
			input.register_transition(t,"left")
				.chain_from(fsm.state("look_left"))
					.chain_to(fsm.state("look_behind"));
		//Look Right -> Look Forward, Look Behind, Pick Right
		}).new_transition("look_right=>pick", (t)=>{
			input.register_transition(t,"down")
				.chain_from(fsm.state("look_right"))
					.chain_to(fsm.state("pick_right"));
		}).new_transition("look_right=>forward", (t)=>{
			input.register_transition(t,"left")
				.chain_from(fsm.state("look_right"))
					.chain_to(fsm.state("look_forward"));
		}).new_transition("look_right=>behind", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("look_right"))
					.chain_to(fsm.state("look_behind"));
		//Look Behind -> Look Left, Look Right, Pick Behind
		}).new_transition("look_behind=>left", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("look_behind"))
					.chain_to(fsm.state("look_left"));
		}).new_transition("look_behind=>right", (t)=>{
			input.register_transition(t,"left")
				.chain_from(fsm.state("look_behind"))
					.chain_to(fsm.state("look_right"));
		}).new_transition("look_behind=>pick", (t)=>{
			input.register_transition(t,"down")
				.chain_from(fsm.state("look_behind"))
					.chain_to(fsm.state("pick_behind"));
		//Pick Left -> Look Left, Pick Behind, Pack
		}).new_transition("pick_left=>look_left", (t)=>{
			input.register_transition(t,"up")
				.chain_from(fsm.state("pick_left"))
					.chain_to(fsm.state("look_left"));
		}).new_transition("pick_left=>behind", (t)=>{
			input.register_transition(t,"left")
				.chain_from(fsm.state("pick_left"))
					.chain_to(fsm.state("pick_behind"));
		}).new_transition("pick_left=>pack", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("pick_left"))
					.chain_to(fsm.state("pack"));
		//Pick Right -> Look Right, Pick Behind, Pack
		}).new_transition("pick_right=>look_right", (t)=>{
			input.register_transition(t,"up")
				.chain_from(fsm.state("pick_right"))
					.chain_to(fsm.state("look_right"));
		}).new_transition("pick_right=>behind", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("pick_right"))
					.chain_to(fsm.state("pick_behind"));
		}).new_transition("pick_right=>pack", (t)=>{
			input.register_transition(t,"left")
				.chain_from(fsm.state("pick_right"))
					.chain_to(fsm.state("pack"));
		//Pick Behind -> Look Behind, Pick Left, Pick Right
		}).new_transition("pick_behind=>look", (t)=>{
			input.register_transition(t,"up")
				.chain_from(fsm.state("pick_behind"))
					.chain_to(fsm.state("look_behind"));
		}).new_transition("pick_behind=>right", (t)=>{
			input.register_transition(t,"left")
				.chain_from(fsm.state("pick_behind"))
					.chain_to(fsm.state("pick_right"));
		}).new_transition("pick_behind=>left", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("pick_behind"))
					.chain_to(fsm.state("pick_left"));
		// Pack -> Look Forward, Pick Left, Pick Right
		}).new_transition("pack=>look_forward", (t)=>{
			input.register_transition(t,new string[]{"up","down"})
				.chain_from(fsm.state("pack"))
					.chain_to(fsm.state("look_forward"));
		}).new_transition("pack=>pick_left", (t)=>{
			input.register_transition(t,"left")
				.chain_from(fsm.state("pack"))
					.chain_to(fsm.state("pick_left"));
		}).new_transition("pack=>pick_right", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("pack"))
					.chain_to(fsm.state("pick_right"));
		//Endgame Transitions
		}).new_transition("time_up", (t)=>{
			t.chain_from(fsm.state("root"))
			.chain_to(fsm.state("game_end"));
		});
		//Now add a player automata.
		fsm.new_automata ("player", (a) => {
			a.move_direct(fsm.state("root"));
		});
	}
	public State state(string name){
		return fsm.state(name);
	}
	public bool is_loading(){
		return fsm.state("loading").is_visited();
	}
	public bool can_pick(){
		return fsm.state ("pick").is_visited();
	}
	public bool can_drag(){
		return fsm.match(drag_states, true, false);
	}
	public bool basket_physics_enabled(){
		return fsm.state("pack").is_visited();
	}
}
