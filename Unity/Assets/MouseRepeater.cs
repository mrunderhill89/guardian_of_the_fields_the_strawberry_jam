using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public interface IPointerOrMouseEnterHandler: IPointerEnterHandler {
	void OnMouseEnter();
}

public interface IPointerOrMouseExitHandler: IPointerExitHandler {
	void OnMouseExit();
}

public class MouseRepeater : BetterBehaviour, IPointerOrMouseEnterHandler, IPointerOrMouseExitHandler {
	public List<IPointerOrMouseEnterHandler> pointer_enter = new List<IPointerOrMouseEnterHandler>();
	public List<IPointerOrMouseExitHandler> pointer_exit = new List<IPointerOrMouseExitHandler>();
	public bool use_mouse_events = true;
	public void OnPointerEnter(PointerEventData evn){
		foreach(IPointerOrMouseEnterHandler handler in pointer_enter){
			handler.OnPointerEnter(evn);
		}
	}

	public void OnMouseEnter(){
		if (use_mouse_events){
			foreach(IPointerOrMouseEnterHandler handler in pointer_enter){
				handler.OnMouseEnter();
			}
		}
	}
	
	public void OnPointerExit(PointerEventData evn){
		foreach(IPointerOrMouseExitHandler handler in pointer_exit){
			handler.OnPointerExit(evn);
		}
	}
	
	public void OnMouseExit(){
		if (use_mouse_events){
			foreach(IPointerOrMouseEnterHandler handler in pointer_enter){
				handler.OnMouseEnter();
			}
		}
	}
}
