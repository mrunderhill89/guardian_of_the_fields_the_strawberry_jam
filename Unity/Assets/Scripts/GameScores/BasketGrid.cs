using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using GameScores;
using UniRx;

public class BasketGrid : BetterBehaviour {
	public IScoreSource source;
	public Transform place_icons_here;
	public ReactiveCollection<GameObject> views = new ReactiveCollection<GameObject>();
	[Show]
	public string ScorePrefab = "BasketIcon";
	protected GameObject _prefab = null;
	public GameObject prefab{
		get{
			if (_prefab == null){
				_prefab = Resources.Load(ScorePrefab) as GameObject;
			}
			return _prefab;
		}
	}
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
				views.AddRange(score.baskets.rx_baskets.Select((basket)=>{
					return create_icon(basket, score.settings.win_condition);
				}));
				score.baskets.rx_baskets.ObserveAdd().Subscribe((evn)=>{
					views.Add(create_icon(evn.Value, score.settings.win_condition));
				});
				score.baskets.rx_baskets.ObserveRemove().Subscribe((evn)=>{
					views.RemoveAt(evn.Index);
				});
			}
		});
	}
	
	GameObject create_icon(BasketSingleScore _score, GameSettings.WinCondition _win){
		GameObject obj = GameObject.Instantiate(prefab);
		obj.GetComponent<BasketIcon>().chain_score(_score).chain_win(_win);
		obj.transform.SetParent(place_icons_here, false);
		return obj;
	}
	
	void OnDestroy(){
		views.Clear();
	}
}
