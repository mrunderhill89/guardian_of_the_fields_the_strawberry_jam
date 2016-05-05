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
	FloatReactiveProperty rx_target{get;}

	float min_accept{get; set;}
	float max_accept{get; set;}
	FloatReactiveProperty rx_min_accept{get;}
	FloatReactiveProperty rx_max_accept{get;}
	IObservable<float> rx_min_accept_true{get;}
	IObservable<float> rx_max_accept_true{get;}
	
	float min_close{get; set;}
	float max_close{get; set;}
	FloatReactiveProperty rx_min_close{get;}
	FloatReactiveProperty rx_max_close{get;}

	bool limit_upper{get; set;}
	bool limit_lower{get; set;}
	IObservable<bool> rx_limit_upper{get;}
	IObservable<bool> rx_limit_lower{get;}
	
	float flat_bonus{get; set;}
	float flat_penalty{get; set;}
	FloatReactiveProperty rx_flat_bonus{get;}
	FloatReactiveProperty rx_flat_penalty{get;}
	
	float range_bonus{get; set;}
	float range_penalty{get; set;}
	FloatReactiveProperty rx_range_bonus{get;}
	FloatReactiveProperty rx_range_penalty{get;}
	
	float epsilon{get; set;}
	
	bool is_close(float value);
	bool is_accept(float value);
	
	bool is_over(float value);
	bool is_over_accept(float value);
	bool is_over_close (float value);
	
	bool is_under(float value);
	bool is_under_accept(float value);
	bool is_under_close (float value);
	
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
	public FloatReactiveProperty rx_target{get{ return _rx_target; }}
	[Show]
	public float target {
		get{ return _rx_target.Value; } 
		set{ _rx_target.Value = value; }
	}

		//Minimum Accepted Value
	[YamlIgnore]
	protected FloatReactiveProperty _rx_min_accept = new FloatReactiveProperty(0.1f);
	[DontSerialize][YamlIgnore]
	public FloatReactiveProperty rx_min_accept{get{ return _rx_min_accept; }}
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
	public FloatReactiveProperty rx_max_accept{get{ return _rx_max_accept; }}
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
	public FloatReactiveProperty rx_min_close{get{ return _rx_min_close; }}
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
	public FloatReactiveProperty rx_max_close{get{ return _rx_max_close; }}
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
	
		//Upper Limit - If false, any value above the target is accepted.
	protected BoolReactiveProperty _rx_limit_upper = new BoolReactiveProperty(false);
	[YamlIgnore]
	public IObservable<bool> rx_limit_upper{get{ return _rx_limit_upper; }}
	[Show]
	public bool limit_upper {
		get{ return _rx_limit_upper.Value; } 
		set{ _rx_limit_upper.Value = value; }
	}
		//Lower Limit - If false, any value below the target is accepted.
	protected BoolReactiveProperty _rx_limit_lower= new BoolReactiveProperty(false);
	[YamlIgnore]
	public IObservable<bool> rx_limit_lower{get{ return _rx_limit_lower; }}
	[Show]
	public bool limit_lower {
		get{ return _rx_limit_lower.Value; } 
		set{ _rx_limit_lower.Value = value; }
	}
	
	//Y Values
		//Flat Bonus
	protected FloatReactiveProperty _rx_flat_bonus = new FloatReactiveProperty(0.0f);
	[YamlIgnore]
	public FloatReactiveProperty rx_flat_bonus{get{ return _rx_flat_bonus; }}
	[Show]
	public float flat_bonus {
		get{ return _rx_flat_bonus.Value; } 
		set{ _rx_flat_bonus.Value = value; }
	}
		//Flat Penalty
	protected FloatReactiveProperty _rx_flat_penalty = new FloatReactiveProperty(0.0f);
	[YamlIgnore]
	public FloatReactiveProperty rx_flat_penalty{get{ return _rx_flat_penalty; }}
	[Show]
	public float flat_penalty {
		get{ return _rx_flat_penalty.Value; } 
		set{ _rx_flat_penalty.Value = value; }
	}
		//Range Bonus
	protected FloatReactiveProperty _rx_range_bonus = new FloatReactiveProperty(0.0f);
	[YamlIgnore]
	public FloatReactiveProperty rx_range_bonus{get{ return _rx_range_bonus; }}
	[Show]
	public float range_bonus {
		get{ return _rx_range_bonus.Value; } 
		set{ _rx_range_bonus.Value = value; }
	}
		//Range Penalty
	protected FloatReactiveProperty _rx_range_penalty = new FloatReactiveProperty(0.0f);
	[YamlIgnore]
	public FloatReactiveProperty rx_range_penalty{get{ return _rx_range_penalty; }}
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
	
	public bool is_over(float value){
		return value > target;
	}
	
	public bool is_over_accept(float value){
		return limit_upper && value > target + max_accept;
	}
	
	public bool is_over_close(float value){
		return limit_upper && value > target + max_close;
	}
	
	public bool is_under(float value){
		return value < target;
	}
	
	public bool is_under_accept(float value){
		return limit_lower && value < target - min_accept;
	}
	
	public bool is_under_close(float value){
		return limit_lower && value < target - min_close;
	}

	public bool is_close(float value){
		return !(is_over_close(value) || is_under_close(value));
	}
	
	public bool is_accept(float value){
		return !(is_over_accept(value) || is_under_accept(value));
	}

	public float evaluate(float value){
		return flat_value(value) + range_value(value);
	}
	public float flat_value(float value){
		if (is_accept(value))
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
		if ( (is_over(value) && !limit_upper) || (is_under(value) && !limit_lower) )
			return range_bonus;
		if (is_close(value)){
			if (is_accept(value)){
				if (is_over(value)){
					return range_bonus*Mathf.Clamp((1.0f + (target - value)/max_accept), 0.0f, 1.0f);
				} else {
					return range_bonus*Mathf.Clamp((value - target + min_accept)/min_accept, 0.0f, 1.0f);
				}
			} else {
				if (is_over(value)){
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
	
	public bool is_disabled(){
		return !(limit_lower || limit_upper);
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