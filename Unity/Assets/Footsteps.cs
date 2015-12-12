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
	public float foot_position = 0.0f;

	public AnimationCurve volume_curve = new AnimationCurve();

	public AudioClip left_foot_sound(){
		return left_foot_sounds [RandomUtils.random_index (left_foot_sounds)];
	}
	public AudioClip right_foot_sound(){
		return right_foot_sounds [RandomUtils.random_index (right_foot_sounds)];
	}

	// Update is called once per frame
	void Update () {
		float new_foot_position = Mathf.Sin (transform.position.z * speed_adjust);
		float volume = volume_curve.Evaluate(Mathf.Abs (foot_position-new_foot_position)/2.0f);
		if (new_foot_position > 0.0f && foot_position < 0.0f) {
			left_foot_source.PlayOneShot(left_foot_sound(), volume);
		}
		if (new_foot_position < 0.0f && foot_position > 0.0f) {
			right_foot_source.PlayOneShot(right_foot_sound(), volume);
		}
		foot_position = new_foot_position;
	}
}
