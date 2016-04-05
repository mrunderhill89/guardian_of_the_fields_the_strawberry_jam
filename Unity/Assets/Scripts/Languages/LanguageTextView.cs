using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.RepresentationModel;
using UniRx;

public class LanguageTextView: BetterBehaviour {
	public Text text;
	public TextMesh mesh;
	public string prefix = "";
	public string suffix = "";
	[Hide]
	public StringReactiveProperty rx_key
		= new StringReactiveProperty("");
	
	[Show]
	public string key{
		get{ return rx_key.Value; }
		set{ rx_key.Value = value; }
	}
	
	[DontSerialize]
	public ReadOnlyReactiveProperty<string> rx_value;
	[Show]
	public string value{
		get{ 
			if (rx_value == null) return "Not yet!";
			return rx_value.Value;
		}
	}
	[DontSerialize]
	public ILanguageController controller;
	
	void Start(){
		if (text == null && GetComponent<Text>() != null){
			text = GetComponent<Text>();
		}
		if (mesh == null && GetComponent<TextMesh>() != null){
			mesh = GetComponent<TextMesh>();
		}
		//Stopgap solution until I can get dependency injection working.
		if (controller == null){
			controller = LanguageController.controller;
		}
		rx_value = controller.rx_load_text(rx_key);
		rx_value.Subscribe((t)=>{
			string full_text = prefix + t + suffix;
			if (text != null)
				text.text = full_text;
			if (mesh != null)
				mesh.text = full_text;
		});
	}
}