using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
public class StrawberryComponent : BetterBehaviour {
	public float quality = 1.00f;

	public Draggable drag = null;
	public Behaviour glow = null;
	public StrawberryStateMachine berry_state;
	public Automata automata;
	public Transform skin;
	public Material material;
	// Use this for initialization
	void Start () {
		berry_state = SingletonBehavior.get_instance<StrawberryStateMachine> ();
		automata = gameObject.GetComponent<Automata> ();
		skin = transform.Find("Strawberry_Mesh/Cube");
		material = skin.GetComponent<Renderer>().material;
		Initialize();
		//Disable dragging until we determine whether we can pick up berries yet.
		drag = gameObject.GetComponent<Draggable>();
		glow = (gameObject.GetComponent("Halo") as Behaviour);
		glow.enabled = false;
	}
	
	public void Initialize(){
		quality = RandomUtils.random_float(0.0f, 2.0f);
		material.SetFloat("_Quality",quality);
		StrawberryScale scale = gameObject.GetComponent<StrawberryScale>();
		if (scale != null){
			scale.Initialize();
		}
	}

	void OnMouseEnter(){
		glow.enabled = true;
	}

	void OnMouseExit(){
		glow.enabled = false;
	}

	void OnMouseDown(){
		berry_state.transitions["field_drag"].trigger_single (automata);
		berry_state.transitions["fall_drag"].trigger_single (automata);
		berry_state.transitions["hold_drag"].trigger_single (automata);
		berry_state.transitions["basket_drag"].trigger_single (automata);
	}
	
	void OnMouseUp(){
		berry_state.transitions["drag_fall"].trigger_single (gameObject.GetComponent<Automata> ());
	}
	// Update is called once per frame
	void Update () {
	}
}
