using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
[ExecuteInEditMode]
public class PaceNameLabel : BetterBehaviour {
	public PaceManager pace;
	public Slider slider;
	public Text text;
	public SortedList<float,string> pace_names = new SortedList<float,string>();
	void Start(){
		if (slider != null){
			slider.onValueChanged.AddListener(this.update);
			this.update(slider.value);
		}
	}
	
	void update(float value){
		if (pace != null && text != null){
			foreach(KeyValuePair<float,string> pair in pace_names){
				if (value < pair.Key) break;
				text.text = LanguageTable.get(pair.Value);
			}
		}
	}
}
