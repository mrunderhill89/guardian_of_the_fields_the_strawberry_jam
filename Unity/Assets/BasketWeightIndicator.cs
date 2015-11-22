using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

[ExecuteInEditMode]
public class BasketWeightIndicator : BetterBehaviour {
	public static Color under_weight = new Color();
	public static Color correct_weight = new Color();
	public static Color over_weight = new Color();
	public static Color locked_color = new Color();
	public static Color second_chance_color = new Color();
	public static Sprite under_sprite;
	public static Sprite over_sprite;
	public static Sprite correct_sprite;
	public static Sprite overflow_sprite;

	public static string format = "0.00";
	public TextMesh text;
	public SpriteRenderer sprite;

	public BasketComponent basket;
	void Start(){
		if (basket == null)
			basket = GetComponentInParent<BasketComponent> ();
		update(0.0f);
	}
	public void update(float value){
	if (text != null){
		text.text = value.ToString(format);

		if (basket.locked){
			if (basket.second_chance){
				text.color = second_chance_color;
			} else {
				text.color = locked_color;
			}
		} else if (basket.is_underweight()){
			text.color = under_weight;
				sprite.sprite = under_sprite;
		} else if (basket.is_overweight()){
			text.color = over_weight;
				sprite.sprite = over_sprite;
		} else {
			text.color = correct_weight;
				if (basket.is_overflow()){
					sprite.sprite = overflow_sprite;
				} else {
					sprite.sprite = correct_sprite;
				}
		}
	}
}

}
