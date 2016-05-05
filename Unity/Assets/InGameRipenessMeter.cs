using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Vexe.Runtime.Types;
using UniRx;
using GameSettings;

public class InGameRipenessMeter : MonoBehaviour {
	public bool is_max = false;
	public SpriteRenderer sprite;
	protected Material material;
	public float max_distance = 0.3f;
	public FloatReactiveProperty rx_value;
	void Start () {
		if (sprite == null)
			sprite = GetComponent<SpriteRenderer>();
		if (sprite != null){
			if (material == null){
				material = new Material(sprite.material);
			}
			sprite.material = material;
			if (is_max){
				GameSettingsComponent.working_rules.win_condition.ripeness.rx_max_accept_true.CombineLatest(
					GameSettingsComponent.working_rules.win_condition.ripeness.rx_limit_upper,
					(value, limited)=>{
						if (limited)
							return value;
						return StrawberryColor.max_quality;
					}
				).Subscribe(this.update);
			} else {
				GameSettingsComponent.working_rules.win_condition.ripeness.rx_min_accept_true.CombineLatest(
					GameSettingsComponent.working_rules.win_condition.ripeness.rx_limit_lower,
					(value, limited)=>{
						if (limited)
							return value;
						return 0.0f;
					}
				).Subscribe(this.update);
			}
		}
	}
	
	public Vector3 align(float value){
		return new Vector3(
			(value-0.5f)*max_distance,
			transform.localPosition.y,
			transform.localPosition.z
		);
	}
	
	void update (float value) {
		material.SetColor ("_Color", StrawberryColor.get_color(value));
		transform.localPosition = align(value/StrawberryColor.max_quality);
	}
}
