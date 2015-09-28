using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraController : BetterBehaviour {
	public Transform target;
	public float drift_time = 0.0f;
	public Dictionary<string,Transform> targets;

	public static float Wrap(float val, float min, float max, float min_inc, float max_inc){
		if (val < min) {
			return Wrap (val + min_inc, min, max, min_inc, max_inc);
		} else if (val > max) {
			return Wrap (val - max_inc, min, max, min_inc, max_inc);
		}
		return val;
	}
	public static Vector3 WrapAngles(Vector3 ang, float bounds = 360.0f, float inc = 360.0f){
		return new Vector3 (
			Wrap(ang.x,-bounds, bounds, inc, inc),
			Wrap(ang.y,-bounds, bounds, inc, inc),
			Wrap(ang.z,-bounds, bounds, inc, inc)
		);
	}
	public CameraController set_target(string target_name, float in_time = 0.4f){
		target = targets [target_name];
		drift_time = in_time;
		return this;
	}

	public Action lazy_set_target(string target_name){
		return () => {
			set_target(target_name);
		};
	}

	// Update is called once per frame
	void Update () {
		if (target != null) {
			if (drift_time > 0.0001f) {
				float elapsed = Time.deltaTime / drift_time;
				Vector3 pos_step = (target.transform.position - transform.position) * elapsed;
				Vector3 euler_rotation = transform.rotation.eulerAngles;
				Vector3 rot_step = WrapAngles(target.transform.rotation.eulerAngles - euler_rotation, 180.0f, 360.0f) * elapsed;
				drift_time -= Time.deltaTime;
				if (drift_time > 0.0000f){
					transform.position += pos_step;
					transform.rotation = Quaternion.Euler(WrapAngles(euler_rotation+rot_step, 180.0f, 360.0f));
				} else {
					transform.position = target.transform.position;
					transform.rotation = target.transform.rotation;
				}
			}
		}
	}

}
