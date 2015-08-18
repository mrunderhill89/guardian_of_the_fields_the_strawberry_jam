using UnityEngine;
using System.Collections;
using UniRx;
public class Draggable : MonoBehaviour {
	private Vector3 screenPoint;
	private Vector3 offset;
	public Subject<Vector3> drag_from;
	public Subject<Vector3> drag_to;
	public Subject<Vector3> release_on;
	public bool allow_dragging = true;
	public float arms_length = 0.5f;

	void Start(){
		drag_from = new Subject<Vector3> ();
		drag_to = new Subject<Vector3> ();
		release_on = new Subject<Vector3> ();
	}

	void OnMouseDown(){
		drag_from.OnNext (gameObject.transform.position);
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		offset = Camera.main.WorldToViewportPoint(gameObject.transform.position) - Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}
	
	void OnMouseDrag(){
		if (allow_dragging) {
			Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			Vector3 cursorToView = Camera.main.ScreenToViewportPoint(cursorPoint) + offset;
			cursorToView.z = arms_length;
			Vector3 cursorPosition = Camera.main.ViewportToWorldPoint(cursorToView);
			transform.position = cursorPosition;
			drag_to.OnNext (cursorPosition);
		}
	}

	void OnMouseUp(){
		release_on.OnNext (gameObject.transform.position);
	}
}
