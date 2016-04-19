using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;
public class RowGenerator : BetterBehaviour {
	public RowHandler2 row;
	[Show]
	public Dictionary<int,GameObject> objects = new Dictionary<int, GameObject>();
	public List<string> pattern = new List<string>();
	public Dictionary<string, UnityEngine.Object> loaded_prefabs = new Dictionary<string,UnityEngine.Object>();
	public Dictionary<string, List<GameObject>> object_pools = new Dictionary<string,List<GameObject>>();
	public UnityEngine.Object get_prefab(string name){
		if (!loaded_prefabs.ContainsKey(name)){
			loaded_prefabs[name] = Resources.Load(name);
		}
		return loaded_prefabs[name];
	}
	public GameObject get_object(string name){
		if (!object_pools.ContainsKey(name))
			object_pools[name] = new List<GameObject>();
		GameObject recycled = object_pools[name].Find((obj)=>{
			return !obj.activeSelf;
		});
		if (recycled == null){
			GameObject fresh = GameObject.Instantiate(get_prefab(name)) as GameObject;
			object_pools[name].Add(fresh);
			return fresh;
		}
		return recycled;
	}
	
	public static int wrap(int i, int around){
		if (around <= 0)
			return i;
		if (i < 0) {
			return wrap(i+around, around);
		}
		return i % around;
	}

	public List<Action<GameObject>> create_events = new List<Action<GameObject>>();
	public List<Action<GameObject>> destroy_events = new List<Action<GameObject>>();
	[DontSerialize]
	public Subject<GameObject> creation = new Subject<GameObject>();
	[DontSerialize]
	public Subject<GameObject> destruction = new Subject<GameObject>();
	public bool log = true;

	public int Count{
		get{return objects.Count;}
	}
	public GameObject at(int i){
		if (objects.ContainsKey (i)) {
			return objects[i];
		}
		return null;
	}
	public GameObject random_entry(int from, int to, Func<GameObject,bool> filter = null){
		int r = RandomUtils.random_int (from, to, "level_generation"), i;
		for (int adjust = 0; adjust < to - from; adjust++) {
			i = from + wrap(r+adjust, to-from);
			if (objects.ContainsKey(i)){
				if (filter == null || filter(objects[i]))
					return objects[i];
			}
		}
		return null;
	}
	public RowGenerator on_create(Action<GameObject> act){
		create_events.Add (act);
		/*
		creation.Subscribe((GameObject obj)=>{
			act(obj);
		});
		*/
		return this;
	}
	public RowGenerator on_destroy(Action<GameObject> act){
		destroy_events.Add (act);
		/*
		destruction.Subscribe((GameObject obj)=>{
			act(obj);
		});
		*/
		return this;
	}

	void Awake () {
		if (row == null)
			row = GetComponent<RowHandler2> ();
		row.creation.Subscribe ((int ci) => {
			if (pattern.Count > 0){
				string prefab = pattern[wrap(ci, pattern.Count)];
				if (prefab != ""){
					//Debug.Log ("Creating "+prefab+" at "+ci);
					objects[ci] = get_object(prefab);
					objects[ci].SetActive(true);
					objects[ci].transform.position = row.cell_to_pos(ci);
					objects[ci].transform.SetParent(transform,true);
					creation.OnNext(objects[ci]);
					foreach(Action<GameObject> act in create_events){
						act(objects[ci]);
					}
				}
			}
		});
		row.destruction.Subscribe ((int di) => {
			if (objects.ContainsKey(di)){
				destruction.OnNext(objects[di]);
				foreach(Action<GameObject> act in destroy_events){
					act(objects[di]);
				}
				objects[di].SetActive(false);
				objects.Remove(di);
			}
		});
	}
	
}
