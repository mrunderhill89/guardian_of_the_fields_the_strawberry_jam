using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

public class OverflowDetector : BetterBehaviour {
	public static float Time_Before_Panic = 0.25f;
	public static float Time_Before_Relax = 0.1f;
	public string panic_material_name = "OverflowBad";
	protected Material panic_material;
	protected Material normal_material;
	float relax_time = 0.0f;
	float panic_time = 0.0f;
	public bool is_hidden = false;
	void Awake(){
		normal_material = GetComponent<Renderer> ().material;
		panic_material = Resources.Load<Material>(panic_material_name);
	}
	void Update(){
		Renderer my_renderer = GetComponent<Renderer> ();
		my_renderer.enabled = true;
		if (is_overflow()) {
			my_renderer.material = panic_material;
			panic_time -= Time.deltaTime;
			if (panic_time < 0.0f) {
				relax_time = 0.0f;
			}
		} else {
			if (is_hidden){
				my_renderer.enabled = false;
			} else {
				my_renderer.material = normal_material;
			}
		}
	}
	void OnTriggerStay(Collider that) {
		StrawberryComponent sb = that.GetComponent<StrawberryComponent> ();
		if (sb != null){
			relax_time += Time.deltaTime;
			if (relax_time > Time_Before_Panic){
				panic_time = Time_Before_Relax;
			}
		}
	}
	public bool is_overflow(){
		return panic_time > 0.0f;
	}
	public bool hidden(){
		return is_hidden;
	}
	public OverflowDetector hidden(bool new_val){
		is_hidden = new_val;
		return this;
	}
}
