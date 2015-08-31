using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraController : BetterBehaviour {
	public Transform target;
	public int default_frames = 60;
	public int default_delay = 0;
	public Dictionary<string,Transform> targets;
	// Use this for initialization
	void Start () {
	}

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
	public Action lazy_set_target(string target_name){
		return this.lazy_set_target (targets [target_name], this.default_frames, this.default_delay);
	}
	public Action lazy_set_target(string target_name, int in_frames, int delay){
		return this.lazy_set_target (targets [target_name], in_frames, delay);
	}
	public Action lazy_set_target(Transform t){
		return this.lazy_set_target (t, this.default_frames, this.default_delay);
	}
	public Action lazy_set_target(Transform t, int in_frames, int delay){
		return () => {
			this.StartCoroutine(this.drift_to_target(t,in_frames, delay));
		};
	}

	public void SetCameraTarget(string target_name){
		this.StartCoroutine(this.drift_to_target(targets[target_name],this.default_frames, this.default_delay));
	}
		
	public IEnumerator drift_to_target(Transform t, int in_frames, int delay_frames = 0){
		float percent, inv_percent;
		Vector3 p_0 = this.transform.position;
		Vector3 p_f = t.position;
		Vector3 r_0 = WrapAngles(this.transform.rotation.eulerAngles, 180.0f, 360.0f);
		Vector3 r_f = WrapAngles(t.rotation.eulerAngles, 180.0f, 360.0f);
		for (int frame = -delay_frames; frame <= in_frames; frame++) {
			percent = Math.Max(Math.Min(((float)frame) / ((float)in_frames), 1.0f),0.0f);
			inv_percent = 1.0f - percent;
			this.transform.position = (p_f * percent) + (p_0 * inv_percent);
			this.transform.rotation = Quaternion.Euler(r_f * percent + r_0 * inv_percent);
			yield return null;
		}
	}
	// Update is called once per frame
	void Update () {
	}

}
