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
				return all_berries.Where(berry => win.is_strawberry_eligible(berry.ripeness, berry.weight));
			}
			public IEnumerable<StrawberrySingleScore> overripe(GameSettings.WinCondition win){
				return all_berries.Where(berry => win.ripeness.is_over_accept(berry.ripeness));
			}
			public IEnumerable<StrawberrySingleScore> underripe(GameSettings.WinCondition win){
				return all_berries.Where(berry =>win.ripeness.is_under_accept(berry.ripeness));
			}
			public IEnumerable<StrawberrySingleScore> underweight(GameSettings.WinCondition win){
				return all_berries.Where(berry => win.berry_size.is_under_accept(berry.ripeness));
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

