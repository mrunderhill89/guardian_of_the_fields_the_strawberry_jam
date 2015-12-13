using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Vexe.Runtime.Types;
public class StrawberryColorIcon : BetterBehaviour {

	public Slider slider;
	public Image icon;
	[Show]
	public float value{
		get{return slider.value / StrawberryColor.max_quality;}
	}
	[Show]
	public Color color{
		get{return StrawberryColor.color_gradient.Evaluate(value);}
	}
	// Update is called once per frame
	void Update () {
		icon.color = color;
	}
}
