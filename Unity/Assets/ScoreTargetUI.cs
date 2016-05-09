using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Vexe.Runtime.Types;

public interface IScoreTargetUI{
	ScoreTarget data{get; set;}
}

public class ScoreTargetUI : BetterBehaviour, IScoreTargetUI {
	public ReactiveProperty<ScoreTarget> rx_data = new ReactiveProperty<ScoreTarget>();
	[DontSerialize]
	public IObservable<ScoreTarget> rx_valid_data;
	public ScoreTarget data{
		get{ return rx_data.Value; }
		set{ rx_data.Value = value; }
	}

	public InputField target;
	public Toggle lower_limit;
	public Toggle upper_limit;
	public InputField min_accept;
	public InputField max_accept;
	public InputField min_close;
	public InputField max_close;
	public InputField flat_bonus;
	public InputField flat_penalty;
	public InputField range_bonus;
	public InputField range_penalty;

	public ObjectVisibility hide_when_unlimited;

	public RxUIAdapter adapter = new RxUIAdapter();
	void Start () {
		rx_valid_data = rx_data.Where(data=>data != null);
		adapter.register_input_float(target, rx_valid_data.SelectMany(d=>d.rx_target), (value)=>{ //Target
			if (data != null)
				data.target = value;
		}).register_toggle(lower_limit, rx_valid_data.SelectMany(d=>d.rx_limit_lower), (value)=>{ //Lower Limit
			if (data != null)
				data.limit_lower = value;
		}).register_toggle(upper_limit, rx_valid_data.SelectMany(d=>d.rx_limit_upper), (value)=>{ //Upper Limit
			if (data != null)
				data.limit_upper = value;
		}).register_input_float(min_accept, rx_valid_data.SelectMany(d=>d.rx_min_accept), (value)=>{ //Min Accept
			if (data != null)
				data.min_accept = value;
		}).register_input_float(max_accept, rx_valid_data.SelectMany(d=>d.rx_max_accept), (value)=>{ //Max Accept
			if (data != null)
				data.max_accept = value;
		}).register_input_float(min_close, rx_valid_data.SelectMany(d=>d.rx_min_close), (value)=>{ //Min Close
			if (data != null)
				data.min_close = value;
		}).register_input_float(max_close, rx_valid_data.SelectMany(d=>d.rx_max_close), (value)=>{ //Max Close
			if (data != null)
				data.max_close = value;
		}).register_input_float(flat_bonus, rx_valid_data.SelectMany(d=>d.rx_flat_bonus), (value)=>{ //Flat Bonus
			if (data != null)
				data.flat_bonus = value;
		}).register_input_float(flat_penalty, rx_valid_data.SelectMany(d=>d.rx_flat_penalty), (value)=>{ //Flat Penalty
			if (data != null)
				data.flat_penalty = value;
		}).register_input_float(range_bonus, rx_valid_data.SelectMany(d=>d.rx_max_close), (value)=>{ //Range Bonus
			if (data != null)
				data.range_bonus = value;
		}).register_input_float(range_penalty, rx_valid_data.SelectMany(d=>d.rx_max_close), (value)=>{ //Range Penalty
			if (data != null)
				data.range_penalty = value;
		});
		rx_valid_data.SelectMany(d=>d.rx_is_limited)
		.DistinctUntilChanged().Subscribe((limited)=>{
			if (hide_when_unlimited != null)
				hide_when_unlimited.visible = limited;
		});
	}
}
