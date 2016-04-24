using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaceManager : BetterBehaviour {
	public float max_speed {
		get{ return GameSettingsComponent.working_rules.time.movement_speed; }
	}
	public GameStateManager player_state;
	[Serialize]
	protected Dictionary<string,Vector2> state_data = new Dictionary<string,Vector2>();
	protected float _pace = 0.25f;
	public float pace{
		get{ return _pace;}
		set{ _pace = value;}
	}
	public PaceManager chain_pace(float value){
		pace = value;
		return this;
	}
	
	public float slide_speed {
		get{ return GameSettingsComponent.working_rules.time.slide_speed; }
	}
	public bool slide_forward {get;set;}
	public bool slide_reverse {get;set;}
	
	[Show]
	public float target_speed{
		get{
			if (slide_forward) return slide_speed;
			if (slide_reverse) return -slide_speed;
			if (state_data == null || player_state == null || player_state.fsm == null) return 0.0f;
			Vector2 pace_factors = player_state.fsm.match<Vector2>(state_data, Vector2.zero);
			if (player_state.fsm.is_state_visited("pack") && !move_while_packing) return 0.0f;
			float raw_speed = max_speed * pace;
			if (pace < 0.0f){
				return raw_speed * pace_factors.y;
			} else {
				return raw_speed * pace_factors.x;
			}
		}
	}
	[Show]
	protected float current_speed = 0.0f;
	[Serialize][Hide]
	protected bool _move_while_packing = false;
	public bool move_while_packing {
		get{return _move_while_packing;}
		set{ _move_while_packing = value;}
	}
	public float max_acceleration = 2.0f;
	// Use this for initialization
	void Start () {
		slide_forward = false;
		slide_reverse = false;
	}

	void Update(){
		float dv = Mathf.Min(
				Mathf.Abs(current_speed-target_speed), 
				max_acceleration * Time.deltaTime
		);
		if (current_speed > target_speed){
			current_speed -= dv;
		} else if (current_speed < target_speed){
			current_speed += dv;
		}
		transform.Translate(new Vector3(0.0f,0.0f,current_speed * Time.deltaTime));
	}
}