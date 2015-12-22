using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using GameScores;
using UniRx;

public class BasketIcon : BetterBehaviour {
	public ReactiveProperty<BasketSingleScore> rx_score = new ReactiveProperty<BasketSingleScore>();
	public BasketSingleScore score{
		get{ return rx_score.Value; }
		set{ rx_score.Value = value;}
	}
	public ReactiveProperty<GameSettings.WinCondition> rx_win = new ReactiveProperty<GameSettings.WinCondition>();
	public GameSettings.WinCondition win{
		get{ return rx_win.Value;}
		set{ rx_win.Value = value;}
	}

	protected ReadOnlyReactiveProperty<Sprite> rx_sprite;

	public Image icon;
	public static Sprite accepted;
	public static Sprite overweight;
	public static Sprite underweight;
	public static Sprite overflow;

	public Text count;
	public Text weight;
	public TooltipBroadcast tooltip;
	void Start () {
		rx_score.Subscribe ((basket) => {
			count.text = basket.count.ToString();
			weight.text = basket.weight.ToString("0.00");
		});
		rx_sprite = rx_score.CombineLatest (rx_win, (basket, win) => {
			if (basket.is_overflow){
				tooltip.key = "basket_overflow";
				return overflow;
			}
			if (basket.is_overweight(win.max_basket_weight)){
				tooltip.key = "basket_overweight";
				return overweight;
			}
			if (basket.is_underweight(win.min_basket_weight)){
				tooltip.key = "basket_underweight";
				return underweight;
			}
			tooltip.key = "basket_accepted";
			return accepted;
		}).ToReadOnlyReactiveProperty<Sprite>();
		rx_sprite.Subscribe ((sprite) => {
			icon.sprite = sprite;
		});
	}
}
