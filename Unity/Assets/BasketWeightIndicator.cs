using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

[ExecuteInEditMode]
public class BasketWeightIndicator : BetterBehaviour {
	public static Color under_weight = new Color();
	public static Color correct_weight = new Color();
	public static Color over_weight = new Color();
	public static string format = "0.00";
	public TextMesh text;
	void Start(){
		update(0.0f);
	}
	public void update(float value){
	if (text != null){
		text.text = value.ToString(format);
		if (value < GameStartData.min_basket_weight){
			text.color = under_weight;
		} else if (value > GameStartData.max_basket_weight){
			text.color = under_weight;
		} else {
			text.color = correct_weight;
		}
	}
}

}
