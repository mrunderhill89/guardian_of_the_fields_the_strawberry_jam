using UnityEngine;
using System.Collections;
using UniRx;
public class Draggable : MonoBehaviour {
	public float min_reach = 0.1f;
	public float max_reach = 1.0f;
	public float min_drop = 0.1f;
	public float max_drop = 2.0f;
	public Rigidbody body = null;
	public bool gravity_on_drop = true;
	public bool kinematic_on_pickup = false;
	private Vector3 viewportPoint;
	private Vector3 offset;
	private Vector3 hold_at;

	void Start(){
		body = gameObject.GetComponent<Rigidbody> ();
	}
	
	void OnMouseDown()
	{
		viewportPoint = Camera.main.WorldToViewportPoint(gameObject.transform.position);		
		hold_at = new Vector3(Input.mousePosition.x, Input.mousePosition.y, (min_drop+max_drop)/2.0f);
		offset = viewportPoint - Camera.main.ScreenToViewportPoint(hold_at);
		if (body != null) {
			body.useGravity = false;
			body.isKinematic = kinematic_on_pickup;
		}
	}
	
	void OnMouseDrag()
	{
		Vector3 current_cursor = new Vector3(Input.mousePosition.x, Input.mousePosition.y, hold_at.z);
		Vector3 curPosition = calculate_delta(current_cursor) + offset;
		transform.position = Camera.main.ViewportToWorldPoint(curPosition);
	}

	void OnMouseUp(){
		if (body != null) {
			body.useGravity = gravity_on_drop;
		}
	}

	Vector3 calculate_delta(Vector3 current){
		Vector3 hold_viewport = Camera.main.ScreenToViewportPoint (hold_at);
		Vector3 delta = Camera.main.ScreenToViewportPoint (current) - hold_viewport;
		delta.z = -delta.y;
		delta.y = 0.0f;
		Vector3 result = delta + hold_viewport;
		result.z = Mathf.Clamp(result.z, min_drop, max_drop);
		return result;
	}
}
