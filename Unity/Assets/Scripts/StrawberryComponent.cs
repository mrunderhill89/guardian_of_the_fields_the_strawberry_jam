using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
public class StrawberryComponent : BetterBehaviour {
	public static float density = 1.00f;
	public float quality = 1.00f;
	[Show]
	public float weight{
		get{return density * transform.localScale.x
			*transform.localScale.y
			*transform.localScale.z
			;}
	}

	public DragHandle drag = null;
	public StrawberryStateMachine berry_state;
	public Automata automata;
	new protected Renderer renderer;

	// Use this for initialization
	void Start () {
		berry_state = SingletonBehavior.get_instance<StrawberryStateMachine> ();
		automata = gameObject.GetComponent<Automata> ();
		renderer = transform.Find("Strawberry_Mesh/Cube").GetComponent<Renderer>();
		Initialize();
		drag = gameObject.GetComponent<DragHandle>();
		berry_state.register_drag_handle(drag);
	}

	public void Initialize(){
		hidden(true);
		quality = RandomUtils.random_float(GameStartData.min_ripeness, GameStartData.max_ripeness);
		StrawberryScale scale = gameObject.GetComponent<StrawberryScale>();
		if (scale != null){
			scale.Initialize();
		}
	}

	public bool hidden(){
		return !this.renderer.enabled;
	}
	public StrawberryComponent hidden(bool is_hidden){
		this.renderer.enabled = !is_hidden;
		
		transform.Find("Strawberry_Mesh/Circle").GetComponent<Renderer>().enabled = !is_hidden;
		return this;
	}
}
