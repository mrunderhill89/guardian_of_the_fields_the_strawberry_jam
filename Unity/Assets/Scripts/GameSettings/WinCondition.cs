using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
namespace GameSettings{
public class WinCondition : IEquatable<WinCondition>
{

  #region Attributes
		public ScoreTarget ripeness {get; set;}
		public ScoreTarget berry_size {get; set;}
		public ScoreTarget basket_weight {get; set;}
		public ScoreTarget distance_covered {get; set;}

		#endregion
		public WinCondition(){
			ripeness = new ScoreTarget();
			berry_size = new ScoreTarget();
			basket_weight = new ScoreTarget();
			distance_covered = new ScoreTarget();
			initialize();
		}

		public WinCondition initialize(){
			ripeness.target = 1.0f;
			ripeness.min_accept = 0.5f;
			ripeness.max_accept = 0.5f;
			ripeness.min_close = 0.6f;
			ripeness.max_close = 0.6f;
			
			berry_size.disable();
			
			basket_weight.target = 16.0f;
			basket_weight.min_accept = 1.0f;
			basket_weight.max_accept = 1.0f;
			basket_weight.min_close = 2.0f;
			basket_weight.max_close = 2.0f;
			basket_weight.flat_bonus = 100.0f;
			basket_weight.flat_penalty = 100.0f;
			basket_weight.range_bonus = 100.0f;
			basket_weight.range_penalty = 100.0f;
			
			distance_covered.target = 500.0f;
			distance_covered.min_accept = 10.0f;
			distance_covered.max_accept = 10.0f;
			distance_covered.min_close = 50.0f;
			distance_covered.max_close = 50.0f;
			distance_covered.limit_upper = false;
			distance_covered.flat_bonus = 0.0f;
			distance_covered.flat_penalty = 300.0f;
			distance_covered.range_bonus = 0.0f;
			distance_covered.range_penalty = 300.0f;

			return this;
		}
		
		public bool is_strawberry_eligible(float ripe, float size){
			return ripeness.is_accept(ripe) && berry_size.is_accept(size);
		}
		
		public float evaluate_strawberry(GameScores.StrawberrySingleScore berry){
			return ripeness.evaluate(berry.ripeness) + berry_size.evaluate(berry.weight);
		}
		
		public float evaluate_basket(GameScores.BasketSingleScore basket){
			return basket_weight.evaluate(basket.weight);
		}
		
		public WinCondition copy_from(WinCondition that){
			ripeness.copy_from(that.ripeness);
			berry_size.copy_from(that.berry_size);
			basket_weight.copy_from(that.basket_weight);
			distance_covered.copy_from(that.distance_covered);
			return this;
		}
		
		public bool Equals (WinCondition that){
			return System.Object.ReferenceEquals(this,that) || (
				ripeness == that.ripeness
				&& berry_size == that.berry_size
				&& basket_weight == that.basket_weight
				&& distance_covered == that.distance_covered
			);
		}
	}
}
