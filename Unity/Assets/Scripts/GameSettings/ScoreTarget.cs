using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Vexe.Runtime.Types;
using YamlDotNet.Serialization;
using UnityEngine;

public interface IScoreTarget{
	float target{get; set;}
	IObservable<float> rx_target{get;}

	float min_accept{get; set;}
	float max_accept{get; set;}
	IObservable<float> rx_min_accept{get;}
	IObservable<float> rx_max_accept{get;}
	IObservable<float> rx_min_accept_true{get;}
	IObservable<float> rx_max_accept_true{get;}
	
	float min_close{get; set;}
	float max_close{get; set;}
	IObservable<float> rx_min_close{get;}
	IObservable<float> rx_max_close{get;}

	bool limit_upper{get; set;}
	bool limit_lower{get; set;}
	IObservable<bool> rx_limit_upper{get;}
	IObservable<bool> rx_limit_lower{get;}
	
	float flat_bonus{get; set;}
	float flat_penalty{get; set;}
	IObservable<float> rx_flat_bonus{get;}
	IObservable<float> rx_flat_penalty{get;}
	
	float range_bonus{get; set;}
	float range_penalty{get; set;}
	IObservable<float> rx_range_bonus{get;}
	IObservable<float> rx_range_penalty{get;}
	
	float epsilon{get; set;}
	
	bool is_close(float value);
	bool is_accept(float value);
	bool is_greater_than(float value);
	bool is_over(float value);
	bool is_under(float value);
	
	float evaluate(float value);
	float flat_value(float value);
	float range_value(float value);
}

public class ScoreTarget: IScoreTarget, IEquatable<IScoreTarget>{
	//X Values
		//Target
	[YamlIgnore]
	protected FloatReactiveProperty _rx_target = new FloatReactiveProperty(1.0f);
	[DontSerialize][YamlIgnore]
	public IObservable<float> rx_target{get{ return _rx_target; }}
	[Show]
	public float target {
		get{ return _rx_target.Value; } 
		set{ _rx_target.Value = value; }
	}

		//Minimum Accepted Value
	[YamlIgnore]
	protected FloatReactiveProperty _rx_min_accept = new FloatReactiveProperty(0.1f);
	[DontSerialize][YamlIgnore]
	public IObservable<float> rx_min_accept{get{ return _rx_min_accept; }}
	[Show]
	public float min_accept {
		get{ return _rx_min_accept.Value; } 
		set{
			//Restrict to positive values. 
			float valid = Mathf.Max(value, 0.0f);
			_rx_min_accept.Value = valid;
			//The accepted bounds should never be larger than the close bounds.
			if (min_close < valid) min_close = valid+epsilon;
		}
	}
	protected IObservable<float> _rx_min_accept_true;
	[DontSerialize][YamlIgnore]
	public IObservable<float> rx_min_accept_true{
		get{ 
			if (_rx_min_accept_true == null){
				_rx_min_accept_true = _rx_target.CombineLatest(_rx_min_accept, (float t, float m)=>{
					return t-m;
				});
			}
			return _rx_min_accept_true; 
		}
	}
		//Maximum Accepted Value
	protected FloatReactiveProperty _rx_max_accept = new FloatReactiveProperty(0.1f);
	[YamlIgnore]
	public IObservable<float> rx_max_accept{get{ return _rx_max_accept; }}
	[Show]
	public float max_accept {
		get{ return _rx_max_accept.Value; } 
		set{
			//Restrict to positive values. 
			float valid = Mathf.Max(value, 0.0f);
			_rx_max_accept.Value = valid;
			//The accepted bounds should never be larger than the close bounds.
			if (max_close < valid) max_close = valid+epsilon;
		}
	}
	protected IObservable<float> _rx_max_accept_true;
	[DontSerialize][YamlIgnore]
	public IObservable<float> rx_max_accept_true{
		get{ 
			if (_rx_max_accept_true == null){
				_rx_max_accept_true = _rx_target.CombineLatest(_rx_max_accept, (float t, float m)=>{
					return t+m;
				});
			}
			return _rx_max_accept_true; 
		}
	}

		//Minimum Close Value
	protected FloatReactiveProperty _rx_min_close = new FloatReactiveProperty(0.2f);
	[YamlIgnore]
	public IObservable<float> rx_min_close{get{ return _rx_min_close; }}
	[Show]
	public float min_close {
		get{ return _rx_min_close.Value; } 
		set{
			//Restrict to positive values. 
			float valid = Mathf.Max(value, 0.0f);
			_rx_min_close.Value = valid;
			//The accepted bounds should never be larger than the close bounds.
			if (min_accept > valid) min_accept = valid-epsilon;
		}
	}
	
		//Maximum Close Value
	protected FloatReactiveProperty _rx_max_close = new FloatReactiveProperty(0.2f);
	[YamlIgnore]
	public IObservable<float> rx_max_close{get{ return _rx_max_close; }}
	[Show]
	public float max_close {
		get{ return _rx_max_close.Value; } 
		set{ 
			//Restrict to positive values. 
			float valid = Mathf.Max(value, 0.0f);
			_rx_max_close.Value = valid;
			//The accepted bounds should never be larger than the close bounds.
			if (max_accept > valid) max_accept = valid-epsilon;
		}
	}
	
		//Upper Limit
	protected BoolReactiveProperty _rx_limit_upper = new BoolReactiveProperty(true);
	[YamlIgnore]
	public IObservable<bool> rx_limit_upper{get{ return _rx_limit_upper; }}
	[Show]
	public bool limit_upper {
		get{ return _rx_limit_upper.Value; } 
		set{ _rx_limit_upper.Value = value; }
	}
		//Lower Limit
	protected BoolReactiveProperty _rx_limit_lower= new BoolReactiveProperty(true);
	[YamlIgnore]
	public IObservable<bool> rx_limit_lower{get{ return _rx_limit_lower; }}
	[Show]
	public bool limit_lower {
		get{ return _rx_limit_lower.Value; } 
		set{ _rx_limit_lower.Value = value; }
	}
	
