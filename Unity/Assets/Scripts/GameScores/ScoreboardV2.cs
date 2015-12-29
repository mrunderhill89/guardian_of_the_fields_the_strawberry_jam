using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using GameScores;
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
	[DontSerialize]
	public ReactiveProperty<System.Func<Score,Score,int>> rx_compare
		=	new ReactiveProperty<System.Func<Score,Score,int>>();
	protected ReadOnlyReactiveProperty<ScoreComparer> rx_comparer;
	
	[DontSerialize][Show]
	public ReactiveProperty<Score[]> sorted_scores;
	[DontSerialize][Show]
	public List<GameObject> views = new List<GameObject>();
	void Start () {
		if (Place == null)
			Place = transform as RectTransform;
		rx_comparer = rx_compare.Scan(new ScoreComparer(), (comp, fun)=>{
			comp.fun = fun;
			return comp;
		}).ToReadOnlyReactiveProperty();
		sorted_scores = SavedScoreComponent.saved_scores.rx_scores.ObserveContents()
		.CombineLatest(rx_comparer, (evn, compare)=>{
			return new UniRx.Tuple<Score[],IComparer<Score>>(evn.Contents,compare);
		})
		.Select((tuple)=>{
			if (tuple.Item2 != null)
				Array.Sort(tuple.Item1, tuple.Item2);
			return tuple.Item1;
		}).ToReactiveProperty();
		sorted_scores.Subscribe(scores=>{
			views = ZipLongest(scores, views, (score, view)=>{
				if (view == null){
					view = GameObject.Instantiate(prefab);
				} else {
					view.transform.SetParent(null, false);
				}
				if (score == null){
					view.SetActive(false);
				} else {
					view.GetComponent<ScoreMinimalFormV2>().score = score;
					view.SetActive(true);
					view.transform.SetParent(Place, false);
				}
				return view;
			}).ToList();
		});
		rx_compare.Value = null;
	}
	
	public class ScoreComparer: IComparer<Score>{
		public System.Func<Score,Score,int> fun;
		public int Compare(Score a, Score b){
			if (fun == null)
				return b.time.date_recorded.CompareTo(a.time.date_recorded);
			return fun(a,b);
		}
	}
	
	public static IEnumerable<Result> ZipLongest<TypeA,TypeB,Result> 
		(IEnumerable<TypeA> sourceA, IEnumerable<TypeB> sourceB, System.Func<TypeA,TypeB,Result> combine) 
	{
		int countA = sourceA.Count();
		int countB = sourceB.Count();
		int longest = Math.Max(countA, countB);
		TypeA[] arrayA = sourceA.ToArray();
		TypeB[] arrayB = sourceB.ToArray();
		TypeA a; TypeB b; Result result;
		for (int i = 0; i < longest; i++){
			a = i < countA? arrayA[i]:default(TypeA);
			b = i < countB? arrayB[i]:default(TypeB);
			result = combine(a,b);
			if (!result.Equals(default(Result)))
				yield return result;
		}
	}
}
