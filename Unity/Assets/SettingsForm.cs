using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Vexe.Runtime.Types;
using UniRx;
public class SettingsForm : BetterBehaviour {
	public GameStartData data_component;
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
	public InputField penalty_small;
	public InputField penalty_medium;
	public InputField penalty_big;

	//Sliders
	public Slider min_ripeness;
	public Slider max_ripeness;
	public Slider min_accepted_ripeness;
	public Slider max_accepted_ripeness;

	public ReactiveProperty<bool> using_main;
	
	void Awake(){
		using_main = data_component.current.Select((data)=>{
			return data == GameStartData.instance;
		}).ToReactiveProperty();
	}
	// Update is called once per frame
	void Start () {
		randomize.onValueChanged.AddListener((value)=>{
			start_data.randomize = value;
		});
		debug.onValueChanged.AddListener((value)=>{
			start_data.debug = value;
		});
		tutorial.onValueChanged.AddListener((value)=>{
			start_data.tutorial = value;
		});
		data_component.current.Subscribe((data)=>{
			rng_seed.text = data.rng_seed.ToString();
			randomize.isOn = data.randomize;
			debug.isOn = data.debug;
			tutorial.isOn = data.tutorial;
			max_berries.text = data.max_berries_in_field.ToString();
			min_size.text = data.min_size.ToString();
			max_size.text = data.max_size.ToString();
			density.text = data.berry_density.ToString();
			break_distance.text = data.break_distance.ToString();
			break_length.text = data.break_length.ToString();//
			game_length.text = data.game_length.ToString();
			start_hour.text = data.start_hour.ToString();
			end_hour.text = data.end_hour.ToString();
			min_berry_weight.text = data.min_berry_weight.ToString();
			min_basket_weight.text = data.min_basket_weight.ToString();
			max_basket_weight.text = data.max_basket_weight.ToString();
			min_ripeness.value = data.min_ripeness;
			max_ripeness.value = data.max_ripeness;
			min_accepted_ripeness.value = data.min_accepted_ripeness;
			max_accepted_ripeness.value = data.max_accepted_ripeness;
			penalty_small.text = data.get_penalty(StrawberryComponent.BerryPenalty.Small).ToString();
			penalty_medium.text = data.get_penalty(StrawberryComponent.BerryPenalty.Medium).ToString();
			penalty_big.text = data.get_penalty(StrawberryComponent.BerryPenalty.Big).ToString();
		});
	}
	
	protected GameStartData.StartData start_data{
		get{
			if (data_component == null) return null;
			return data_component.current.Value;
		}
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

	
	void Update(){
		if (start_data != null){
			rng_seed.enabled = !start_data.randomize;
			start_data.rng_seed = ParseIntOrDefault(rng_seed.text, start_data.rng_seed);
			start_data.max_berries_in_field = ParseIntOrDefault(max_berries.text, start_data.max_berries_in_field);
			start_data.min_size = ParseFloatOrDefault(min_size.text, start_data.min_size);
			start_data.max_size = ParseFloatOrDefault(max_size.text, start_data.max_size);
			start_data.min_ripeness = min_ripeness.value;
			start_data.max_ripeness = max_ripeness.value;
			start_data.berry_density = ParseFloatOrDefault(density.text, start_data.berry_density);
			start_data.break_distance = ParseIntOrDefault(break_distance.text, start_data.break_distance);
			start_data.break_length = ParseIntOrDefault(break_length.text, start_data.break_length);
			start_data.game_length = ParseFloatOrDefault(game_length.text, start_data.game_length);
			start_data.start_hour = ParseFloatOrDefault(start_hour.text, start_data.start_hour);
			start_data.end_hour = ParseFloatOrDefault(end_hour.text, start_data.end_hour);
			start_data.min_accepted_ripeness = min_accepted_ripeness.value;
			start_data.max_accepted_ripeness = max_accepted_ripeness.value;
			start_data.min_berry_weight = ParseFloatOrDefault(min_berry_weight.text, start_data.min_berry_weight);
			start_data.min_basket_weight = ParseFloatOrDefault(min_basket_weight.text, start_data.min_basket_weight);
			start_data.max_basket_weight = ParseFloatOrDefault(max_basket_weight.text, start_data.max_basket_weight);
			start_data.set_penalty(StrawberryComponent.BerryPenalty.Small, 
				ParseFloatOrDefault(penalty_small.text, start_data.get_penalty(StrawberryComponent.BerryPenalty.Small)));
			start_data.set_penalty(StrawberryComponent.BerryPenalty.Medium, 
				ParseFloatOrDefault(penalty_medium.text, start_data.get_penalty(StrawberryComponent.BerryPenalty.Medium)));
			start_data.set_penalty(StrawberryComponent.BerryPenalty.Big, 
				ParseFloatOrDefault(penalty_big.text, start_data.get_penalty(StrawberryComponent.BerryPenalty.Big)));
		}
	}
}
