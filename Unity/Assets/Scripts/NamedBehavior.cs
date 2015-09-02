using Vexe.Runtime.Types;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GenericDictionary
{
	protected Dictionary<string,object> _dict = new Dictionary<string, object>();

	public void Add<T>(string key, T value) where T : class
	{
		_dict.Add(key, value);
	}
	
	public T GetValue<T>(string key) where T : class
	{
		return _dict[key] as T;
	}

	public Dictionary<string,T> ToStandardDict<T>() where T:class{
		var output = new Dictionary<string,T> ();
		foreach (string key in _dict.Keys) {
			output[key] = GetValue<T>(key);
		}
		return output;
	}
}


public abstract class NamedBehavior : BetterBehaviour {
	public string _instance_name = "Named Component";
	[Serialize]
	public string instance_name {
		get {return _instance_name;}
		set {_instance_name = value;}
	}

	static Dictionary<Type, Dictionary<GameObject, GenericDictionary>> instances;
	static NamedBehavior(){
		instances = new Dictionary<Type, Dictionary<GameObject, GenericDictionary>>();
	}
	public static Dictionary<string,T> GetComponentsByName<T>(GameObject obj) where T:NamedBehavior{
		Type T_type = typeof(T);
		if (!instances.ContainsKey (T_type)) {
			instances.Add(T_type, new Dictionary<GameObject, GenericDictionary>());
		}
		if (!instances[T_type].ContainsKey(obj)){
			instances[T_type].Add(obj,new GenericDictionary());
			foreach (T component in (obj as GameObject).GetComponents<T>()){
				instances[T_type][obj].Add<T>(component.instance_name, component);
			}
		}
		return instances [T_type] [obj].ToStandardDict<T>();
	}
	public static T GetComponentByName<T>(GameObject obj, string name)
		where T:NamedBehavior
	{
		Dictionary<string, T> instances = GetComponentsByName<T>(obj);
		if (instances.ContainsKey(name)){
			return (T)instances[name];
		}
		return null;
	}
	public static T GetOrCreateComponentByName<T>(GameObject obj, string name)
		where T:NamedBehavior
	{
		T existing = GetComponentByName<T>(obj, name);
		if (existing == null) {
			existing = obj.AddComponent<T>();
			existing.instance_name = name;
			instances[typeof(T)][obj].Add<T>(name, existing);
		}
		return existing;
	}
}