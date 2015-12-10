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
			form.data_component.load_settings(filename.text);
		});
		save.onClick.AddListener(()=>{
			form.data_component.save_settings(filename.text);
		});
		set_to_current.onClick.AddListener(()=>{
			GameStartData.instance = form.data_component.current.Value;
			set_to_current.gameObject.SetActive(false);
		});
		form.using_main.Subscribe((main)=>{
			set_to_current.gameObject.SetActive(!main);
		});
		filename.text = GameStartData.default_filepath;
	}
}
