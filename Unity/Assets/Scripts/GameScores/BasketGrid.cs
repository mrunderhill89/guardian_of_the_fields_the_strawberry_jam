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
	public static string ScorePrefab = "BasketIcon";
	protected static GameObject _prefab = null;
	public static GameObject prefab{
		get{
			if (_prefab == null){
				_prefab = Resources.Load(ScorePrefab) as GameObject;
			}
			return _prefab;
		}
	}
	void Start(){
		source.rx_score.Subscribe ((score) => {
			views.Clear ();
			if (score != null){
				views.AddRange(score.baskets.baskets.Select((basket)=>{
					GameObject obj = GameObject.Instantiate(prefab);
					obj.GetComponent<BasketIcon>().score = basket;
					obj.GetComponent<BasketIcon>().win = score.settings.win_condition;
					obj.transform.SetParent(place_icons_here,false);
					return obj;
				}));
			}
		});
		views.ObserveRemove ().Subscribe ((evn) => {
			Destroy(evn.Value);
		});
	}
	void OnDestroy(){
		views.Clear();
	}
}
