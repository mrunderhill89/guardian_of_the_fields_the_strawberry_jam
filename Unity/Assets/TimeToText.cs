using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeToText : MonoBehaviour {
	public Text text_component;
	public GameTimer timer;
	void Start(){
		if (text_component == null)
			text_component = GetComponent<Text> ();
		if (timer == null)
			timer = GetComponent<GameTimer> ();
	}
	void Update () {
		text_component.text = timer.ToString ();
	}
}
