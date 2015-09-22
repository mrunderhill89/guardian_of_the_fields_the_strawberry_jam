using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using UniRx;
public class Draggable : BetterBehaviour {
	public static float pitch = 0.0f;
	public float min_reach = 0.1f;
	public float max_reach = 1.0f;
	public float min_drop = 0.1f;
	public float max_drop = 2.0f;
	public Rigidbody body = null;
	public bool gravity_on_drop = true;
	public bool kinematic_on_pickup = false;
	protected Vector3 screen_pos;
	protected Vector3 prev_mouse;
	public float distance{
		get{return screen_pos.z;}
	}
	protected bool is_dragging;
	void Start(){
		body = gameObject.GetComponent<Rigidbody> ();
	}

	public UnityEvent on_pickup;
	public UnityEvent on_drag;
	public UnityEvent on_drop;
	
	public bool is_grabbable = true;
	public Func<bool> check_grabbable;
	public bool can_grab{get{
		if (check_grabbable != null){
			return check_grabbable();
		}
		return is_grabbable;
	}}

	void OnMouseDown()
	{
		//Get distance from the camera to the object
		screen_pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		if (can_grab){
			//Set the object's position to be relative to the camera.
			gameObject.transform.SetParent(Camera.main.transform,true);
			if (body != null) {
				body.useGravity = false;
				body.isKinematic = kinematic_on_pickup;
			}
			prev_mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
			is_dragging = true;
			on_pickup.Invoke();
		}
	}
	
	void OnMouseDrag()
	{
		if (is_dragging) {
			Vector3 cur_mouse = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distance);
			Vector3 delta_mouse = Camera.main.ScreenToViewportPoint (cur_mouse) - Camera.main.ScreenToViewportPoint (prev_mouse);
			Vector3 towards_camera = transform.localPosition * Input.GetAxis("Mouse ScrollWheel");
			float pitch_rad = pitch * Mathf.Deg2Rad;
			Vector3 delta_position = new Vector3 (
				                        delta_mouse.x + towards_camera.x,
										delta_mouse.y * Mathf.Cos (pitch_rad)
										+ towards_camera.y,
										delta_mouse.y * Mathf.Sin (pitch_rad)
										+ towards_camera.z
			                        );
			transform.localPosition += delta_position;
			if (body != null) {
				
			}
			prev_mouse = cur_mouse;
			on_drag.Invoke();
		}
	}

	void OnMouseUp(){
		if (is_dragging) {
			gameObject.transform.SetParent (null, true);
			if (body != null) {
				body.useGravity = gravity_on_drop;
			}
			on_drop.Invoke();
		}
		is_dragging = false;
	}
}
