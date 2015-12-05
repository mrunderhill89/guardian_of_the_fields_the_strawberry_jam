using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
public class GameStateManager : BetterBehaviour {
	protected static GameStateManager _main = null;
	public static GameStateManager main{
		get{ return _main;}
		private set{ _main = value;}
	}

	public InputController input;
	public CameraController camera_control;
	public PaceManager cart_control;
	public Vector3 gravity;
	public StateMachine fsm;
	public ScoreHandler scores;
	public GameTimer timer;
	public List<string> drag_states = new List<string>();

	Action lazy_log(string message){
		return () => {
			Debug.Log (message);
		};
	}

	void Awake(){
		main = this;
		if (fsm == null)
			fsm = gameObject.AddComponent<StateMachine> ();
		if (scores == null)
			scores = GetComponent<ScoreHandler>();
	}
	void Start () {
		Physics.gravity = gravity;
		StrawberryStateMachine berry_state = StrawberryStateMachine.main;
		//Set up States
		fsm.state ("root")
		.add_child( 
           fsm.state ("gameplay")
			.add_child (
				fsm.state("loading")
				.add_child(fsm.state("load_forward")
					.on_entry(new StateEvent(camera_control.lazy_set_target("look_forward"))), true
				).add_child(fsm.state("load_left")
					.on_entry(new StateEvent(camera_control.lazy_set_target("look_left")))
				).add_child(fsm.state("load_right")
					.on_entry(new StateEvent(camera_control.lazy_set_target("look_right")))
				).add_child(fsm.state("load_behind")
					.on_entry(new StateEvent(camera_control.lazy_set_target("look_behind")))
				).chain_initial(fsm.state("load_forward")
			    ).on_entry( new StateEvent(()=>{Application.runInBackground = true;})
				).on_exit(new StateEvent(()=>{
					Application.runInBackground = false;
					timer.add_countdown(GameStartData.instance.game_length, (t)=>{
						fsm.transition("time_up").trigger();
					});
					if (GameStartData.instance.tutorial){
						timer.add_countdown(GameStartData.instance.game_length-60.0f, (t)=>{
							GameMessages.Log(LanguageTable.get("tutorial_time_up"), 10.0f);
						});
						timer.add_countdown(GameStartData.instance.game_length/2.0f, (t)=>{
							GameMessages.Log(LanguageTable.get("tutorial_midday"), 10.0f);
						});
						GameMessages.Log(LanguageTable.get("tutorial_move"), 10.0f);
					}
					timer.started = true;
				})),true
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
				.on_entry(new StateEvent(()=>{
					if (GameStartData.instance.tutorial){
						GameMessages.Log(LanguageTable.get("tutorial_pick"), 10.0f);
					}
				}).Limit(1)
				).add_child(fsm.state("pick_left")
		    		.on_entry(new StateEvent(camera_control.lazy_set_target("pick_left")))
		        ).add_child(fsm.state("pick_right")
			        .on_entry(new StateEvent(camera_control.lazy_set_target("pick_right")))
		        ).add_child(fsm.state("pick_behind")
			        .on_entry(new StateEvent(camera_control.lazy_set_target("pick_behind")))
		        )
			).add_child (
				fsm.state ("pack")
					.on_entry(new StateEvent(camera_control.lazy_set_target("pack")))
					.on_entry(new StateEvent(()=>{
							if (GameStartData.instance.tutorial){
								GameMessages.Log(LanguageTable.get("tutorial_pack"), 10.0f);
							}
						}).Limit(1)
					)
			), true
		).add_child (
			fsm.state ("game_end")
				.add_child(
					fsm.state("weigh_baskets")
					.on_entry(new StateEvent(()=>{
						timer.started = false;
						camera_control.set_target("pack");
						//Clear all remaining strawberries in the field.
						scores.lock_strawberries = true;
						foreach (StrawberryComponent berry in berry_state.get_strawberries("field")){
							Destroy (berry.gameObject);
						}
						foreach (StrawberryComponent berry in berry_state.get_strawberries("fall")){
							Destroy (berry.gameObject);
						}
						//Check strawberries that the player has gathered
						foreach (StrawberryComponent berry in BasketComponent.get_all_strawberries().ToList()){
							if (berry.is_ineligible()){
								DestroyObject(berry.gameObject);
							}
						}
					}))
					,true
				).add_child(
					fsm.state("second_chance")
					.on_entry(new StateEvent(()=>{
					//Create a time that will go off in X minutes.
					//That timer will trigger the transition to the next state.
					}))
				).add_child(
				fsm.state("final_tally").on_entry(new StateEvent(()=>{
						//Lock all the baskets so that dragging isn't possible anymore.
						foreach (BasketComponent basket in BasketComponent.baskets){
							basket.locked = true;
						}
						scores.lock_baskets=true;
						scores.record_score();
						scores.save_scores();
					}))
				).add_child(
				fsm.state("return_to_menu").on_entry(new StateEvent(()=>{
					Application.LoadLevel("title_screen");
				}))
			)
		);
		//Set up Transitions
		//Loading -> Look
		fsm.new_transition("loading=>look", (t)=>{
			t.chain_from(fsm.state("loading"))
			.chain_to (fsm.state("look"))
	  		.chain_auto_run(false)
			.add_test(new TransitionTest(()=>{
				return berry_state.finished_loading() && LeafStateSystem.instance.finished_loading;
			}));
		//Loading Look Forward -> Look Left, Look Right
		}).new_transition("load_forward=>left", (t)=>{
			input.register_transition(t,"left")
				.chain_from(fsm.state("load_forward"))
					.chain_to(fsm.state("load_left"));
		}).new_transition("load_forward=>right", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("load_forward"))
					.chain_to(fsm.state("load_right"));
		//Loading Look Left -> Look Behind, Look Forward
		}).new_transition("load_left=>behind", (t)=>{
			input.register_transition(t,"left")
				.chain_from(fsm.state("load_left"))
					.chain_to(fsm.state("load_behind"));
		}).new_transition("load_left=>forward", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("load_left"))
					.chain_to(fsm.state("load_forward"));
		//Loading Look Right -> Look Behind, Look Forward
		}).new_transition("load_right=>forward", (t)=>{
			input.register_transition(t,"left")
				.chain_from(fsm.state("load_right"))
					.chain_to(fsm.state("load_forward"));
		}).new_transition("load_right=>behind", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("load_right"))
					.chain_to(fsm.state("load_behind"));
		//Loading Look Behind -> Look Left, Look Right
		}).new_transition("load_behind=>left", (t)=>{
			input.register_transition(t,"right")
				.chain_from(fsm.state("load_behind"))
					.chain_to(fsm.state("load_left"));
		}).new_transition("load_behind=>right", (t)=>{
			input.register_transition(t,"left")
				.chain_from(fsm.state("load_behind"))
					.chain_to(fsm.state("load_right"));
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
			t.chain_from(fsm.state("gameplay"))
			.chain_to(fsm.state("game_end"));
		}).new_transition("weight=>2nd_chance", (t)=>{
			t.chain_from(fsm.state("weigh_baskets"))
				.chain_to(fsm.state ("second_chance"));
		}).new_transition("2nd chance=>final", (t)=>{
			t.chain_from(fsm.state("second_chance"))
				.chain_to(fsm.state ("final_tally"));
		}).new_transition("final=>menu", (t)=>{
			t.chain_from(fsm.state("final_tally"))
				.chain_to(fsm.state ("return_to_menu"));
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
		return fsm.is_state_visited("loading");
	}
	public bool can_pick(){
		return fsm.is_state_visited("pick");
	}
	public bool can_drag(){
		return fsm.match(drag_states, true, false);
	}
	public bool basket_physics_enabled(){
		return fsm.is_state_visited("pack");
	}
}
