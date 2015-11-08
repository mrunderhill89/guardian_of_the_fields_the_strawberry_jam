using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vexe.Runtime.Types;

[ExecuteInEditMode]
public class PlaceOnSliderUI : BetterBehaviour {
	public float position = 0.0f;
	public Slider slider;
	[Show]
	public float calculated_position{
		get{ 
			if (slider != null){
				float value_min = slider.minValue;
				float value_max = slider.maxValue;
				float screen_min = slider.GetComponent<RectTransform>().rect.xMin;
				float screen_max = slider.GetComponent<RectTransform>().rect.xMax;
				//(position-slider.minValue)/(slider.maxValue-slider.minValue) - 
				float normalized = (position - value_min)/(value_max - value_min);
				return screen_min + normalized * (screen_max - screen_min);
			}
			return transform.localPosition.x;
		}
	}
	// Update is called once per frame
	void Update () {
		if (slider != null){
			Vector2 new_position = new Vector2(
				calculated_position,
				transform.localPosition.y
			);
			transform.localPosition = new_position;
		}
	}
}
