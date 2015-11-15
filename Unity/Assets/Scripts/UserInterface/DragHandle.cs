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
	List<Transition> incoming = new List<Transition> ();
	List<Transition> outgoing = new List<Transition> ();
	List<State> states = new List<State> ();
	float distance;
	float target_distance;
	float drift_time;
	Vector3 offset;
	Automata a;

	public float min_distance = 0.1f;
	public float max_distance = 1.0f;
	public float scroll_speed = 1.0f;
	public float initial_drift_time = 0.5f;

	void Awake(){
		if (a == null) {
			a = gameObject.GetComponent<Automata>();
		}
	}

	protected bool _can_drag = false;
	[Show]
	public bool can_drag{
		get{ return _can_drag;}
		private set{ _can_drag = value;}
	}
	protected bool _can_release = false;
	[Show]
	public bool can_release{
		get{ return _can_drag;}
		private set{ _can_drag = value;}
	}

	public bool is_dragging(){
		if (a == null)
			return false;
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

	void OnMouseEnter(){
		foreach (Transition pickup in incoming) {
			if (pickup.test_single(a)){
				can_drag = true;
				break;
			}
		}
	}
	void OnMouseExit(){
		can_drag = false;
	}

	void OnMouseDown()
	{
		foreach (Transition pickup in incoming) {
			pickup.trigger_single(a);
		}
		Vector3 screen_pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);			
		distance = screen_pos.z;
		target_distance = Mathf.Max(
			Mathf.Min(
				distance,
				max_distance
			),min_distance
		);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
		drift_time = initial_drift_time;
	}

	void Update()
	{
		if (is_dragging()){
			target_distance = Mathf.Max(
				Mathf.Min(
					target_distance + (scroll_speed * Input.GetAxis("Mouse ScrollWheel")),
					max_distance
				),min_distance
			);
			if (drift_time > 0.0001f) {
				float elapsed = Time.deltaTime / drift_time;
				distance += (target_distance - distance) * elapsed;
				drift_time -= Time.deltaTime;
			} else {
				distance = target_distance;
			}
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