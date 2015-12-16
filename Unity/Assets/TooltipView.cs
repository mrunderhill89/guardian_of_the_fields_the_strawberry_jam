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
	
	void Start(){
		vis = ObjectVisibility.GetVisibility(gameObject, vis);
		hide();
	}
	
	void Update(){
		if (vis.visible){
			transform.position = Input.mousePosition;
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
