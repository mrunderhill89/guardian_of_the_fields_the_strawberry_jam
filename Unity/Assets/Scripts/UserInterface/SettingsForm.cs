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
	public InputField min_berry_weight;
	public InputField min_basket_weight;
	public InputField max_basket_weight;
	public InputField distance_target;


	//Sliders
	public Slider min_ripeness;
	public Slider max_ripeness;
	public Slider min_accepted_ripeness;
	public Slider max_accepted_ripeness;

	void Start () {
		register_toggle (randomize, (x => x.randomness.rx_randomize));
		register_toggle (tutorial, (x => x.flags.rx_tutorial));
		register_toggle (debug, (x => x.flags.rx_cheats));
		register_input_int (rng_seed, (x => x.randomness.rx_seed));
		register_input_int (max_berries, (x => x.strawberry.rx_max_berries_in_field));
		register_input_float (min_size, (x => x.strawberry.rx_min_size));
		register_input_float (max_size, (x => x.strawberry.rx_max_size));
		register_input_float (density, (x => x.strawberry.rx_density));
		register_input_int (break_distance, (x=> x.breaks.rx_distance));
		register_input_int (break_length, (x=> x.breaks.rx_length));
		register_input_float (game_length, (x => x.time.rx_game_length));
		register_input_float (start_hour, (x => x.time.rx_start_hour));
		register_input_float (end_hour, (x => x.time.rx_end_hour));
		register_slider (min_ripeness, (x => x.strawberry.rx_min_ripeness));
		register_slider (max_ripeness, (x => x.strawberry.rx_max_ripeness));
		
		register_input_float (distance_target, (x => x.win_condition.distance_covered.rx_target));
	}

	public SettingsForm register_toggle(Toggle toggle, Func<GameSettings.Model, BoolReactiveProperty> get_property){
		data_component.rx_current_rules
			.SelectMany<GameSettings.Model, bool>((model)=>{
				if (model == null) return Observable.Never<bool>();
				return get_property(model).AsObservable<bool>();
			})
			.DistinctUntilChanged()
			.Subscribe ((value) => {
				toggle.isOn = value;
			});
		toggle.onValueChanged.AddListener ((value) => {
			get_property(data_component.current_rules).Value = value;
		});
		return this;
	}

	public SettingsForm register_slider(Slider slider, Func<GameSettings.Model, FloatReactiveProperty> get_property){
		data_component.rx_current_rules
			.SelectMany<GameSettings.Model, float>((model)=>{
				if (model == null) return Observable.Never<float>();
				return get_property(model).AsObservable<float>();
			}).DistinctUntilChanged()
			.Subscribe ((value) => {
				slider.value = value;
			});
		slider.onValueChanged.AddListener ((value) => {
			get_property(data_component.current_rules).Value = value;
		});
		return this;
	}

	public SettingsForm register_input_int(InputField input, Func<GameSettings.Model, IntReactiveProperty> get_property){
		data_component.rx_current_rules
			.SelectMany<GameSettings.Model, int>((model)=>{
				if (model == null) return Observable.Never<int>();
				return get_property(model).AsObservable<int>();
			}).DistinctUntilChanged()
			.Subscribe ((value) => {
				input.text = value.ToString();
			});
		input.onValueChange.AddListener ((value) => {
			get_property(data_component.current_rules).Value = 
				ParseIntOrDefault(input.text, get_property(data_component.current_rules).Value);
		});
		return this;
	}

	public SettingsForm register_input_float(InputField input, Func<GameSettings.Model, FloatReactiveProperty> get_property){
		data_component.rx_current_rules
			.SelectMany<GameSettings.Model, float>((model)=>{
				if (model == null) return Observable.Never<float>();
				return get_property(model).AsObservable<float>();
			}).DistinctUntilChanged()
			.Subscribe ((value) => {
				input.text = value.ToString();
			});
		input.onValueChange.AddListener ((value) => {
			get_property(data_component.current_rules).Value = 
				ParseFloatOrDefault(input.text, get_property(data_component.current_rules).Value);
		});
		return this;
	}


	public static int ParseIntOrDefault(string s, int def)
	{
		int number;
		if (int.TryParse(s, out number))
			return number;
		return def;
	}

	public static float ParseFloatOrDefault(string s, float def)
	{
		float number;
		if (float.TryParse(s, out number))
			return number;
		return def;
	}
}
