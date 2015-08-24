using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System;

public class GameStateManager : BetterBehaviour {
	[NonSerialized]
	public HFSM_State root;
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
		Func<bool> hands_full = () => {
			return false;
		};
		//Look
		HFSM_State look_forward = new HFSM_State ()
			.on_entry(camera_control.lazy_set_target(camera_control.c_look_forward));
		HFSM_State look_left = new HFSM_State ()
			.on_entry(camera_control.lazy_set_target(camera_control.c_look_left));
		HFSM_State look_right = new HFSM_State ()
			.on_entry(camera_control.lazy_set_target(camera_control.c_look_right));
		HFSM_State look = new HFSM_State ()
			.add_child(look_forward)
			.add_child(look_left)
			.add_child(look_right)
			.on_entry(lazy_log("Looking"))
			.on_update(()=>{
					cart_control.move();
			});
		look.initial = look_forward;

		//Pick
		HFSM_State pick_left = new HFSM_State ()
			.on_entry(camera_control.lazy_set_target(camera_control.c_pick_left, 20, 20));
		HFSM_State pick_right = new HFSM_State ()
			.on_entry(camera_control.lazy_set_target(camera_control.c_pick_right, 20, 20));
		HFSM_State pick = new HFSM_State ()
			.add_child(pick_left)
			.add_child(pick_right)
				.on_entry(()=>{
				StrawberryComponent.allow_picking_unpicked = true;
				}).on_exit(()=>{
					StrawberryComponent.allow_picking_unpicked = false;
				});

		//Pack
		HFSM_State pack = new HFSM_State ()
			.on_entry(()=>{
				camera_control.lazy_set_target(camera_control.c_pack)();
				//Draggable.calculate_delta = Draggable.xz_plane;
			}).on_exit(()=>{
				//Draggable.calculate_delta = Draggable.xy_plane;
			});

		//Transitions
		//Look Forward -> Look Left, Look Right, Pack
		look_forward.add_transition (
			new HFSM_Transition (look_forward, look_left, input.on_left)
		).add_transition (
			new HFSM_Transition (look_forward, look_right, input.on_right)
		).add_transition (
			new HFSM_Transition (look_forward, pack, input.on_down)
		);
		//Look Left -> Look Forward, Pick Left
		look_left.add_transition (
			new HFSM_Transition (look_left, look_forward, input.on_right)
		).add_transition (
			new HFSM_Transition (look_left, pick_left, input.on_down)
		);
		//Look Right -> Look Forward, Pick Right
		look_right.add_transition (
			new HFSM_Transition (look_right, look_forward, input.on_left)
		).add_transition (
			new HFSM_Transition (look_right, pick_right, input.on_down)
		);
		//Pick Left -> Look Left, Pack
		pick_left.add_transition(
			new HFSM_Transition (pick_left, look_left, input.on_up)
		).add_transition(
			new HFSM_Transition (pick_left, pack, hands_full)
		);
		//Pick Right -> Look Left, Pack
		pick_right.add_transition(
			new HFSM_Transition (pick_right, look_right, input.on_up)
		).add_transition(
			new HFSM_Transition (pick_right, pack, hands_full)
		);
		// Pack -> Look Forward
		pack.add_transition (
			new HFSM_Transition (pack, look_forward, input.on_up)
		);

		//Main State
		root = new HFSM_State()
			.add_child(look)
			.add_child(pick)
			.add_child(pack);

	}
	
	// Update is called once per frame
	void Update () {
		root.run();
	}
}
