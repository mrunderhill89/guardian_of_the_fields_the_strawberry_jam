using UnityEngine;
using System.Collections;

public class HideDebugControls : MonoBehaviour {
	public ObjectVisibility vis;
	void Start () {
		if (vis == null)
			vis = GetComponent<ObjectVisibility>();
		vis.visible = GameStartData.instance.debug;
	}
}
