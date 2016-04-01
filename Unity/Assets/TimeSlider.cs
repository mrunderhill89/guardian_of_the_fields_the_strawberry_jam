using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

public class TimeSlider : BetterBehaviour {
	public float screen_start = 0.0f;
	public float screen_finish = 0.0f;

	public GameTimer timer;

	public float target_percent {
		get{
			return Mathf.Clamp(timer.time.total / GameSettingsComponent.working_rules.time.game_length, 0.0f, 1.0f);
		}
	}
	public float screen_position{
		get{
			if (GameSettingsComponent.working_rules.time.infinite_length){
				return 0;
			}
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
