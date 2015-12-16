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

	public void OnPointerEnter(PointerEventData evn){
		tooltip.show(LanguageTable.get(key));
	}

	public void OnPointerExit(PointerEventData evn){
		tooltip.hide();
	}

}
