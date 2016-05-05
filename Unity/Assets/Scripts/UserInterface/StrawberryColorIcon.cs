using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Vexe.Runtime.Types;
using UniRx;

public class StrawberryColorIcon : BetterBehaviour {

	public Slider slider;
	public Image icon;
	
	[DontSerialize]
	public ReadOnlyReactiveProperty<float> rx_value;

	protected float clamp_value(float value){
		return value / StrawberryColor.max_quality;
	}

	void Start() {
		if (slider != null){
			rx_value = slider.OnValueChangedAsObservable().Select<float, float>(
				clamp_value).ToReadOnlyReactiveProperty<float>(clamp_value(slider.value));
			rx_value.Subscribe(
				value => {
					if (icon != null)
						icon.color = StrawberryColor.color_gradient.Evaluate(value);
				}
			);
		}
	}
}
