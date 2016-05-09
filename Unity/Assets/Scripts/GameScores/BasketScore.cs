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
		
		public float flat_score(ScoreTarget weight_target){
			if (is_overflow){
				//Overflowing baskets are automatic fails.
				return weight_target.flat_penalty;
			}
			return weight_target.flat_value(weight);
		}

		public float range_score(ScoreTarget weight_target){
			if (is_overflow){
				//Overflowing baskets are automatic fails.
				return weight_target.range_penalty;
			}
			return weight_target.range_value(weight);
		}
		
		public float total_score(ScoreTarget weight_target){
			return flat_score(weight_target) + range_score(weight_target);
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
			return baskets.Where (basket=>win.basket_weight.is_accept(basket.weight));
		}
		public IEnumerable<BasketSingleScore> overweight(GameSettings.WinCondition win){
			return baskets.Where (basket=>win.basket_weight.is_over(basket.weight));
		}
		public IEnumerable<BasketSingleScore> underweight(GameSettings.WinCondition win){
			return baskets.Where (basket=>win.basket_weight.is_under(basket.weight));
		}
		public IEnumerable<BasketSingleScore> overflow(GameSettings.WinCondition win){
			return baskets.Where (basket=>basket.is_overflow);
		}

		public float flat_score(GameSettings.WinCondition win){
			return baskets.Aggregate(0.0f, (sum, basket)=>sum+basket.flat_score(win.basket_weight));
		}
		
		public float range_score(GameSettings.WinCondition win){
			return baskets.Aggregate(0.0f, (sum, basket)=>sum+basket.range_score(win.basket_weight));
		}

		public float total_score(GameSettings.WinCondition win){
			return baskets.Aggregate(0.0f, (sum, basket)=>sum+basket.total_score(win.basket_weight));
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

