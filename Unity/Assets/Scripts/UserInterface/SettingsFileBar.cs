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
	public ObjectVisibility appear_when_different;
	public Button apply;
	public Button revert;
	public GameSettingsComponent data_component;

	// Use this for initialization
	void Start () {
		load.onClick.AddListener(()=>{
			data_component.import(filename.text);
		});
		save.onClick.AddListener(()=>{
			data_component.export(filename.text);
		});
		apply.onClick.AddListener(()=>{
			data_component.apply();
		});
		revert.onClick.AddListener(()=>{
			data_component.revert();
		});
		data_component.rx_is_working_rules.Subscribe((value)=>{
			appear_when_different.visible = !value;
		});
		filename.text = GameSettings.Model.default_filename;
	}
}
