using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class ScoreboardV2 : BetterBehaviour {
	public RectTransform Place;
	[Show]
	public static string ScorePrefab = "ScoreMinimal";
	protected static GameObject _prefab = null;
	public static GameObject prefab{
		get{
			if (_prefab == null){
				_prefab = Resources.Load(ScorePrefab) as GameObject;
			}
			return _prefab;
		}
	}
	public float prefab_size = 100.0f;
	[Show]
	public float content_size{
		get{
			return SavedScoreComponent.Count;
		}
	}

	public ReactiveCollection<GameObject> views;

	void Start () {
		if (Place == null)
			Place = transform as RectTransform;
		Place.sizeDelta = new Vector2(Place.sizeDelta.x, content_size);
		views = SavedScoreComponent.saved_scores.rx_scores
		.RxWhere(score=>score != null)
		.RxSelect((score) => {
			GameObject obj = GameObject.Instantiate(prefab);
			obj.GetComponent<ScoreMinimalFormV2>().score = score;
			obj.transform.SetParent(Place,false);
			return obj;
		});
		views.ObserveRemove ().Subscribe ((evn) => {
			Destroy (evn.Value);
		});
	}
}
