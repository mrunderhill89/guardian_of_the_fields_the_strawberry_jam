using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using GameScores;
using UniRx;

public class ScoreStrawberrySingleView : BetterBehaviour {
	protected ReactiveProperty<StrawberrySingleScore> rx_strawberry
		= new ReactiveProperty<StrawberrySingleScore>();

	public StrawberrySingleScore strawberry{
		get{ return rx_strawberry.Value; }
		set{ rx_strawberry.Value = value;}
	}
	
	public GameSettings.WinCondition win {get;set;}
	
	public ObjectVisibility vis;
	public Image color_icon;
	public Text weight_text;
	public Image status_icon;
	public Text flat_score_text;
	public Text range_score_text;
	public Text total_score_text;
	public TooltipBroadcast tooltip;
	
	public static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
	public static Dictionary<string, string> language_keys = new Dictionary<string, string>();
	
	[DontSerialize]
	public ReadOnlyReactiveProperty<float> rx_ripeness;
	[Show]
	public float ripeness{
		get{
			if (rx_ripeness != null)
				return rx_ripeness.Value;
			return 0.0f;
		}
	}
	
	[DontSerialize]
	public ReadOnlyReactiveProperty<float> rx_weight;
	[Show]
	public float weight{
		get{
			if (rx_weight != null)
				return rx_weight.Value;
			return 0.0f;
		}
	}
	
	void Start(){
		rx_ripeness = rx_strawberry.Select(berry=>berry.ripeness).ToReadOnlyReactiveProperty<float>();
		rx_weight = rx_strawberry.Select(berry=>berry.weight).ToReadOnlyReactiveProperty<float>();
		rx_strawberry.Subscribe((berry) => {
			if (berry != null && win != null){
				vis.visible = true;
				color_icon.color = StrawberryColor.get_color(berry.ripeness);
				weight_text.text = berry.weight.ToString("0.00");
				
				ScoreTarget ripeness = win.ripeness;
				ScoreTarget berry_weight = win.berry_size;
				
				//Status Icon
				if (ripeness.is_accept(berry.ripeness)){
					if (berry_weight.is_accept(berry.weight)){
						status_icon.sprite = sprites["Accepted"];
					} else {
						status_icon.sprite = sprites["Undersize"];
					}
				} else {
					if (ripeness.is_over(berry.ripeness)){
						status_icon.sprite = sprites["Overripe"];
					} else {
						status_icon.sprite = sprites["Underripe"];
					}
				}
				
				//Score Views
				float flat_score = ripeness.flat_value(berry.ripeness) 
					+ berry_weight.flat_value(berry.weight);
				float range_score = ripeness.range_value(berry.ripeness) 
					+ berry_weight.range_value(berry.weight); 
				float total_score = flat_score + range_score;

				ScoreDetailedForm.format_score_text(flat_score_text, flat_score);
				ScoreDetailedForm.format_score_text(range_score_text, range_score);
				ScoreDetailedForm.format_score_text(total_score_text, total_score);
			} else {
				//Hide the view if we don't have all the necessary data.
				vis.visible = false;
			}
		});
	}
}