using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
	public InputController(){
		went = new Dictionary<string,bool> ();
		on_left = on_dir ("Horizontal", "left", false);
		on_right = on_dir ("Horizontal", "right", true);
		on_up = on_dir ("Vertical", "up", true);
		on_down = on_dir ("Vertical", "down", false);
	}
}
