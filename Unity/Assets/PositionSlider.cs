﻿using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

public class PositionSlider : BetterBehaviour {
	public float screen_start = 0.0f;
	public float screen_finish = 0.0f;

	public float target_finish = 1000.0f;
	public bool loop = false;

	public Transform target;
	[Show]
	public float target_position{
		get{
			if (target == null) return 0;
			return target.position.z;
		}
	}
	[Show]
	public float target_percent {
		get{
			return loop?
				(target_position % target_finish) / target_finish:
				target_position / target_finish;
		}
	}
	[Show]
	public float screen_position{
		get{
			return screen_start + 
				target_percent *
				(screen_finish - screen_start);
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = new Vector3(
			screen_position,
			transform.localPosition.y,
			transform.localPosition.z
		);
	}
}