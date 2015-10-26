using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;
[ExecuteInEditMode]
public class StrawberryGlow : BetterBehaviour {
	ObjectVisibility visibility;
	DragHandle drag;
	void Start (){
		visibility = GetComponent<ObjectVisibility> ();
		drag = GetComponentInParent<DragHandle> ();
	}
	// Update is called once per frame
	void Update () {
		visibility.visible = drag.can_drag || drag.is_dragging();
	}
}
