using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class OverflowDetector : BetterBehaviour {
	public static float Time_Before_Panic = 0.25f;
	public static float Time_Before_Relax = 0.1f;
	public string panic_material_name = "OverflowBad";
	protected Material panic_material;
	protected Material normal_material;
	public BasketComponent basket;
	public ObjectVisibility visible;
	float relax_time = 0.0f;
	float panic_time = 0.0f;
	public bool show_while_passive = false;

	public List<Action> panic_events = new List<Action>();
	public List<Action> relax_events = new List<Action>();
	public OverflowDetector on_panic(Action act){
		panic_events.Add(act);
		return this;
	}
	public OverflowDetector on_relax(Action act){
		relax_events.Add(act);
		return this;
	}
	void Awake(){
		normal_material = GetComponent<Renderer> ().material;
		panic_material = Resources.Load<Material>(panic_material_name);
		if (visible == null)
			visible = GetComponent<ObjectVisibility> ();
		if (basket == null)
			basket = GetComponentInParent<BasketComponent> ();
	}
	void Update(){
		Renderer my_renderer = GetComponent<Renderer> ();
		if (is_overflow()) {
			visible.visible = true;
			my_renderer.material = panic_material;
			panic_time -= Time.deltaTime;
			if (panic_time < 0.0f) {
				relax_time = 0.0f;
				foreach(Action act in relax_events){
					act();
				}
			}
		} else {
			my_renderer.material = normal_material;
			visible.visible = show_while_passive;
		}
	}
	void OnTriggerStay(Collider that) {
		StrawberryComponent sb = that.GetComponent<StrawberryComponent> ();
		if (sb != null){
			relax_time += Time.deltaTime;
			if (relax_time > Time_Before_Panic){
				panic_time = Time_Before_Relax;
				foreach(Action act in panic_events){
					act();
				}
			}
		}
	}
	public bool is_overflow(){
		return panic_time > 0.0f;
	}
}
