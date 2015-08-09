using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {
	public Transform target;
	public int default_frames = 60;
	//Link these to the appropriate view objects
	public Transform c_look_forward = null,c_look_left = null,c_look_right = null;
	public Transform c_pick_left = null,c_pick_right = null,c_pack = null;

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
	public Action lazy_set_target(Transform t){
		return this.lazy_set_target (t, this.default_frames);
	}
	public Action lazy_set_target(Transform t, int in_frames){
		return () => {
			this.StartCoroutine(this.drift_to_target(t,in_frames));
		};
	}

	public IEnumerator drift_to_target(Transform t, int in_frames){
		float percent, inv_percent;
		Vector3 p_0 = this.transform.position;
		Vector3 p_f = t.position;
		Vector3 r_0 = WrapAngles(this.transform.rotation.eulerAngles, 180.0f, 360.0f);
		Vector3 r_f = WrapAngles(t.rotation.eulerAngles, 180.0f, 360.0f);
		for (int frame = 0; frame <= in_frames; frame++) {
			percent = Math.Min(((float)frame) / ((float)in_frames), 1.0f);
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
