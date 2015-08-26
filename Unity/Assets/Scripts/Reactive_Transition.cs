using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using StateChain = System.Collections.Generic.List<Reactive_HFSM>;

public class Reactive_Transition: BetterBehaviour {
	public ReactiveProperty<Reactive_HFSM> from;
	public ReactiveProperty<Reactive_HFSM> to;
	//public IObservable<Reactive_HFSM[][]> path;
	public IObservable<StateChain> upswing;
	public IObservable<StateChain> downswing;
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
		actions = upswing.CombineLatest<StateChain,StateChain,List<Action>>(downswing, (up, down) => {
			var output = new List<Action>();
			foreach(Reactive_HFSM us in up){
				output.Add(us.lazy_set_current(null));
			}
			if (on_transfer != null){
				output.Add(on_transfer.Invoke);
			}
			foreach(Reactive_HFSM ds in down){
			}
			return output;
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
