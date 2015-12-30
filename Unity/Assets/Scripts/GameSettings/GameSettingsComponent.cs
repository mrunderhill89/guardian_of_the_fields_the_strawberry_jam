using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using GameSettings;
using UnityEngine;
using UniRx;
using Vexe.Runtime.Types;

public class GameSettingsComponent : BetterBehaviour
{
  #region Attributes
	[DontSerialize]
	public static ReactiveProperty<Model> rx_working_rules = new ReactiveProperty<Model>();
	[Show]
	public static Model working_rules{
		get{
			if (rx_working_rules.Value == null)
				rx_working_rules.Value = Model.import_static();
			return rx_working_rules.Value;
		}
		private set {rx_working_rules.Value = value;}
	}
	[DontSerialize]
	public ReactiveProperty<Model> rx_current_rules = new ReactiveProperty<Model>();
	[Show]
	public Model current_rules{
		get{
			if (rx_current_rules.Value == null){
				current_rules = working_rules;
			}
			return rx_current_rules.Value;
		}
		private set {rx_current_rules.Value = value;}
	}

	[Show]
	public bool is_working_rules{
		get{ return current_rules == working_rules;}
	}

	[Show]
	public GameSettingsComponent import(string filename = ""){
		current_rules = Model.import_static(filename);
		return this;
	}

	[Show]
	public void apply(){
		working_rules.copy_from (current_rules);
	}
	[Show]
	public void revert(){
		current_rules.copy_from (working_rules);
	}
	[Show]
	public GameSettingsComponent export(string filename = ""){
		current_rules.export (filename);
		return this;
	}
	
	public ReactiveProperty<bool> rx_is_working_rules;
	private static IDisposable sync_seed;
	void Awake(){
		//Synchronize Unity's random number seed with the working ruleset's.
		if (sync_seed == null){
			sync_seed = rx_working_rules.SelectMany((rules)=>{
				if (rules == null)
					return Observable.Never<UniRx.Tuple<Model,bool>>();
				return rules.randomness.rx_randomize.Select((randomize)=>{
					return new UniRx.Tuple<Model,bool>(rules, randomize);
				});
			}).ObserveOnMainThread().Subscribe((tuple)=>{
				Model rules = tuple.Item1;
				bool randomize = tuple.Item2;
				if (randomize){
					rules.randomness.seed = UnityEngine.Random.seed;
				}
			});
		}
		rx_working_rules.Value = Model.import_static();
		current_rules = working_rules.copy_of();
		rx_is_working_rules = new ReactiveProperty<bool>();
	}
	
	void Update(){
		rx_is_working_rules.Value = (working_rules == current_rules);
	}
	#endregion


}

