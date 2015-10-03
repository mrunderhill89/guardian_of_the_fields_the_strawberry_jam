//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;
public class DragHandle : BetterBehaviour
{
	List<Transition> incoming;
	List<Transition> outgoing;
	List<State> states;
	float distance;
	public float scroll_speed = 1.0f;
	Vector3 offset;
	Automata a;

	void Awake(){
		incoming = new List<Transition> ();
		outgoing = new List<Transition> ();
		states = new List<State> ();
		if (a == null) {
			a = gameObject.GetComponent<Automata>();
		}
	}

	public bool is_dragging(){
		foreach (State s in states) {
			if (a.visiting (s)) return true;
		}
		return false;
	}
	public DragHandle register_incoming(Transition t){
		incoming.Add(t);
		return this;
	}
	public DragHandle register_outgoing(Transition t){
		outgoing.Add(t);
		return this;
	}
	public DragHandle register_state(State s){
		states.Add (s);
		return this;
	}

	void OnMouseDown()
	{
		foreach (Transition pickup in incoming) {
			pickup.trigger_single(a);
		}
		Vector3 screen_pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);			
		distance = screen_pos.z;
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
	}

	void Update()
	{
		if (is_dragging()){
			distance += scroll_speed * Input.GetAxis("Mouse ScrollWheel");
			Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
			transform.position = curPosition;
		}
	}

	void OnMouseUp(){
		foreach (Transition drop in outgoing) {
			drop.trigger_single(a);
		}
	}
}
