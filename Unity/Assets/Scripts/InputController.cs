using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;

public class InputController: BetterBehaviour{
	protected Dictionary<string, bool> went;
	public Dictionary<string, UnityEvent> direction_events;
	protected void read_dir(string axis_name, string dir_name, bool gt){
		if (!went.ContainsKey (dir_name)) {
			went[dir_name] = false;
		}
		double axis = Input.GetAxis(axis_name);
		if (axis != 0.0 && (axis<0.0 ^ gt)){
			if (!went[dir_name]) {
				went[dir_name] = true;
				if (direction_events.ContainsKey(dir_name)){
					direction_events[dir_name].Invoke();
				}
			}
		} else {
			went[dir_name] = false;
		};
	}

	public UnityEvent on_dir(string name){
		if (!direction_events.ContainsKey (name)) {
			direction_events[name] = new UnityEvent();
		}
		return direction_events [name];
	}

	void Awake(){
		if (direction_events == null) {
			direction_events = new Dictionary<string, UnityEvent> ();
		}
		if (went == null) {
			went = new Dictionary<string, bool> ();
		}
	}

	void Update(){
		read_dir ("Horizontal", "left", false);
		read_dir ("Horizontal", "right", true);
		read_dir ("Vertical", "up", true);
		read_dir ("Vertical", "down", false);
	}
}

/*
public class InputController
{
	protected Dictionary<string, bool> went;

	protected Func<bool> on_dir(string axis_name, string dir_name, bool gt){
		if (!went.ContainsKey (dir_name)) {
			went[dir_name] = false;
		}
		return () => {
			double axis = Input.GetAxis(axis_name);
			if (axis != 0.0 && (axis<0.0 ^ gt)){
				if (!went[dir_name]) {
					went[dir_name] = true;
					return true;
				}
			} else {
				went[dir_name] = false;
			};
			return false;
		};
	}
	public Func<bool> on_left;
	public Func<bool> on_right;
	public Func<bool> on_up;
	public Func<bool> on_down;
	public IObservable<Vector3> mouse_pos;
	public IObservable<Vector3[]> mouse_delta_pos;
	public IObservable<Vector3> mouse_vel;
	public IObservable<Vector3[]> mouse_delta_vel;
	public IObservable<Vector3> mouse_acc;

	protected static InputController instance = null;
	public static InputController get_instance(){
		if (instance == null) {
			instance = new InputController();
		}
		return instance;
	}
	protected InputController(){
		went = new Dictionary<string,bool> ();
		on_left = on_dir ("Horizontal", "left", false);
		on_right = on_dir ("Horizontal", "right", true);
		on_up = on_dir ("Vertical", "up", true);
		on_down = on_dir ("Vertical", "down", false);
		mouse_pos = Observable.IntervalFrame(0).Select<long, Vector3>((frame)=>{
			return Input.mousePosition;
		});
		mouse_delta_pos = mouse_pos.Scan (new Vector3[]{Vector3.zero, Vector3.zero}, (pa,p) => {
			return new Vector3[]{pa[1],p};
		});
		mouse_vel = mouse_delta_pos.Select ((pa) => {
			return pa [1] - pa [0];
		});
		mouse_delta_vel = mouse_vel.Scan (new Vector3[]{Vector3.zero, Vector3.zero}, (pa,p) => {
			return new Vector3[]{pa[1],p};
		});
		mouse_acc = mouse_delta_vel.Select ((pa) => {
			return pa [1] - pa [0];
		});
	}
}*/
