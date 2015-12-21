using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class ScoreboardV2 : BetterBehaviour {
	public RectTransform Place;
	public ScoreHandler Scores;

	[Show]
	public static string ScorePrefab = "ScoreMinimal";
	protected static UnityEngine.Object _prefab = null;
	public static UnityEngine.Object prefab{
		get{
			if (_prefab == null){
				_prefab = Resources.Load(ScorePrefab);
			}
			return _prefab;
		}
	}
	public float prefab_size = 100.0f;
	[Show]
	public float content_size{
		get{
			if (Scores != null){
				return Scores.saved_scores.Count * prefab_size;
			}
			return 0.0f;
		}
	}

	public List<GameObject> views = new List<GameObject>();

	void Start () {
		if (Place == null)
			Place = transform as RectTransform;
		Place.sizeDelta = new Vector2(Place.sizeDelta.x, content_size);
		views = Scores.saved_scores.Select((score)=>{
			GameObject view = GameObject.Instantiate(prefab) as GameObject;
			view.GetComponent<ScoreMinimalForm>().score.Value = score;
			return view;
		}).ToList<GameObject>();
		order ();
	}

	ScoreboardV2 order(){
		foreach (GameObject view in views){
			view.transform.SetParent(null,false);
			view.transform.SetParent(Place,false);
		}
		return this;
	}
}
