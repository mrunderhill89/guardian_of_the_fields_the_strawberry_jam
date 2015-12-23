using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
namespace GameScores {
	public class Score: IBerryScoreSource, IBasketScoreSource{
		public GameSettings.Model settings { get; set; }
		public TimeScore time {get; set;}
		public StrawberryScore berries { get; set;}
		public BasketScore baskets { get; set; }

		public Score(){
			settings = new GameSettings.Model ();
			time = new TimeScore ();
			berries = new StrawberryScore ();
			baskets = new BasketScore ();
		}

		public float remaining_time(){
			return settings.time.game_length - time.played_for;
		}

		public IEnumerable<StrawberrySingleScore> ripe_berries(string category){
			return berries.get_category(category).ripe(settings.win_condition);
		}
		public IEnumerable<StrawberrySingleScore> overripe_berries(string category){
			return berries.get_category(category).overripe(settings.win_condition);
		}
		public IEnumerable<StrawberrySingleScore> underripe_berries(string category){
			return berries.get_category(category).underripe(settings.win_condition);
		}
		public IEnumerable<StrawberrySingleScore> underweight_berries(string category){
			return berries.get_category(category).underweight(settings.win_condition);
		}
		public IEnumerable<StrawberrySingleScore> total_berries(string category){
			return berries.get_category (category).all_berries;
		}

		public IEnumerable<BasketSingleScore> accepted_baskets(){
			return baskets.accepted (settings.win_condition);
		}
		public IEnumerable<BasketSingleScore> overweight_baskets(){
			return baskets.overweight (settings.win_condition);
		}
		public IEnumerable<BasketSingleScore> underweight_baskets(){
			return baskets.underweight (settings.win_condition);
		}
		public IEnumerable<BasketSingleScore> overflow_baskets(){
			return baskets.overflow (settings.win_condition);
		}

		
		public Score copy_from(Score that){
			settings.copy_from(that.settings);
			time.copy_from (that.time);
			berries.copy_from (that.berries);
			baskets.copy_from (that.baskets);
			return this;
		}

		public Score copy_of(){
			return new Score ().copy_from (this);
		}
	}
}