﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using GameScores;
using UniRx;

public class BasketIcon : BetterBehaviour, IPointerOrMouseEnterHandler, IPointerOrMouseExitHandler {
	[DontSerialize]
	public ReactiveProperty<BasketSingleScore> rx_score = new ReactiveProperty<BasketSingleScore>();
	[Show]
	public BasketSingleScore score{
		get{ return rx_score.Value; }
		set{ rx_score.Value = value;}
	}
	[DontSerialize]
	public ReactiveProperty<GameSettings.WinCondition> rx_win = new ReactiveProperty<GameSettings.WinCondition>();
	[Show]
	public GameSettings.WinCondition win{
		get{ return rx_win.Value;}
		set{ rx_win.Value = value;}
	}

	public SpriteRenderer icon_3d;
	public Image icon;
	
	public BasketComponent component;
	public Text count;
	public TextMesh count_3d;
	public Text weight;
	public TextMesh weight_3d;
	public Text id_text;
	public TextMesh id_text_3d;
	
	public Text flat_score_text;
	public Text range_score_text;
	public Text total_score_text;
	
	public TooltipBroadcast tooltip;
	public ObjectOpacity opacity;
	public float off_opacity = 0.8f;
	public float over_opacity = 0.0f;

	[DontSerialize]
	public ReadOnlyReactiveProperty<Sprite> rx_sprite;	
	[DontSerialize]
	public ReadOnlyReactiveProperty<String> rx_id_text;
	[DontSerialize]
	public ReadOnlyReactiveProperty<String> rx_count_text;
	[DontSerialize]
	public ReadOnlyReactiveProperty<String> rx_weight_text;
	[DontSerialize]
	public ReadOnlyReactiveProperty<String> rx_tooltip_text;
	
	public enum BasketCondition{
		Accepted,
		Overweight,
		Underweight,
		Overflow
	}
	public static Dictionary<BasketCondition, Sprite> sprites = new Dictionary<BasketCondition, Sprite>();
	public static Dictionary<BasketCondition, string> language_keys = new Dictionary<BasketCondition, string>();
	ReadOnlyReactiveProperty<BasketCondition> condition;

	IObservable<UniRx.Tuple<BasketSingleScore, GameSettings.WinCondition>> rx_with_win;
	void Start () {
		if (component != null)
			score = component.score_data;
		if (win == null)
			win = GameSettingsComponent.working_rules.win_condition;
			
		if (id_text != null || id_text_3d != null){
			rx_id_text = LanguageController.controller.rx_load_text("basket_single")
				.CombineLatest(rx_score.SelectMany(rx_score.SelectMany(s=>s.rx_id)), (text, id)=>{
					return text+" #"+id.ToString();
				}).ToReadOnlyReactiveProperty<string>();
			rx_id_text.Subscribe((text)=>{
				if (id_text != null)
					id_text.text = text;
				if (id_text_3d != null)
					id_text_3d.text = text;
			});
		}
		
		if (count != null || count_3d != null){
			rx_count_text = rx_score
				.SelectMany(s=>s.rx_count)
				.Select(value=>value.ToString())
				.ToReadOnlyReactiveProperty<string>();
			rx_count_text.Subscribe((text)=>{
				if (count != null)
					count.text = text;
				if (count_3d != null)
					count_3d.text = text;
			});
		}
		
		if (weight != null || weight_3d != null){
			rx_weight_text = rx_score
				.SelectMany(s=>s.rx_weight)
				.Select(value=>value.ToString("0.00"))
				.ToReadOnlyReactiveProperty<string>();
			rx_weight_text.Subscribe((text)=>{
				if (weight != null)
					weight.text = text;
				if (weight_3d != null)
					weight_3d.text = text;
			});
		}
		
		condition = rx_score.SelectMany(s=>{
			return s.rx_weight.CombineLatest(s.rx_overflow, (w,overflow)=>{
				return new UniRx.Tuple<float,bool>(w,overflow);
			});
		}).CombineLatest (rx_win, (tuple, wc) => {
			float w = tuple.Item1;
			bool overflow = tuple.Item2;
			if (overflow){
				return BasketCondition.Overflow;
			}
			if (wc.basket_weight.is_accept(w)){
				return BasketCondition.Accepted;
			} else {
				if (wc.basket_weight.is_over(w)){
					return BasketCondition.Overweight;
				} else {
					return BasketCondition.Underweight;
				}
			}
		}).ToReadOnlyReactiveProperty<BasketCondition>();
		
		rx_with_win = rx_score.CombineLatest(rx_win, (s, w)=>{
			return new UniRx.Tuple<BasketSingleScore, GameSettings.WinCondition>(s,w);
		});
		rx_with_win.Subscribe((tuple)=>{
			if (flat_score_text != null){
				ScoreDetailedForm.format_score_text(flat_score_text, tuple.Item2.evaluate_basket_flat(tuple.Item1));
			}
			if (range_score_text != null){
				ScoreDetailedForm.format_score_text(range_score_text, tuple.Item2.evaluate_basket_range(tuple.Item1));
			}
			if (total_score_text != null){
				ScoreDetailedForm.format_score_text(total_score_text, tuple.Item2.evaluate_basket(tuple.Item1));
			}
		});
		
		rx_tooltip_text = condition.Select(c=>language_keys[c]).ToReadOnlyReactiveProperty<string>();
		rx_tooltip_text.Subscribe((key)=>{
			if (tooltip != null){
				tooltip.key = key;
			}
		});
		
		rx_sprite = condition.Select(c=>sprites[c]).ToReadOnlyReactiveProperty<Sprite>();
		rx_sprite.Subscribe ((sprite) => {
			if (icon != null)
				icon.sprite = sprite;
			if (icon_3d != null)
				icon_3d.sprite = sprite;
		});
		
		if (opacity != null){
			opacity.opacity = off_opacity;
		}
	}
	
	public BasketIcon chain_score(BasketSingleScore _score){
		score = _score;
		return this;
	}
	public BasketIcon chain_win(GameSettings.WinCondition _win){
		win = _win;
		return this;
	}

	public void fade_in(){
		if (opacity != null){
			opacity.target_opacity = over_opacity;
		}
	}
	
	public void fade_out(){
		if (opacity != null){
			opacity.target_opacity = off_opacity;
		}
	}

	public void OnPointerEnter(PointerEventData evn){
		fade_in();
	}

	public void OnPointerExit(PointerEventData evn){
		fade_out();
	}

	public void OnMouseEnter(){
		fade_in();
	}
	public void OnMouseExit(){
		fade_out();
	}
}
