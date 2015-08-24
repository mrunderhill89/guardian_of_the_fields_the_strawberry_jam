using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class Reactive_Transition: BetterBehaviour {
	public ReactiveProperty<Reactive_HFSM> from;
	public ReactiveProperty<Reactive_HFSM> to;
	public IObservable<Reactive_HFSM[][]> path;
	public IObservable<bool> active;
	public IObservable<List<Action>> actions;
	public Subject<int> beat;

	public List<Func<bool>> tests;
	public bool auto_run = false;

	public UnityEvent on_transfer = null;

	void Start(){
		initialize ();
	}
	void initialize(){
		beat = new Subject<int>();
		tests = new List<Func<bool>> ();
		active = from.SelectMany ((Reactive_HFSM f) => {
			if (f == null)
				//Without a from state, all transitions are inactive.
				return Observable.Constant(false);
			return f.active;
		});
		path = from.SelectMany ((Reactive_HFSM f) => {
			if (f == null){
				//Can't create a path without both a from and to state.
				return Observable.Never<Reactive_HFSM[]>();
			}
			return f.parents;
		})
		.CombineLatest (to.SelectMany ((Reactive_HFSM t) => {
			if (t == null){
				//Can't create a path without both a from and to state.
				return Observable.Never<Reactive_HFSM[]>();
			}
			return t.parents;
		}),
				(Reactive_HFSM[] fp, Reactive_HFSM[] tp) => {
					return new Reactive_HFSM[][]{fp,tp};
				}
		).Select((Reactive_HFSM[][] parents)=>{
				/*
				 * This algorithm finds the common root (or "pivot") between both states,
				 * as well as every intermediate parent between them.
				*/
				//0 => From parents, 1=> To parents
				var upswing = new List<Reactive_HFSM>();
				var downswing = new List<Reactive_HFSM>();
				//Start at the end of the list and work up.
				//Resulting array: [this, parent1, parent2, ..., pivot]
				int ui = parents[0].Length-1, di = parents[1].Length-1;
				Reactive_HFSM us, ds;
				while (ui >= 0 && di >= 0){
					us = parents[0][ui]; ds = parents[1][di];
					if (us == ds){
						//Add the pivot state to both lists.
						upswing.Add(us); downswing.Add(ds);
						return new Reactive_HFSM[][]{upswing.ToArray(),downswing.ToArray()};
					}
					if (di > ui){
						//Add the current down state to downswing and decrement.
						downswing.Add(ds);
						di--;
					} else {
						//Add the current up state to upswing and decrement.
						upswing.Add(us);
						ui--;
					}
				}
				throw(new Exception("From and To states share no common parent."));
		});
		actions = path.Select ((Reactive_HFSM[][] p) => {
			var output = new List<Action>();
			Reactive_HFSM us,ds;
			//Add upswing actions (from start to pivot)
			for (int ui = 0; ui < p[0].Length; ui++){
				us = p[0][ui];
				//Set upswing states to null.
				output.Add(us.lazy_set_current(null));
			}
			//Add the transition's own action
			if (on_transfer != null){
				output.Add(on_transfer.Invoke);
			}
			//Add downswing actions (from pivot to end)
			for (int di = p[1].Length-1; di > 0; di--){
				ds = p[1][di];
				//Set downswing states to the next one in the sequence.
				output.Add(ds.lazy_set_current(p[1][di-1]));
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
