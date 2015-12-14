using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Vexe.Runtime.Types;
using UniRx;

public class SettingsFileBar : BetterBehaviour {
	public InputField filename;
	public Button load;
	public Button save;
	public Button set_to_current;
	public SettingsForm form;

	// Use this for initialization
	void Start () {
		load.onClick.AddListener(()=>{
			form.data_component.import(filename.text);
		});
		save.onClick.AddListener(()=>{
			form.data_component.export(filename.text);
		});
		set_to_current.onClick.AddListener(()=>{
			form.data_component.apply();
		});
		filename.text = GameSettings.Model.default_filename;
	}
}
