using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Vexe.Runtime.Types;
using UniRx;
public class SettingsForm : BetterBehaviour {
	public GameSettingsComponent data_component;
	//Toggles
	/* 
	Randomize
	Debug
	Tutorial
	*/
	public Toggle randomize;
	public Toggle debug;
	public Toggle tutorial;

	//Input Fields
	public InputField rng_seed;//
	public InputField max_berries;//
	public InputField min_size;//
	public InputField max_size;//
	public InputField density;//
	public InputField break_distance;
	public InputField break_length;
	public InputField game_length;
	public InputField start_hour;
	public InputField end_hour;
	public InputField movement_speed;
	public InputField slide_speed;


	//Sliders
	public Slider min_ripeness;
	public Slider max_ripeness;

	RxUIAdapter adapter = new RxUIAdapter();
	
	IObservable<T> get_property<T>(Func<GameSettings.Model, IObservable<T>> extract){
		return data_component.rx_current_rules.Where(rules=> rules != null).SelectMany(extract);
	}

	void Start () {
		adapter.register_toggle (randomize, get_property<bool>(rules => rules.randomness.rx_randomize),
			(value)=>{data_component.current_rules.randomness.randomize = value;}
		).register_toggle (tutorial, get_property<bool>(rules=> rules.flags.rx_tutorial),
			(value)=>{data_component.current_rules.flags.tutorial = value;}
		).register_toggle (debug, get_property<bool>(rules=> rules.flags.rx_cheats),
			(value)=>{data_component.current_rules.flags.cheats = value;}
		).register_input_int(rng_seed, get_property<int>(rules=> rules.randomness.rx_seed),
			(value)=>{data_component.current_rules.randomness.seed = value;}
		).register_input_int (max_berries, get_property<int>(rules=> rules.strawberry.rx_max_berries_in_field),
			(value)=>{data_component.current_rules.strawberry.max_berries_in_field = value;}
		).register_input_float (min_size, get_property<float>(rules=> rules.strawberry.rx_min_size),
			(value)=>{data_component.current_rules.strawberry.min_size = value;}
		).register_input_float (max_size, get_property<float>(rules => rules.strawberry.rx_max_size),
			(value)=>{data_component.current_rules.strawberry.max_size = value;}
		).register_input_float (density, get_property<float>(rules => rules.strawberry.rx_density),
			(value)=>{data_component.current_rules.strawberry.density = value;}
		).register_input_int (break_distance, get_property<int>(rules => rules.breaks.rx_distance),
			(value)=>{data_component.current_rules.breaks.distance = value;}
		).register_input_int (break_length, get_property<int>(rules => rules.breaks.rx_length),
			(value)=>{data_component.current_rules.breaks.length = value;}
		).register_input_float (game_length, get_property<float>(rules => rules.time.rx_game_length),
			(value)=>{data_component.current_rules.time.game_length = value;}
		).register_input_float (start_hour, get_property<float>(rules => rules.time.rx_start_hour),
			(value)=>{data_component.current_rules.time.start_hour = value;}
		).register_input_float (end_hour, get_property<float>(rules => rules.time.rx_end_hour),
			(value)=>{data_component.current_rules.time.end_hour = value;}
		).register_slider (min_ripeness, get_property<float>(rules => rules.strawberry.rx_min_ripeness),
			(value)=>{data_component.current_rules.strawberry.min_ripeness = value;}
		).register_slider (max_ripeness, get_property<float>(rules => rules.strawberry.rx_max_ripeness),
			(value)=>{data_component.current_rules.strawberry.max_ripeness = value;}
		).register_input_float (movement_speed, get_property<float>(rules => rules.time.rx_movement_speed),
			(value)=>{data_component.current_rules.time.movement_speed = value;}
		).register_input_float (slide_speed, get_property<float>(rules => rules.time.rx_slide_speed),
			(value)=>{data_component.current_rules.time.slide_speed = value;}
		);
	}

	void OnDestroy(){
		adapter.Dispose();
	}
}
