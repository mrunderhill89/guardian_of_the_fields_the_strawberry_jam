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
			if (rx_working_rules.Value == null){
				rx_working_rules.Value = SaveLoadSelector.get_settings_loader().load("Default");
			}
			return rx_working_rules.Value;
		}
		private set {rx_working_rules.Value = value;}
	}

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
	}

	#endregion


}