	//Y Values
		//Flat Bonus
	protected FloatReactiveProperty _rx_flat_bonus = new FloatReactiveProperty(1.0f);
	[YamlIgnore]
	public IObservable<float> rx_flat_bonus{get{ return _rx_flat_bonus; }}
	[Show]
	public float flat_bonus {
		get{ return _rx_flat_bonus.Value; } 
		set{ _rx_flat_bonus.Value = value; }
	}
		//Flat Penalty
	protected FloatReactiveProperty _rx_flat_penalty = new FloatReactiveProperty(1.0f);
	[YamlIgnore]
	public IObservable<float> rx_flat_penalty{get{ return _rx_flat_penalty; }}
	[Show]
	public float flat_penalty {
		get{ return _rx_flat_penalty.Value; } 
		set{ _rx_flat_penalty.Value = value; }
	}
		//Range Bonus
	protected FloatReactiveProperty _rx_range_bonus = new FloatReactiveProperty(1.0f);
	[YamlIgnore]
	public IObservable<float> rx_range_bonus{get{ return _rx_range_bonus; }}
	[Show]
	public float range_bonus {
		get{ return _rx_range_bonus.Value; } 
		set{ _rx_range_bonus.Value = value; }
	}
		//Range Penalty
	protected FloatReactiveProperty _rx_range_penalty = new FloatReactiveProperty(1.0f);
	[YamlIgnore]
	public IObservable<float> rx_range_penalty{get{ return _rx_range_penalty; }}
	[Show]
	public float range_penalty {
		get{ return _rx_range_penalty.Value; } 
		set{ _rx_range_penalty.Value = value; }
	}

	protected float _epsilon = 0.000001f;
	[Show]
	public float epsilon {
		get{ return _epsilon; }
		set{ _epsilon = Mathf.Max(value, 0.0f);}
	}
	
	public bool is_close(float value){
		if ((value < target && !limit_lower) || (value > target && !limit_upper))
			return true;
		if ((min_close < epsilon && value <= target) || (max_close < epsilon && value > target))
			return false;
		return value >= target-min_close && value <= target+max_close;
	}
	
	public bool is_accept(float value){
		if ((value < target && !limit_lower) || (value > target && !limit_upper))
			return true;
		if ((min_accept < epsilon && value <= target) || (max_accept < epsilon && value > target))
			return false;
		return value >= target-min_accept && value <= target+min_accept;
	}

	public bool is_greater_than(float value){
		return value > target;
	}
	
	public bool is_over(float value){
		return !is_accept(value) && is_greater_than(value);
	}
	
	public bool is_under(float value){
		return !(is_accept(value) || is_greater_than(value));
	}
	
	public float evaluate(float value){
		return flat_value(value) + range_value(value);
	}
	public float flat_value(float value){
		if (!(limit_lower || is_greater_than(value)) || (!limit_upper && is_greater_than(value)) || is_accept(value))
			return flat_bonus;
		return -flat_penalty;
	}
	/*
		Asserts:
			max_accept > 0
			min_accept > 0
			max_close > max_accept
			min_close > min_accept
	*/
	public float range_value(float value){
		if (!(limit_lower || is_greater_than(value)) || (!limit_upper && is_greater_than(value)))
			return range_bonus;
		if (is_close(value)){
			if (is_accept(value)){
				if (is_greater_than(value)){
					return range_bonus*Mathf.Clamp((1.0f + (target - value)/max_accept), 0.0f, 1.0f);
				} else {
					return range_bonus*Mathf.Clamp((value - target + min_accept)/min_accept, 0.0f, 1.0f);
				}
			} else {
				if (is_greater_than(value)){
					return (-range_penalty)*Mathf.Clamp((value - target - max_accept)/(max_close-max_accept), 0.0f, 1.0f);
				} else {
					return (-range_penalty)*Mathf.Clamp(1.0f - (value - target + min_close)/(min_close - min_accept), 0.0f, 1.0f);
				}
			}
		}
		return -range_penalty;
	}
	
	public void disable(){
		limit_upper = false;
		limit_lower = false;

		flat_bonus = 0.0f;
		flat_penalty = 0.0f;
		range_bonus = 0.0f;
		range_penalty = 0.0f;
	}
	
	public ScoreTarget copy_from(IScoreTarget that){
		target = that.target;
		min_accept = that.min_accept;
		max_accept = that.max_accept;
		min_close = that.min_close;
		max_close = that.min_close;
		
		limit_upper = that.limit_upper;
		limit_lower = that.limit_lower;
		
		flat_bonus = that.flat_bonus;
		flat_penalty = that.flat_penalty;
		range_bonus = that.range_bonus;
		range_penalty = that.range_penalty;
		
		epsilon = that.epsilon;
		return this;
	}
	public bool Equals (IScoreTarget that){
		return System.Object.ReferenceEquals(this,that) || (
			target == that.target
			&& min_accept == that.min_accept
			&& max_accept == that.max_accept
			&& min_close == that.min_close
			&& max_close == that.min_close
			&& limit_upper == that.limit_upper
			&& limit_lower == that.limit_lower
			&& flat_bonus == that.flat_bonus
			&& flat_penalty == that.flat_penalty
			&& range_bonus == that.range_bonus
			&& range_penalty == that.range_penalty
		);
	}
}