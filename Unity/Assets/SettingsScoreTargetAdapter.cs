﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Vexe.Runtime.Types;

public class SettingsScoreTargetAdapter : BetterBehaviour {
	public GameSettingsComponent settings_component;
	public IScoreTargetUI ripeness;
	public IScoreTargetUI berry_weight;
	public IScoreTargetUI basket_weight;
	public IScoreTargetUI distance;

	// Use this for initialization
	void Start () {
		settings_component.rx_current_rules.Subscribe(rules=>{
			if (rules != null){
				if (ripeness != null)
					ripeness.data = rules.win_condition.ripeness;
				if (berry_weight != null)
					berry_weight.data = rules.win_condition.berry_size;
				if (basket_weight != null)
					basket_weight.data = rules.win_condition.basket_weight;
				if (distance != null)
					distance.data = rules.win_condition.distance_covered;
			}
		});
	}
}