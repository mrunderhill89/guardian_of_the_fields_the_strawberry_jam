using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BreakButton : MonoBehaviour {
	ObjectVisibility visibility;
	// Use this for initialization
	void Start () {
		if (visibility == null){
			visibility = GetComponent<ObjectVisibility>();
		}
	}
	
	public bool should_be_visible(){
		return false;
	}
	// Update is called once per frame
	void Update () {
		visibility.visible = should_be_visible();
	}
}
