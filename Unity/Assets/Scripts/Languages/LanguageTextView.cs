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
	public List<Text> ui_texts = new List<Text>();
	public List<TextMesh> mesh_texts = new List<TextMesh>();

	public LanguageTextView add_text(Text text){
		ui_texts.Add(text);
		return this;
	}
	
	public LanguageTextView add_mesh(TextMesh mesh){
		mesh_texts.Add(mesh);
		return this;
	}

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
			if (rx_value == null) return key;
			return rx_value.Value;
		}
	}
	[DontSerialize]
	public ILanguageController controller;
	
	void Start(){
		if (ui_texts.Count == 0)
			ui_texts.AddRange(GetComponents<Text>());
		if (mesh_texts.Count == 0)
			mesh_texts.AddRange(GetComponents<TextMesh>());
		//Stopgap solution until I can get dependency injection working.
		if (controller == null){
			controller = LanguageController.controller;
		}
		rx_value = controller.rx_load_text(rx_key);
		rx_value.Subscribe((t)=>{
			string full_text = prefix + t + suffix;
			foreach(Text ui in ui_texts){
				ui.text = full_text;
			}
			foreach(TextMesh mesh in mesh_texts){
				mesh.text = full_text;
			}
		});
	}
}