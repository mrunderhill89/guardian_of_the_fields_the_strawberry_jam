using UnityEngine;
using System.Collections;

public class StrawberryFootprint : MonoBehaviour {

	public DragHandle drag;
	public ObjectVisibility vis;
	public Transform target;
	public float max_distance = 2.0f;
	void Update () {
		if (drag.is_dragging()){
			RaycastHit hit;
			if (Physics.Raycast(target.position, Vector3.down, out hit, max_distance, drag.layer_mask, QueryTriggerInteraction.Ignore)) {
				transform.position = hit.point;
				vis.visible = true;
				return;
			}
		}
		vis.visible = false;
	}
}
