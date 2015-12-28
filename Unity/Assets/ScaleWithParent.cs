using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
using UniRx;

[ExecuteInEditMode]
public class ScaleWithParent : BetterBehaviour {
	public RectTransform target;
	public RectTransform parent;
	public Vector2 target_natural_scale;
	public Vector2 parent_natural_scale;
	void Start(){
		if (target == null)
			target = gameObject.transform as RectTransform;
		if (parent == null)
			parent = target.parent as RectTransform;
		target_natural_scale = target.sizeDelta;
		parent_natural_scale = parent.sizeDelta;
	}

	void Update () {
		if (target != null && parent != null){
			target.sizeDelta = new Vector2(
				target_natural_scale.x * parent.sizeDelta.x / parent_natural_scale.x,
				target_natural_scale.y * parent.sizeDelta.y / parent_natural_scale.y
			);
		}
	}
}
