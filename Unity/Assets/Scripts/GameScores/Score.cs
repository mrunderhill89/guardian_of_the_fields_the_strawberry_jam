using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;
namespace GameScores {
	public class Score: IBerryScoreSource, IBasketScoreSource{
		public GameSettings.Model settings { get; set; }
		public StringReactiveProperty rx_player_name = new StringReactiveProperty("");
		public string player_name { 
			get {
				return rx_player_name.Value;
			} 
			set{
				rx_player_name.Value = value;
			} 
		}
		public TimeScore time {get; set;}
		public StrawberryScore berries { get; set;}
		[Show]
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

		public bool finished(){
			return remaining_time() <= 0.0f || accepted_baskets().Count() >= baskets.Count();
		}

		public float total_weight(string category = "gathered"){
			return total_berries(category).Aggregate(0.0f, (sum, berry)=>{
				return sum + berry.weight;
			});
		}
		public float average_weight(string category = "gathered"){
			int count = baskets.Count();
			if (count == 0)
				return 0.0f;
			return total_weight(category) / count;
		}
		public float average_ripeness(string category = "gathered"){
			int count = total_berries("gathered").Count();
			if (count == 0)
				return 0.0f;
			return total_berries(category).Aggregate(0.0f, (sum, berry)=>{
				return sum + berry.ripeness;
			}) / count;
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
			player_name = that.player_name;
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