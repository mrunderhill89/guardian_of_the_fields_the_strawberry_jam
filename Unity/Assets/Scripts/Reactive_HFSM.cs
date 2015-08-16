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
	public string name = "HFSM";
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

	// Use this for initialization
	void Start () {
		parent = new Subject<Reactive_HFSM>();
		initial = new Subject<Reactive_HFSM>();
		current = new Subject<Reactive_HFSM>();
		beat = new Subject<int>();
		//Set initial parent and initial state from the editor.
		if (initial_parent != null) {
			parent.OnNext (initial_parent);
		}
		if (initial_initial != null) {
			initial.OnNext (initial_initial);
		}
		// Select = Map, SelectMany = FlatMap, CombineLatest = Combine
		parents = parent.SelectMany((Reactive_HFSM p) => {
			if (p != null){
				return p.parents.Select((Reactive_HFSM[] p_pars)=>{
					Reactive_HFSM[] result = new Reactive_HFSM[p_pars.Length+1];
					//[root, child1, child2, ..., this]
					p_pars.CopyTo(result, 0);
					result[p_pars.Length] = this;
					return result;
				});
			}
			return Observable.Return(new Reactive_HFSM [] {this});
		});
		level = parents.Select ((Reactive_HFSM[] ps) => {
			return ps.Length;
		});
		//True if root or parent is active and this is current.
		active = parent.SelectMany ((Reactive_HFSM p) => {
			if (p != null) {
				return p.active.CombineLatest(
					p.current.Select((Reactive_HFSM c)=>{
						return c == this;
					}),
					//Is the parent state active, and is this state the parent's current?
					(bool p_active, bool is_current)=>{return p_active && is_current;}
				);
			}
			//States with no parent are active by default.
			return Observable.Return(true);
		});
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
		actions = current.CombineLatest<Reactive_HFSM,Reactive_HFSM,List<Action>>(initial, (Reactive_HFSM c, Reactive_HFSM i) => {
			var acts = new List<Action>();
			if (c == null){
				if (i != null){
					acts.Add(this.lazy_set_current(i));
				}
			} else {
				acts.Add(this.on_update.Invoke);
				acts.Add(c.run);
			}
			return acts;
		});
		//Main update channel
		beat.CombineLatest<int,bool,bool> (active, (int frame, bool is_active) => {
			return is_active;
		}).Where ((bool is_active) => {
			return is_active;
		}).CombineLatest<bool, List<Action>, List<Action>> (actions, (bool is_active, List<Action> acts) => {
			return acts;
		}).Where((List<Action> acts)=>{
			return acts.Count > 0;
		}).Subscribe((List<Action> acts)=>{
			Debug.Log("HFSM Beat");
			StartCoroutine(run_actions(acts));
		});
		if (auto_run) {
			StartCoroutine (this.coroutine ());
		}
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
