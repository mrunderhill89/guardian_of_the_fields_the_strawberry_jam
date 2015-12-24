using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

namespace GameScores
{
	public class StrawberrySingleScore{
		public float weight { get; set; }
		public float ripeness { get; set;}
		public StrawberrySingleScore chain_weight(float value){
			weight = value;
			return this;
		}
		public StrawberrySingleScore chain_ripeness(float value){
			ripeness = value;
			return this;
		}
		public bool is_overripe(float max_ripeness){
			return ripeness > max_ripeness;
		}
		public bool is_underripe(float min_ripeness){
			return ripeness < min_ripeness;
		}
		public bool is_underweight(float min_weight){
			return weight < min_weight;
		}
		public bool is_eligible(float max_ripeness, float min_ripeness, float min_weight){
			return !(is_overripe (max_ripeness) || is_underripe (min_ripeness) || is_underweight (min_weight));
		}

		public StrawberrySingleScore copy_from(StrawberrySingleScore that){
			ripeness = that.ripeness;
			weight = that.weight;
			return this;
		}
		public StrawberrySingleScore copy_of(){
			return new StrawberrySingleScore().copy_from(this);
		}

	}
	public class StrawberryScore
	{

		public class RipenessSorter
		{
			public ReactiveCollection<StrawberrySingleScore> rx_berries
				= new ReactiveCollection<StrawberrySingleScore>();
			public List<StrawberrySingleScore> all_berries{
				get{ return rx_berries.ToList();}
				set{ rx_berries.SetRange(value);}
			}
			public IEnumerable<StrawberrySingleScore> ripe(GameSettings.WinCondition win){
				return all_berries.Where(berry => berry.is_eligible(win.max_ripeness, win.min_ripeness, win.min_size));
			}
			public IEnumerable<StrawberrySingleScore> overripe(GameSettings.WinCondition win){
				return all_berries.Where(berry => berry.is_overripe(win.max_ripeness));
			}
			public IEnumerable<StrawberrySingleScore> underripe(GameSettings.WinCondition win){
				return all_berries.Where(berry => berry.is_underripe(win.min_ripeness));
			}
			public IEnumerable<StrawberrySingleScore> underweight(GameSettings.WinCondition win){
				return all_berries.Where(berry => berry.is_underweight(win.min_size));
			}
			[Show]
			public RipenessSorter from_state_machine(string state_name){
				if (StrawberryStateMachine.main != null) {
					rx_berries.SetRange(
						StrawberryStateMachine.main.get_strawberries(state_name).Select(comp=>
							new StrawberrySingleScore().chain_ripeness(comp.quality).chain_weight(comp.weight)
						)
					);
				}
				return this;
			}
			public RipenessSorter copy_from(RipenessSorter that){
				rx_berries.SetRange (that.all_berries.Select (score => score.copy_of ()));
				return this;
			}
			public RipenessSorter copy_of(){
				return new RipenessSorter().copy_from(this);
			}
		}
		protected Dictionary<string, RipenessSorter> _categories
			= new Dictionary<string, RipenessSorter>();
		public Dictionary<string, RipenessSorter> categories{
			get{ return _categories;}
			set{ _categories = value;}
		}
		public RipenessSorter get_category(string name){
			if (!categories.ContainsKey (name))
				categories [name] = new RipenessSorter ();
			return categories [name];
		}
		public StrawberryScore copy_from(StrawberryScore that){
			categories.Clear ();
			foreach (KeyValuePair<string,RipenessSorter> kvp in that.categories) {
				categories[kvp.Key] = kvp.Value.copy_of();
			}
			return this;
		}
		public StrawberryScore copy_of(){
			return new StrawberryScore().copy_from(this);
		}
	}
}
