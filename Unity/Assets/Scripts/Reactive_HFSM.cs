using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

[CustomEditor(typeof(Reactive_HFSM))]
public class HFSM_Editor : Editor {
	public Reactive_HFSM _target;
	void OnEnable()    
	{
		_target = (Reactive_HFSM)target;
	}

	public override void OnInspectorGUI () 
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update ();
		bool allowSceneObjects = !EditorUtility.IsPersistent (target);
		_target.auto_run = EditorGUILayout.Toggle("Auto Run", _target.auto_run);
		_target.name = EditorGUILayout.TextField ("Name", _target.name);
		string parent_name = _target.initial_parent == null?"none":_target.initial_parent.name;
		string initial_name = _target.initial_initial == null?"none":_target.initial_initial.name;
		_target.initial_parent = (Reactive_HFSM)EditorGUILayout.ObjectField ("Parent ("+parent_name+")", _target.initial_parent, typeof(Reactive_HFSM), allowSceneObjects);
		if (GUI.changed && _target.initial_parent != null && _target.initial_parent.initial_initial == null) {
			_target.initial_parent.initial_initial = _target;
		}
		_target.initial_initial = (Reactive_HFSM)EditorGUILayout.ObjectField ("Initial ("+initial_name+")", _target.initial_initial, typeof(Reactive_HFSM), allowSceneObjects);
		if (GUI.changed && _target.initial_initial != null && _target.initial_initial.initial_parent != _target) {
			_target.initial_initial = null;
		}
		//_target.on_entry = (UnityEvent)EditorGUILayout.ObjectField ("On Entry", _target.on_entry, typeof(UnityEvent), allowSceneObjects);
	}

}


public class Reactive_HFSM : MonoBehaviour {
	public static DependencyManager<Reactive_HFSM> deps;
	static Reactive_HFSM(){
		deps = new DependencyManager<Reactive_HFSM>();
	}
	public new string name = "HFSM";
	public Reactive_HFSM initial_parent = null;
	public Reactive_HFSM initial_initial = null;
	public Subject<Reactive_HFSM> parent;
	public Subject<Reactive_HFSM> initial;
	public Subject<Reactive_HFSM> current;
	public IObservable<Reactive_HFSM[]> parents;
	public IObservable<int> level;
	public IObservable<bool> active;
	public Subject<int> beat;
	public IObservable<bool[]> activity_change;
	public IObservable<List<Action>> actions;

	public UnityEvent on_entry;
	public UnityEvent on_exit;
	public UnityEvent on_update;

	public bool auto_run = false;

	IObservable<Reactive_HFSM[]> get_parent_chain(Reactive_HFSM p){
		if (p != null){
			return p.parents.Select<Reactive_HFSM[],Reactive_HFSM[]>(extend_chain);
		}
		return Observable.Return<Reactive_HFSM[]>(new Reactive_HFSM [] {this});
	}
	Reactive_HFSM[] extend_chain(Reactive_HFSM[] p_pars){
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
		if (p != null && p.active != null) {
			return p.active.CombineLatest(
				p.current.Select((Reactive_HFSM c)=>{
					return c == this;
				}),
				//Is the parent state active, and is this state the parent's current?
				(bool p_active, bool is_current)=>{
					return p_active && is_current;
				}
			);
		}
		//States with no parent are active by default.
		return Observable.Return(true);
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
		return acts;
	}
	// Use this for initialization
	void Start(){
		if (initial_parent == null) {
			initialize ();
		} else {
			deps.register_dep(initial_parent, initialize);
		}
	}
	void initialize () {
		parent = new Subject<Reactive_HFSM>();
		initial = new Subject<Reactive_HFSM>();
		current = new Subject<Reactive_HFSM>();
		beat = new Subject<int>();
		// Select = Map, SelectMany = FlatMap, CombineLatest = Combine
		parents = parent.SelectMany(get_parent_chain as Func<Reactive_HFSM, IObservable<Reactive_HFSM[]>>) as IObservable<Reactive_HFSM[]>;
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
			return is_active;
		}).Where ((bool is_active) => {
			return is_active;
		}).CombineLatest<bool, List<Action>, List<Action>> (actions, (bool is_active, List<Action> acts) => {
			Debug.Log(name+" Beat");
			return acts;
		}).Where((List<Action> acts)=>{
			return acts.Count > 0;
		}).Subscribe((List<Action> acts)=>{
			StartCoroutine(run_actions(acts));
		});
		//Set initial parent and initial state from the editor.
		if (initial_parent != null) {
			parent.OnNext (initial_parent);
		}
		if (initial_initial != null) {
			initial.OnNext (initial_initial);
		}
		if (auto_run) {
			StartCoroutine (this.coroutine ());
		}
		deps.load (this);
	}
	public Action lazy_set_current(Reactive_HFSM next_current){
		return () => {
			current.OnNext(next_current);
		};
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
