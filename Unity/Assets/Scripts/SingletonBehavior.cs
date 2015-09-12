using Vexe.Runtime.Types;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public abstract class SingletonBehavior: BetterBehaviour
{
	private static Dictionary<Type,SingletonBehavior> instances;
	static SingletonBehavior(){
		instances = new Dictionary<Type, SingletonBehavior> ();
	}
	protected void Awake(){
		Type this_type = this.GetType();
		if (!instances.ContainsKey(this_type)){
			instances.Add (this_type, this);
		} else {
			Debug.LogError("Already an instance of singleton behavior:"+this_type.Name);
		}
	}
	public static T get_instance<T>() where T:SingletonBehavior{
		T instance = instances[typeof(T)] as T;
		if (instance == default(T)){
			Debug.LogError("Singleton instance not defined:"+typeof(T).Name);
		}
		return instance;
	}
}