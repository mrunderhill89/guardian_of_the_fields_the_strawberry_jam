using UnityEngine;
using System.Collections;

public class StrawberryFootprint : MonoBehaviour {

	public DragHandle drag;
	public ObjectOpacity opacity;
	public Transform target;
	public float max_distance = 2.0f;
	void Start(){
		if (opacity == null)
			opacity = GetComponent<ObjectOpacity> ();
	}
	void Update () {
		if (drag.is_dragging()){
			RaycastHit hit;
			if (Physics.Raycast(target.position, Vector3.down, out hit, max_distance, drag.layer_mask, QueryTriggerInteraction.Ignore)) {
				transform.position = hit.point;
				opacity.target_opacity = 1.0f;
				return;
			}
		}
		opacity.target_opacity = 0.0f;
		opacity.opacity = 0.0f;
	}
}
