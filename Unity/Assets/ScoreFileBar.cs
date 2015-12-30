using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class ScoreFileBar : BetterBehaviour {
	public InputField file_input;
	public Button load;
	public Button save;
	public Button clear;
	public SavedScoreComponent data_component;
	// Use this for initialization
	void Start () {
		file_input.text = SavedScores.default_filename;
		load.onClick.AddListener(()=>{
			data_component.import(file_input.text);
		});
		save.onClick.AddListener(()=>{
			data_component.export(file_input.text);
		});
		clear.onClick.AddListener(()=>{
			data_component.clear();
		});
	}
}
