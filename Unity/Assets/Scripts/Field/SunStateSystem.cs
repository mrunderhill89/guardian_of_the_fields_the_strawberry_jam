﻿using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
public class SunStateSystem : BetterBehaviour {
	StateMachine fsm;
	public GameTimer timer;
	public float zero_angle_hour = 6.0f;
	[Show]
	public float sun_angle{
		get{ return transform.rotation.eulerAngles.x;}
		set{
			Vector3 euler = new Vector3(value,90.0f,90.0f);
			transform.rotation = Quaternion.Euler(euler);
		}
	}
	public float hour_to_angle(float hr){
		return (hr-zero_angle_hour) * 15.0f;
	}
	void Update(){
		sun_angle = hour_to_angle(timer.time.game_f_hours);
	}
}
