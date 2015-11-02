using Vexe.Runtime.Types;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaceStateManager : BetterBehaviour {
	protected StateMachine fsm;
	protected float _speed = 0.0f;
	protected float _target_speed = 0.0f;
	protected float _curve_point = 0.0f;
	public float acceleration = 1.0f;
	public AnimationCurve anim_curve = new AnimationCurve();
	public ObjectVisibility ui_visibility;

	protected Dictionary<string, float> _time_in_pace = new Dictionary<string,float>();
	protected Dictionary<string, int> _pace_changes = new Dictionary<string,int>();
	protected List<string> _enable_in_states = new List<string>();
	[Show]
	public Dictionary<string, float> time_in_pace{
		get{return _time_in_pace;}
	}
	[Show]
	public Dictionary<string, int> pace_changes{
		get{return _pace_changes;}
	}
	[Show]
	public List<string> enable_in_states {
		get{return _enable_in_states;}
		private set{_enable_in_states = value;}
	}

	public PaceStateManager record_pace_time(string name, float dt=0.0f){
		if (!time_in_pace.ContainsKey(name)){
			time_in_pace[name] = 0.0f;
		}
		time_in_pace[name] += dt;
		return this;
	}
	public PaceStateManager record_pace_change(string name){
		if (!pace_changes.ContainsKey(name)){
			pace_changes[name]=0;
		}
		pace_changes[name]++;
		return this;
	}

	public float target_speed(){
		return _target_speed;
	}
	
	public PaceStateManager target_speed(float value){
		if (_target_speed != value) {
			reset_curve ();
		}
		_target_speed = value;
		return this;
	}
	public PaceStateManager reset_curve(){
		_curve_point = 1.0f;
		return this;
	}
	
	protected State new_pace(string name, GameStateManager game_state, Dictionary<string,float> speeds){
		return fsm.state(name).on_update(new StateEvent(()=>{
					record_pace_time(name, Time.deltaTime);
					target_speed(game_state.fsm.match<float>(speeds, 0.0f));
				}));
	}
	
	protected Transition new_pace_transition(string from, string to, string _prefix = ""){
		string prefix = _prefix.Length > 0? _prefix+":":"";
		string t_name = prefix+from+"=>"+to;
		return fsm.transition(t_name)
			.chain_from(fsm.state(from)).chain_to(fsm.state(to))
			.on_transfer(new TransitionEvent(()=>{
				record_pace_change(t_name);
			})).chain_auto_run(false);
	}
	// Use this for initialization
	void Start () {
		GameStateManager game_state = SingletonBehavior.get_instance<GameStateManager> ();
		if (fsm == null){
			fsm = GetComponent<StateMachine>();
			if (fsm == null){
				fsm = gameObject.AddComponent<StateMachine>();
			}
		}
		if (ui_visibility == null){
			foreach(string state_name in enable_in_states){
				fsm.state(state_name)
					.on_entry(new StateEvent(()=>{
						ui_visibility.visible = true;
					}))
					.on_exit(new StateEvent(()=>{
						ui_visibility.visible = false;
					}));
			}
		}
		fsm.state ("root")
		.add_child (
			new_pace("slow", game_state, new Dictionary<string,float>(){
				{"look_forward", 0.3f},
				{"look_left", 0.2f},
				{"look_right", 0.2f}
			})
			,true
		).add_child (
			new_pace("medium", game_state, new Dictionary<string,float>(){
				{"look_forward", 0.5f},
				{"look_left", 0.5f},
				{"look_right", 0.5f}
			})
		).add_child (
			new_pace("fast", game_state, new Dictionary<string,float>(){
				{"look_forward", 1.0f},
				{"look_left", 0.8f},
				{"look_right", 0.8f},
				{"look_behind", 0.5f},
				{"pack", 0.6f}
			})
		).add_child (
			new_pace("reverse", game_state, new Dictionary<string,float>(){
				{"look_behind", -0.3f},
				{"look_left", -0.2f},
				{"look_right", -0.2f}
			})
		);
		//Direct Transitions
		fsm.add_transition(new_pace_transition("root", "slow", "direct"))
		.add_transition(new_pace_transition("root", "medium", "direct"))
		.add_transition(new_pace_transition("root", "fast", "direct"))
		.add_transition(new_pace_transition("root", "reverse", "direct"));
		//Speed Up
		fsm.add_transition(new_pace_transition("reverse", "slow", "accel"))
		.add_transition(new_pace_transition("slow", "medium", "accel"))
		.add_transition(new_pace_transition("medium", "fast", "accel"));
		//Slow Down
		fsm.add_transition(new_pace_transition("slow", "reverse", "decel"))
		.add_transition(new_pace_transition("medium", "slow", "decel"))
		.add_transition(new_pace_transition("fast", "medium", "decel"));
		fsm.new_automata ("current_pace", (a) => {
			a.move_direct(fsm.state("root"));
		});
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
	[Show]
	public void set_pace(string name){
		fsm.transition("direct:root=>"+name).trigger();
	}
	[Show]
	public PaceStateManager speed_up(){
		foreach(KeyValuePair<string,Transition> pair in fsm.transitions){
			if (pair.Key.StartsWith("accel:")){
				if(pair.Value.test_any()){
					pair.Value.trigger();
					break;
				}
			}
		}
		return this;
	}
	[Show]
	public PaceStateManager slow_down(){
		foreach(KeyValuePair<string,Transition> pair in fsm.transitions){
			if (pair.Key.StartsWith("decel:")){
				if(pair.Value.test_any()){
					pair.Value.trigger();
					break;
				}
			}
		}
		return this;
	}
}