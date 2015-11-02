using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class KeyboardSpeedControls : BetterBehaviour {
	public float max_increment = 0.05f;
	protected float hold_time = 0.0f;
	public AnimationCurve hold_curve = new AnimationCurve();
	public float hold_duration = 1.0f;
	public Slider slider;
	// Update is called once per frame
	void Update () {
		double speed_axis = Input.GetAxis("Speed");
		if (Input.GetAxis("Stop") > 0.0){
			slider.value = 0.0f;
			return;
		}
		if (speed_axis != 0.0){
			float increment = max_increment * hold_curve.Evaluate(hold_time);
			if (speed_axis > 0.0){
				slider.value += increment;
			} else if (speed_axis < 0.0) {
				slider.value -= increment;
			}
			hold_time += Time.deltaTime;
		} else {
			hold_time = 0.0f;
		}
	}
}
