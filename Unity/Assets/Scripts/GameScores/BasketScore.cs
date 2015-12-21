using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

namespace GameScores
{
	public class BasketSingleScore{
		[Show]
		public float weight {get; set;}
		public BasketSingleScore chain_weight(float value){
			weight = value;
			return this;
		}
		[Show]
		public bool is_overflow {get; set;}
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
			is_overflow = false;
		}

		public BasketSingleScore copy_from(BasketSingleScore that){
			weight = that.weight;
			is_overflow = that.is_overflow;
			return this;
		}

		public BasketSingleScore copy_of(){
			return new BasketSingleScore ().copy_from (this);
		}
	}
	public class BasketScore
	{
		public List<BasketSingleScore> baskets = new List<BasketSingleScore>();
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

		public BasketScore copy_from(BasketScore that){
			baskets = that.baskets.Select(basket => basket.copy_of()).ToList();
			return this;
		}
		public BasketScore copy_of(){
			return new BasketScore ().copy_from (this);
		}
	}
}

