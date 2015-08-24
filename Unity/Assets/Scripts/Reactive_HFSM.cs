using Vexe.Runtime.Types;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class Reactive_HFSM : BetterBehaviour {
	[DontSerialize]
	public static DependencyManager<Reactive_HFSM> deps;
	static Reactive_HFSM(){
		deps = new DependencyManager<Reactive_HFSM>();
	}
	public new string name = "HFSM";
	public ReactiveProperty<Reactive_HFSM> parent { get; private set;}
	public ReactiveProperty<Reactive_HFSM>  initial { get; private set;}
	public ReactiveProperty<Reactive_HFSM>  current { get; private set;}
	[DontSerialize]
	public IObservable<Reactive_HFSM[]> parents;
	[DontSerialize]
	public IObservable<int> level;
	[DontSerialize]
	public IObservable<bool> active;
	[DontSerialize]
	public Subject<int> beat;
	[DontSerialize]
	public IObservable<bool[]> activity_change;
	[DontSerialize]
	public IObservable<List<Action>> actions;

	public UnityEvent on_entry;
	public UnityEvent on_exit;
	public UnityEvent on_update;

	public bool auto_run = false;

	IObservable<Reactive_HFSM[]> get_parent_chain(Reactive_HFSM p){
		if (p != null){
			return p.parents.Select<Reactive_HFSM[],Reactive_HFSM[]>(extend_chain);
		}
		return Observable.Constant<Reactive_HFSM[]>(new Reactive_HFSM [] {this});
	}

	Reactive_HFSM[] extend_chain(Reactive_HFSM[] p_pars){
		Debug.Log ("Get parents from parent:"+p_pars.Length);
		Reactive_HFSM[] result = new Reactive_HFSM[p_pars.Length+1];
		//[root, child1, child2, ..., this]
		p_pars.CopyTo(result, 0);
		result[p_pars.Length] = this;
		return result;
	}
	int get_level(Reactive_HFSM[] parent_chain){
		return parent_chain.Length;
	}
	IObservable<bool> get_active(Reactive_HFSM p){
		if (p != null) {
			Debug.Log ("Get activity from parent.");
			return p.active.CombineLatest(
				p.current.Select((Reactive_HFSM c)=>{
					return c == this;
				}),
				//Is the parent state active, and is this state the parent's current?
				(bool p_active, bool is_current)=>{
					Debug.Log(name+" parent active:"+p_active);
					Debug.Log(name+" is current:"+is_current);
					return p_active && is_current;
				}
			);
		}
		Debug.Log ("Root is always active.");
		//States with no parent are active by default.
		return Observable.Constant(true);
	}
	List<Action> build_actions(Reactive_HFSM c, Reactive_HFSM i){
		var acts = new List<Action>();
		if (c == null){
			if (i != null){
				acts.Add(this.lazy_set_current(i));
			}
		} else {
			if (on_update != null) {
				acts.Add (this.on_update.Invoke);
			}
			acts.Add(c.run);
		}
		Debug.Log (name + " built actions:"+acts.Count);
		return acts;
	}
	// Use this for initialization
	void Start(){
		//parent = new ReactiveProperty<Reactive_HFSM> ();
		//initial = new ReactiveProperty<Reactive_HFSM> ();
		current = new ReactiveProperty<Reactive_HFSM> ();
		initialize ();
	}
	void initialize () {
		beat = new Subject<int>();
		// Select = Map, SelectMany = FlatMap, CombineLatest = Combine
		parents = parent.SelectMany(get_parent_chain as Func<Reactive_HFSM, IObservable<Reactive_HFSM[]>>);
		level = parents.Select<Reactive_HFSM[], int>(get_level);
		level.Subscribe((int lv)=>{Debug.Log(name+" Level:"+lv.ToString());});
		//True if root or parent is active and this is current.
		active = parent.SelectMany(get_active as Func<Reactive_HFSM, IObservable<bool>>);
		active.Subscribe((bool a)=>{Debug.Log(name+" Active:"+a.ToString());});
		//Determines whether we're exiting or entering the state for the first time.
		activity_change = active.Scan(new bool []{false,false}, (bool[] c, bool a)=>{
			return new bool[]{c[1],a};
		});
		activity_change.Subscribe((bool[] c)=>{
			if (!c[0] && c[1] && on_entry != null){
				//Enter - was inactive, now active
				on_entry.Invoke();
			}
			if(c[0] && !c[1] && on_exit != null){
				//Exit - was active, now inactive
				on_exit.Invoke();
			}
		});
		//Generate actions
		actions = current.CombineLatest<Reactive_HFSM,Reactive_HFSM,List<Action>>(initial, build_actions);
		//Main update channel
		beat.CombineLatest<int,bool,bool> (active, (int frame, bool is_active) => {
			//Debug.Log(name+" Beat Combine");
			return is_active;
		}).Where ((bool is_active) => {
			//Debug.Log(name+" Activity Check:"+is_active);
			return is_active;
		}).CombineLatest<bool, List<Action>, List<Action>> (actions, (bool is_active, List<Action> acts) => {
			//Debug.Log(name+" Actions:"+acts.Count);
			return acts;
		}).Where((List<Action> acts)=>{
			//Debug.Log(name+" Actions Check");
			return acts.Count > 0;
		}).Subscribe((List<Action> acts)=>{
			//Debug.Log(name+" Actions Start");
			StartCoroutine(run_actions(acts));
		});
		//Debug Logs
		current.Subscribe((child)=>{
			if (child != null){
				Debug.Log(name+": "+child.name);
			} else {
				Debug.Log(name+": null");
			}
		});
		//Set initial parent and initial state from the editor.
		if (auto_run) {
			Debug.Log(name+" Auto-Run Start");
			StartCoroutine (this.coroutine ());
		}
		deps.load (this);
	}
	public Action lazy_set_current(Reactive_HFSM next_current){
		return () => {
			current.Value = next_current;
		};
	}
	IEnumerator run_actions(List<Action> acts){
		foreach(Action act in acts){
			act();
			yield return null;
		}
	}
	public void run(){
		beat.OnNext(1);
	}
	IEnumerator coroutine(){
		for (int frame = 0; true; frame++){
			this.run();
			yield return null;
		}
	}
}
