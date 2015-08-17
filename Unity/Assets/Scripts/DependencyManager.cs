using UnityEngine;
using UniRx;
using System;
using System.Collections;
using System.Collections.Generic;

using ActionList = System.Collections.Generic.List<System.Action>;

public class DependencyManager <T>
{
	protected struct dep{
		public T waiting_for;
		public ActionList actions;
	}
	protected Subject<T> pass_in;
	protected IObservable<HashSet<T>> loaded;
	protected Subject<dep> dependency;
	protected IObservable<Dictionary<T, ActionList>> dependencies;
	public DependencyManager ()
	{
		pass_in = new Subject<T> ();
		dependency = new Subject<dep> ();
		loaded = pass_in.Scan (new HashSet<T> (), (objs, obj) => {
			objs.Add(obj);
			return objs;
		});
		dependencies = dependency.Scan (new Dictionary<T,ActionList> (), (Dictionary<T,ActionList> deps, dep n_dep) => {
			T key = n_dep.waiting_for;
			if (!deps.ContainsKey(key)){
				deps[key] = new ActionList();
			}
			deps[key].AddRange(n_dep.actions);
			return deps;
		});
		loaded.CombineLatest<HashSet<T>, Dictionary<T,ActionList>, ActionList> (dependencies, (l, d) => {
			ActionList actions = new ActionList();
			foreach(T key in d.Keys){
				if (l.Contains(key)){
					actions.AddRange(d[key]);
					d[key].Clear();
				}
			}
			return actions;
		}).Where(actions=>{return actions.Count > 0;}).Subscribe(actions=>{
			actions.ForEach(action=>{
				action();
			});
		});
	}
	public void load(T obj){
		pass_in.OnNext (obj);
	}
	public void register_dep(T obj, ActionList actions){
		dep n_dep = new dep ();
		n_dep.waiting_for = obj;
		n_dep.actions = actions;
		dependency.OnNext (n_dep);
	}
	public void register_dep(T obj, Action action){
		ActionList actions = new ActionList ();
		actions.Add (action);
		register_dep (obj, actions);
	}
}
