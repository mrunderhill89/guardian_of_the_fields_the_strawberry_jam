using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using GameScores;
using UniRx;

public class ScoreStrawberryView : BetterBehaviour {
	public IScoreSource source;
	
	public ReactiveCollection<GameObject> views = new ReactiveCollection<GameObject>();
	
	[Show]
	public static string prefab_name = "ScoreSingleStrawberryView";
	protected static GameObject _prefab = null;
	[Show]
	public static GameObject prefab{
		get{
			if (_prefab == null){
				_prefab = Resources.Load(prefab_name) as GameObject;
			}
			return _prefab;
		}
	}
	
	public Transform place_icons_here;	
	
	public Text ripe_flat_text;
	public Text ripe_range_text;
	public Text ripe_total_text;

	public Text overripe_flat_text;
	public Text overripe_range_text;
	public Text overripe_total_text;

	public Text underripe_flat_text;
	public Text underripe_range_text;
	public Text underripe_total_text;

	public Text undersize_flat_text;
	public Text undersize_range_text;
	public Text undersize_total_text;

	public Text all_flat_text;
	public Text all_range_text;
	public Text all_total_text;
	
	public bool reverse = false;
	
	public string category = "Gathered";
	IDisposable source_sub;
	void Start(){
		views.ObserveRemove ().Subscribe ((evn) => {
			Destroy(evn.Value);
		});
		source_sub = source.rx_score.Subscribe((score)=>{
			foreach(GameObject view in views){
				Destroy(view);
			}
			views.Clear();
			if (score != null){
				GameSettings.WinCondition win = score.settings.win_condition;
				//Get current icons
				views.AddRange(score.berries.categories[category].rx_berries.Select((berry)=>{
					return create_icon(berry, win);
				}));
				
				//Update totals
				update_totals(score.berries.categories[category].ripe(win), win, ripe_flat_text, ripe_range_text, ripe_total_text);
				update_totals(score.berries.categories[category].overripe(win), win, overripe_flat_text, overripe_range_text, overripe_total_text);
				update_totals(score.berries.categories[category].underripe(win), win, underripe_flat_text, underripe_range_text, underripe_total_text);
				update_totals(score.berries.categories[category].underweight(win), win, undersize_flat_text, undersize_range_text, undersize_total_text);
				update_totals(score.berries.categories[category].all_berries, win, all_flat_text, all_range_text, all_total_text);
			};
		});
	}
	
	GameObject create_icon(StrawberrySingleScore _score, GameSettings.WinCondition _win){
		GameObject obj = GameObject.Instantiate(prefab);
		var component = obj.GetComponent<ScoreStrawberrySingleView>();
		component.strawberry = _score;
		component.win = _win;
		obj.transform.SetParent(place_icons_here, false);
		return obj;
	}
	
	void update_totals(IEnumerable<StrawberrySingleScore> scores, GameSettings.WinCondition win, Text flat_text, Text range_text, Text total_text){
		var data = scores.Aggregate(new float[]{0.0f,0.0f,0.0f}, (sum, berry)=>{
			sum[0] += win.evaluate_strawberry_flat(berry);
			sum[1] += win.evaluate_strawberry_range(berry);
			sum[2] += win.evaluate_strawberry(berry);
			return sum;
		});
		if (reverse){
			data[0] *= -1.0f;
			data[1] *= -1.0f;
			data[2] *= -1.0f;
		}
		ScoreDetailedForm.format_score_text(flat_text, data[0]);
		ScoreDetailedForm.format_score_text(range_text, data[1]);
		ScoreDetailedForm.format_score_text(total_text, data[2]);
	}
	
	void OnDestroy(){
		views.Clear();
		if (source_sub != null)
			source_sub.Dispose();
	}
}
