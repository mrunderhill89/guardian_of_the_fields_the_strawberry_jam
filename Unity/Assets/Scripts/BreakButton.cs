using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BreakButton : MonoBehaviour {
	UI_Visibility visibility;
	// Use this for initialization
	void Start () {
		if (visibility == null){
			visibility = GetComponent<UI_Visibility>();
			if (visibility == null){
				visibility = gameObject.AddComponent<UI_Visibility>();
			}
		}
		visibility.add_element(GetComponent<Image>())
		.add_element(GetComponent<Button>())
		.add_element(GetComponentsInChildren<Text>());
	}
	
	public bool should_be_visible(){
		return false;
	}
	// Update is called once per frame
	void Update () {
		visibility.visible = should_be_visible();
	}
}
