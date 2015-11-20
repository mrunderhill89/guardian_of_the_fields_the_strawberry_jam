using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.RepresentationModel;

public class ScoreHandler : BetterBehaviour {
	public StrawberryStateMachine berry_state;
	public class TotalScore{
		public Dictionary<string, StrawberryScore> strawberries = 
			new Dictionary<string, StrawberryScore>();
		public BasketScore baskets = new BasketScore();
		public TotalScore(){
			strawberries["fall"] = new StrawberryScore();
			strawberries["basket"] = new StrawberryScore();
		}
	}
	public class StrawberryScore{
		public int ripe = 0;
		public int overripe = 0;
		public int underripe = 0;
		public int undersize = 0;
		public void reset(){
			ripe = 0;
			overripe = 0;
			underripe = 0;
			undersize = 0;
		}
		public StrawberryScore get_from_state(StrawberryStateMachine berry_state, string state_name){
			reset();
			foreach (StrawberryComponent berry in berry_state.get_strawberries(state_name)){
				if (berry.is_under_size()){undersize++;}
				if (berry.is_over_ripe()){
					overripe++;
				} else if (berry.is_under_ripe()){
					underripe++;
				} else if (!berry.is_under_size()){
					ripe++;
				}
			}
			return this;
		}
	}
	public class BasketScore{
		public int accepted = 0;
		public int overweight = 0;
		public int underweight = 0;
		public int overflow = 0;
		public void reset(){
			accepted = 0;
			overweight = 0;
			underweight = 0;
			overflow = 0;
		}
		public BasketScore get_from_current(){
			reset();
			foreach(BasketComponent basket in BasketComponent.baskets){
				if (basket.is_overflow()){
					overflow++;
				}
				if (basket.is_overweight()){overweight++;} 
				else if (basket.is_underweight()){underweight++;}
				else if (!basket.is_overflow()){accepted++;}
			}
			return this;
		}
	}
	public TotalScore current_score = new TotalScore();
	public bool lock_strawberries = false;
	public bool lock_baskets = false;
	
	void Update(){
		if (!lock_strawberries){
			current_score.strawberries["fall"].get_from_state(berry_state, "fall");
			current_score.strawberries["basket"].get_from_state(berry_state, "basket");
		}
		if (!lock_baskets){
			current_score.baskets.get_from_current();
		}
	}
	/*
	public class BasketScoreData{
		public int accepted = 0;
		public int overweight = 0;
		public int underweight = 0;
		public int overflow = 0;
		public void reset(){
			accepted = 0;
			overweight = 0;
			underweight = 0;
			overflow = 0;
		}
	}
	protected static BasketScoreData saved_score = new BasketScoreData();
	public static bool lock_scores = false;
	
	public static BasketScoreData current_score{
		get{
			if (!lock_scores){
				saved_score.reset();
				foreach(BasketComponent basket in BasketComponent.baskets){
					if (basket.is_overflow()){
						saved_score.overflow++;
					}
					if (basket.is_overweight()){
						saved_score.overweight++;
					} else if (basket.is_underweight()){
						saved_score.underweight++;
					} else if (!basket.is_overflow()){
						saved_score.accepted++;
					}
				}
			}
			return saved_score;
		}
	}
	
		public class StrawberryScoreData{
		public int ripe = 0;
		public int overripe = 0;
		public int underripe = 0;
		public int undersize = 0;
		public void reset(){
			ripe = 0;
			overripe = 0;
			underripe = 0;
			undersize = 0;
		}
	}

	public bool lock_scores = false;
	[Show]
	public StrawberryScoreData gathered{
		get{
			return get_score_data("basket");
		}
	}
	[Show]
	public StrawberryScoreData dropped{
		get{
			return get_score_data("fall");
		}
	}
*/
}
