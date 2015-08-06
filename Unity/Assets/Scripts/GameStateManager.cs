﻿using UnityEngine;
using System.Collections;
using System;

public class GameStateManager : MonoBehaviour {
	public HFSM_State root;
	public InputController input;
	public Transform camera_transform = null;
	public Transform c_look_forward = null,c_look_left = null,c_look_right = null;
	public Transform c_pick_left = null,c_pick_right = null,c_pack = null;
	Action lazy_log(string message){
		return () => {
			Debug.Log (message);
		};
	}
	Action set_camera(Transform t){
		return () => {
			if (t != null){
				camera_transform.position = t.position;
				camera_transform.rotation = t.rotation;
			}
		};
	}
	void Start () {
		input = new InputController();
		Func<bool> hands_full = () => {
			return false;
		};
		//Look
		HFSM_State look_forward = new HFSM_State ()
			.on_entry(set_camera(c_look_forward));
		HFSM_State look_left = new HFSM_State ()
			.on_entry(set_camera(c_look_left));
		HFSM_State look_right = new HFSM_State ()
			.on_entry(set_camera(c_look_right));
		HFSM_State look = new HFSM_State ()
			.add_child(look_forward)
			.add_child(look_left)
			.add_child(look_right)
			.on_entry(lazy_log("Looking"));
		look.initial = look_forward;

		//Pick
		HFSM_State pick_left = new HFSM_State ()
			.on_entry(set_camera(c_pick_left));
		HFSM_State pick_right = new HFSM_State ()
			.on_entry(set_camera(c_pick_right));
		HFSM_State pick = new HFSM_State ()
			.add_child(pick_left)
			.add_child(pick_right)
			.on_entry(lazy_log("Picking"));

		//Pack
		HFSM_State pack = new HFSM_State ()
			.on_entry(set_camera(c_pack));;

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