using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace GameScores {
	public class Score{
		public GameSettings.Model settings;
		public TimeScore time = new TimeScore();
		public StrawberryScore berries = new StrawberryScore();
		public BasketScore baskets = new BasketScore();

		public Score(){
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

	}
}