using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaceManager : BetterBehaviour {
	public Dictionary<string, PaceData> paces = new Dictionary<string, PaceData>();
	protected PaceData current_pace;
	protected float _speed = 0.0f;
	protected float _target_speed = 0.0f;
	protected float _curve_point = 0.0f;
	public float acceleration = 1.0f;
	public AnimationCurve anim_curve = new AnimationCurve();

	public float target_speed(){
		return _target_speed;
	}
	public PaceManager target_speed(float value){
		if (_target_speed != value) {
			reset_curve ();
		}
		_target_speed = value;
		return this;
	}
	public PaceManager reset_curve(){
		_curve_point = 1.0f;
		return this;
	}
	// Use this for initialization
	void Start () {
		GameStateManager game_state = SingletonBehavior.get_instance<GameStateManager> ();
		paces ["fast"] = ScriptableObject.CreateInstance<PaceData>()
			.manager(this)
			.attach_to_state(game_state.state("look_forward"), 1.0f)
			.attach_to_state(game_state.state("look_left"), 0.8f)
			.attach_to_state(game_state.state("look_right"), 0.8f)
			.attach_to_state(game_state.state("look_behind"), 0.5f)
			.attach_to_state(game_state.state("pack"), 0.6f);
		paces ["medium"] = ScriptableObject.CreateInstance<PaceData>()
			.manager(this)
			.attach_to_state(game_state.state("look_forward"), 0.5f)
			.attach_to_state(game_state.state("look_left"), 0.5f)
			.attach_to_state(game_state.state("look_right"), 0.5f);
		paces ["slow"] = ScriptableObject.CreateInstance<PaceData>()
			.manager(this)
			.attach_to_state(game_state.state("look_forward"), 0.3f)
			.attach_to_state(game_state.state("look_left"), 0.2f)
			.attach_to_state(game_state.state("look_right"), 0.2f);
		paces ["reverse"] = ScriptableObject.CreateInstance<PaceData>()
			.manager(this)
			.attach_to_state(game_state.state("look_behind"), -0.3f)
			.attach_to_state(game_state.state("look_left"), -0.2f)
			.attach_to_state(game_state.state("look_right"), -0.2f);
		set_pace("slow");
	}

	void Update(){
		if (_curve_point > 0.0f) {
			float percent = anim_curve.Evaluate (_curve_point);
			_speed = (percent *_speed) + ((1.0f - percent) * _target_speed);
			_curve_point -= Time.deltaTime * acceleration;
		} else {
			_speed = _target_speed;
		}
		transform.Translate(new Vector3(0.0f,0.0f,_speed * Time.deltaTime));
	}
	public bool is_in_pace(PaceData pace){
		return current_pace == pace;
	}
	[Show]
	public void set_pace(string name){
		if (paces.ContainsKey (name)) {
			target_speed(0.0f);
			current_pace = paces [name];
		}
	}
}