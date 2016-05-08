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
	
	public string category = "Gathered";
	void Start(){
		views.ObserveRemove ().Subscribe ((evn) => {
			Destroy(evn.Value);
		});
		source.rx_score.Subscribe((score)=>{
			foreach(GameObject view in views){
				Destroy(view);
			}
			views.Clear();
			if (score != null){
				//Get current icons
				views.AddRange(score.berries.categories[category].rx_berries.Select((berry)=>{
					return create_icon(berry, score.settings.win_condition);
				}));
			}
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
	
	void OnDestroy(){
		views.Clear();
	}
}
