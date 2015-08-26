using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using StateChain = System.Collections.Generic.List<Reactive_HFSM>;

public class Reactive_Transition: BetterBehaviour {
	public class StatePath{
		public StateChain up;
		public StateChain down;
		public Reactive_HFSM root;
		public StatePath(){
			up = new StateChain();
			down = new StateChain();
			root = null;
		}
	}
	public ReactiveProperty<Reactive_HFSM> from;
	public ReactiveProperty<Reactive_HFSM> to;
	public IObservable<StateChain> upswing;
	public IObservable<StateChain> downswing;
	[DontSerialize]
	public IObservable<StatePath> path;
	public IObservable<bool> active;
	public IObservable<List<Action>> actions;
	[DontSerialize]
	public Subject<int> beat;

	public List<Func<bool>> tests;
	public bool auto_run = false;

	public UnityEvent on_transfer = null;

	void Start(){
		beat = new Subject<int>();
		tests = new List<Func<bool>> ();
		active = from.SelectMany ((Reactive_HFSM f) => {
			if (f == null)
				//Without a from state, all transitions are inactive.
				return Observable.Return(false);
			return f.active;
		});
		upswing = from.SelectMany((Reactive_HFSM f) => {
			if (f == null)
				return Observable.Never<StateChain>();
			return f.parents as IObservable<StateChain>;
		});
		downswing = to.SelectMany((Reactive_HFSM t) => {
			if (t == null)
				return Observable.Never<StateChain>();
			return t.parents as IObservable<StateChain>;
		});
		path = upswing.CombineLatest<StateChain,StateChain,StatePath>(downswing, (StateChain up, StateChain down) => {
			var _path = new StatePath();
			if (up.Count > 0 && down.Count > 0){
				int ui = up.Count-1, di = down.Count-1;
				Reactive_HFSM us = up[ui], ds = down[di];
				while (_path.root == null && (ui >= 0 && di >= 0)){
					if (us == ds){
						_path.root = us;
						_path.down.Add(us);
					} else if (ui > di || di == 0){
						ui--;
						_path.up.Add(us);
						us = up[ui];
					} else {
						di--;
						_path.down.Add(ds);
						ds = down[di];
					}
				}
			}
			return _path;
		}).Where((_path)=>{
			//Only return complete paths
			return _path.root != null;
		});
		actions = path.Select((StatePath _path) => {
			var _actions = new List<Action>();
			_actions.AddRange(_path.up.AsEnumerable().Reverse().Select((Reactive_HFSM us)=>{
				return us.lazy_set_current(null);
			}));
			_actions.Add(this.on_transfer.Invoke);
			_path.down.Aggregate(null, (Reactive_HFSM first, Reactive_HFSM next)=>{
				if (first != null){
					_actions.Add(first.lazy_set_current(next));
				}
				return next;
			});
			return _actions;
		});
		beat.CombineLatest<int, bool, bool> (active, (int frame, bool is_active) => {
			if (is_active){
				foreach (Func<bool> test in tests){
					if (!test()) return false;
				}
				return true;
			}
			return false;
		}).Where ((bool is_active) => {
			//Only keep going if all tests succeeded.
			return is_active;
		})
		.CombineLatest<bool, List<Action>, List<Action>> (actions, (bool is_active, List<Action> acts) => {
			return acts;
		}).Where((List<Action> acts)=>{
			return acts.Count > 0;
		}).Subscribe((List<Action> acts)=>{
			Debug.Log("Transition Beat");
			StartCoroutine(run_actions(acts));
		});
		if (auto_run) {
			StartCoroutine (this.coroutine ());
		}
	}
	IEnumerator run_actions(List<Action> acts){
		foreach(Action act in acts){
			act();
			yield return null;
		}
	}
	public void run(){
		beat.OnNext(0);
	}
	IEnumerator coroutine(){
		while (true) {
			this.run ();
			yield return null;
		}
	}
}
