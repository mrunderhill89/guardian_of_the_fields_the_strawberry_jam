using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraController : BetterBehaviour {
	public Transform target;
	public float drift_duration = 0.5f;
	float drift_point = 0.0f;
	public Dictionary<string,Transform> targets;
	public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

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
	public CameraController set_target(string target_name){
		target = targets [target_name];
		drift_point = 1.0f;
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
			if (drift_point > 0.0001f) {
				float elapsed = Time.deltaTime / drift_duration;
				drift_point -= elapsed;
				if (drift_point > 0.0000f){
					float multiplier = curve.Evaluate(1.0f - drift_point);
					Vector3 velocity = (target.position - transform.position);
					Vector3 ang_velocity = WrapAngles(
			           target.rotation.eulerAngles - transform.rotation.eulerAngles,
						180.0f,
						360.0f
					);
					transform.position += multiplier * velocity;
					transform.rotation = Quaternion.Euler (WrapAngles(
						transform.rotation.eulerAngles + (multiplier * ang_velocity),
						180.0f,
						360.0f
					));
				} else {
					transform.position = target.transform.position;
					transform.rotation = target.transform.rotation;
				}
			}
		}
	}

}
