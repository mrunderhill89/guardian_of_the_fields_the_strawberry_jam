using UnityEngine;
using UnityEngine.UI;
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
	
	[DontSerialize][Show]
	public IObservable<Score[]> sorted_scores;
	[DontSerialize][Show]
	public List<GameObject> views = new List<GameObject>();
	
	public ReactiveCollection<ScoreComparer> rx_sorters
		= new ReactiveCollection<ScoreComparer>();
	public Dropdown select_filter;
	public Toggle reverse_toggle;
	[DontSerialize]
	public ReadOnlyReactiveProperty<ScoreComparer> rx_current_sorter;
	[DontSerialize]
	public ReadOnlyReactiveProperty<bool> rx_reverse_sort;

	void Start () {
		if (Place == null)
			Place = transform as RectTransform;
		
		rx_sorters.Add(
			new ScoreComparer()
			.key("sort_date")
			.compare((a,b)=>{
				return b.time.date_recorded.CompareTo(a.time.date_recorded);
			})
		);
		rx_sorters.Add(
			new ScoreComparer()
			.key("sort_accepted_berries")
			.compare((a,b)=>{
				return a.ripe_berries("gathered").Count().CompareTo(b.ripe_berries("gathered").Count());
			})
		);
		rx_sorters.Add(
			new ScoreComparer()
			.key("sort_accepted_baskets")
			.compare((a,b)=>{
				return a.accepted_baskets().Count().CompareTo(b.accepted_baskets().Count());
			})
		);

		rx_current_sorter = select_filter
		.SelectFromCollection(rx_sorters, (f,index)=>{
			return f.rx_key.SelectMany(key=>LanguageController.controller.rx_load_text(key));
		})
		.ToReadOnlyReactiveProperty();
		
		rx_reverse_sort = reverse_toggle.OnValueChangedAsObservable().ToReadOnlyReactiveProperty<bool>(false);
		
		sorted_scores = rx_current_sorter
		.CombineLatest(rx_reverse_sort, (compare, reverse)=>{
			compare.reverse = reverse;
			return compare;
		}).CombineLatest(SavedScoreComponent.saved_scores.rx_scores.ObserveContents(), (compare, evn)=>{
			if (compare != null){
				Array.Sort(evn.Contents, compare);
			}
			return evn.Contents;
		});
		
		sorted_scores.Subscribe(scores=>{
			foreach(GameObject view in views){
				if (view != null){
					view.transform.SetParent(null, false);
				}
			}
			views = ZipLongest(scores, views, (score, view)=>{
				if (view == null){
					view = GameObject.Instantiate(prefab);
				}
				view.transform.SetParent(Place, false);
				if (score == null){
					view.SetActive(false);
				} else {
					view.GetComponent<ScoreMinimalFormV2>().score = score;
					view.SetActive(true);
				}
				return view;
			}).ToList();
		});
	}
	
	public class ScoreComparer: IComparer<Score>{
		public StringReactiveProperty rx_key = new StringReactiveProperty("sort_unknown");
		public bool reverse = false;
		protected System.Func<Score,Score,int> fun;
		public int Compare(Score a, Score b){
			int result = fun(a,b) * (reverse ? -1:1);
			return result;
		}
		public ScoreComparer compare(System.Func<Score,Score,int> _compare){
			fun = _compare;
			return this;
		}
		public string key(){
			return rx_key.Value;
		}
		public ScoreComparer key(string _key){
			rx_key.Value = _key;
			return this;
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
