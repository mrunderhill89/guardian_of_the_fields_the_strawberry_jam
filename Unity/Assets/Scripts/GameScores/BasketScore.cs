using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;
namespace GameScores
{
	public class BasketSingleScore{
		[DontSerialize]
		public FloatReactiveProperty rx_weight = new FloatReactiveProperty(0.0f);
		[Show]
		public float weight {
			get {
				return rx_weight.Value;
			} 
			set{
				rx_weight.Value = value;
			}
		}
		public BasketSingleScore chain_weight(float value){
			weight = value;
			return this;
		}
		[DontSerialize]
		public IntReactiveProperty rx_count = new IntReactiveProperty(0);
		[Show]
		public int count {
			get {
				return rx_count.Value;
			} 
			set{
				rx_count.Value = value;
			}
		}
		public BasketSingleScore chain_count(int value){
			count = value;
			return this;
		}
		[DontSerialize]
		public IntReactiveProperty rx_id = new IntReactiveProperty(1);
		[Show]
		public int id {
			get {
				return rx_id.Value;
			} 
			set{
				rx_id.Value = value;
			}
		}
		public BasketSingleScore chain_id(int value){
			id = value;
			return this;
		}
		[DontSerialize]
		public BoolReactiveProperty rx_overflow = new BoolReactiveProperty(false);
		[Show]
		public bool is_overflow {
			get {
				return rx_overflow.Value;
			} 
			set {
				rx_overflow.Value = value;
			}
		}
		public BasketSingleScore chain_overflow(bool value){
			is_overflow = value;
			return this;
		}

		public bool is_overweight(float max_weight){
			return weight > max_weight;
		}
		public bool is_underweight(float min_weight){
			return weight < min_weight;
		}
		public bool is_eligible(float max_weight, float min_weight){
			return !(is_overflow || is_overweight (max_weight) || is_underweight (min_weight));
		}

		public BasketSingleScore(){
			weight = 0.0f;
			count = 0;
			is_overflow = false;
		}

		public BasketSingleScore copy_from(BasketSingleScore that){
			id = that.id;
			weight = that.weight;
			count = that.count;
			is_overflow = that.is_overflow;
			return this;
		}

		public BasketSingleScore copy_of(){
			return new BasketSingleScore ().copy_from (this);
		}
	}
	public class BasketScore
	{
		public ReactiveCollection<BasketSingleScore> rx_baskets = new ReactiveCollection<BasketSingleScore>();
		[Show]
		public List<BasketSingleScore> baskets{
			get{ return rx_baskets.ToList(); }
			set{
				rx_baskets.SetRange(value);
			}
		}

		public IEnumerable<BasketSingleScore> accepted(GameSettings.WinCondition win){
			return baskets.Where (basket=>basket.is_eligible(win.max_basket_weight, win.min_basket_weight));
		}
		public IEnumerable<BasketSingleScore> overweight(GameSettings.WinCondition win){
			return baskets.Where (basket=>basket.is_overweight(win.max_basket_weight));
		}
		public IEnumerable<BasketSingleScore> underweight(GameSettings.WinCondition win){
			return baskets.Where (basket=>basket.is_underweight(win.min_basket_weight));
		}
		public IEnumerable<BasketSingleScore> overflow(GameSettings.WinCondition win){
			return baskets.Where (basket=>basket.is_overflow);
		}

		
		public int Count(){
			return rx_baskets.Count;
		}

		public BasketScore copy_from(BasketScore that){
			rx_baskets.SetRange (that.rx_baskets.Select (basket => basket.copy_of ()));
			return this;
		}
		public BasketScore copy_of(){
			return new BasketScore ().copy_from (this);
		}
	}
}

