using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class TooltipView : BetterBehaviour {
	public Text text;
	public ObjectVisibility vis;
	public float x_padding = 160;
	public float y_padding = 30;

	void Start(){
		vis = ObjectVisibility.GetVisibility(gameObject, vis);
		hide();
	}
	
	void Update(){
		if (vis.visible){
			transform.position = new Vector3(
				Mathf.Clamp(Input.mousePosition.x, x_padding, Screen.width - x_padding),
				Mathf.Clamp(Input.mousePosition.y, y_padding, Screen.height - y_padding),
				transform.position.z
			);

		}
	}
	
	public void show(string msg = ""){
		if (msg != "") 
			text.text = msg;
		vis.visible = true;
	}
	
	public void hide(){
		vis.visible = false;
	}
}
