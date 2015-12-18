using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Vexe.Runtime.Types;


public class Footsteps : BetterBehaviour {
	public List<AudioClip> left_foot_sounds = new List<AudioClip>();
	public List<AudioClip> right_foot_sounds = new List<AudioClip>();

	public AudioSource left_foot_source;
	public AudioSource right_foot_source;

	public float speed_adjust = 1.0f;
	public float volume_adjust = 1.0f;
	protected float record_z = 0.0f;
	public float max_delta_z = 1.0f;
	public float foot_position = 0.0f;

	public AnimationCurve pace_curve = new AnimationCurve();
	public AnimationCurve volume_curve = new AnimationCurve();

	public AudioClip left_foot_sound(){
		return left_foot_sounds [RandomUtils.random_index (left_foot_sounds)];
	}
	public AudioClip right_foot_sound(){
		return right_foot_sounds [RandomUtils.random_index (right_foot_sounds)];
	}

	// Update is called once per frame
	void Update () {
		float delta_z = transform.position.z - record_z;
		float new_foot_position = foot_position + (pace_curve.Evaluate(delta_z / max_delta_z) * speed_adjust);
		float s_old = Mathf.Sin(foot_position);
		float s_new = Mathf.Sin(new_foot_position);
		float volume = volume_adjust * volume_curve.Evaluate(Mathf.Abs(s_new - s_old)/2.0f);
		if (s_new > 0.0f && s_old < 0.0f) {
			left_foot_source.PlayOneShot(left_foot_sound(), volume);
		}
		if (s_new < 0.0f && s_old > 0.0f) {
			right_foot_source.PlayOneShot(right_foot_sound(), volume);
		}
		foot_position = new_foot_position;
		record_z = transform.position.z;
	}
}
