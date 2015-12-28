using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
public class StrawberryGlow : BetterBehaviour {
	public ObjectOpacity opacity;
	DragHandle drag;
	void Start (){
		if (opacity == null)
			opacity = GetComponent<ObjectOpacity> ();
		drag = GetComponentInParent<DragHandle> ();
	}
	// Update is called once per frame
	void Update () {
		if (opacity != null && drag.can_drag || drag.is_dragging()){
			opacity.target_opacity = 1.0f;
		} else {
			opacity.target_opacity = 0.0f;
		}
	}
}
