using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class StrawberryStateMachine : SingletonBehavior {
	public StateMachine fsm;
	public int field_strawberries = 100;
	public GameStateManager player_state;
	new void Awake () {
		base.Awake();
		fsm = gameObject.AddComponent<StateMachine>();
		fsm.state ("root")
			.add_child (
				fsm.state ("field")
				.on_descent(new StateEvent((Automata a)=>{
					StrawberryComponent sb = a.gameObject.GetComponent<StrawberryComponent>();
					sb.Initialize();
				}))
				.on_exit(new StateEvent(()=>{
					GenerateStrawberries(1);
				}))
				.initial(()=>{
					State state = StrawberryRowState.random_row();
					return state.parent(fsm.state("field"));
				}), true
			).add_child (
				fsm.state ("drag")
			).add_child (
				fsm.state ("fall")
				.on_entry(new StateEvent((Automata a)=>{
					Rigidbody body = a.GetComponent<Rigidbody>();
					if (body != null){
						body.useGravity = true;
						body.isKinematic = false;
					}
				}))
			).add_child (
				fsm.state ("hand")
				.on_entry(new StateEvent((Automata a)=>{
					Rigidbody body = a.GetComponent<Rigidbody>();
					if (body != null){
						body.useGravity = false;
						body.isKinematic = true;
					}
				}))
			).add_child (
				fsm.state ("basket")
			);
		fsm.new_transition("field_drag", (t)=>{
			t.from(fsm.state("field"))
			.to (fsm.state("drag"))
			.auto_run(false)
			;
		}).new_transition("fall_drag", (t)=>{
			t.from(fsm.state("field"))
				.to (fsm.state("drag"))
					.auto_run(false)
					;
		}).new_transition("hand_drag", (t)=>{
			t.from(fsm.state("hand"))
				.to (fsm.state("drag"))
					.auto_run(false)
					;
		}).new_transition("basket_drag", (t)=>{
			t.from(fsm.state("basket"))
				.to (fsm.state("drag"))
					.auto_run(false)
					;
		}).new_transition("drag_fall", (t)=>{
			t.from(fsm.state("drag"))
				.to (fsm.state("fall"))
					.auto_run(false)
					;
		});

	}
	void Start(){
		player_state = SingletonBehavior.get_instance<GameStateManager> ();
		GenerateStrawberries(field_strawberries);
	}
	public bool finished_loading(){
		return fsm.state("field").is_visited() && fsm.state("field").visitors.ToList().Find((Automata a)=>{
			return a.is_travelling();
		}) == null;
	}
	public DragHandle register_drag_handle(DragHandle drag){
		drag.register_incoming (fsm.transition("field_drag"))
			.register_incoming (fsm.transition("fall_drag"))
			.register_incoming (fsm.transition("hand_drag"))
			.register_incoming (fsm.transition("basket_drag"))
			.register_state (fsm.state("drag"))
			.register_outgoing(fsm.transition("drag_fall"));
		return drag;
	}
	void GenerateStrawberries(int num){
		GameObject berry;
		for (int u = 0; u < num; u++){
			berry = GameObject.Instantiate(Resources.Load ("Strawberry")) as GameObject;
			string berry_name = berry.name+":"+(fsm.count_automata()+1).ToString();
			fsm.automata(berry_name, berry.GetComponent<Automata>())
				.move_direct(fsm.state("root"));
		}
	}
}
