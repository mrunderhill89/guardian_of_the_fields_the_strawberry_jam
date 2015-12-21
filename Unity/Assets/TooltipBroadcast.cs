using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class TooltipBroadcast : BetterBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public TooltipView tooltip;
	public string key = "";

	void Start(){
		if (tooltip == null) {
			foreach(TooltipView t in GameObject.FindObjectsOfType<TooltipView>()){
				if (t != null){
					tooltip = t; break;
				}
			}
		}
	}

	public void OnPointerEnter(PointerEventData evn){
		if (tooltip != null) {
			tooltip.show (LanguageTable.get (key));
		}
	}

	public void OnPointerExit(PointerEventData evn){
		if (tooltip != null) {
			tooltip.hide ();
		}
	}

}
