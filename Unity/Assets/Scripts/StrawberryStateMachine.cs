using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
				.on_entry(new StateEvent((Automata a)=>{
					StrawberryComponent sb = a.gameObject.GetComponent<StrawberryComponent>();
					sb.Initialize();
				}))
				.initial(()=>{
					return StrawberryRowState.random_row();
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
		return true;
		/*
		int total = fsm.state ("field").count ();
		int visible = fsm.state ("visible").count ();
		return visible > 0 && visible == total;
		*/
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
	void Update () {
		//GenerateStrawberries(field_strawberries - states["init"].count() - states["field"].count());
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
