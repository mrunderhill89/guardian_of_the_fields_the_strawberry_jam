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

public class LanguageLabelView: BetterBehaviour {
	public List<Text> ui_texts = new List<Text>();
	public List<TextMesh> mesh_texts = new List<TextMesh>();

	public LanguageLabelView add_text(Text text){
		ui_texts.Add(text);
		return this;
	}
	
	public LanguageLabelView add_mesh(TextMesh mesh){
		mesh_texts.Add(mesh);
		return this;
	}
	[Hide]
	public StringReactiveProperty rx_language
		= new StringReactiveProperty("");
	
	[Show]
	public string language{
		get{ return rx_language.Value; }
		set{ rx_language.Value = value; }
	}
	
	[DontSerialize]
	public ReadOnlyReactiveProperty<string> rx_value;
	[Show]
	public string value{
		get{ 
			if (rx_value == null) return language;
			return rx_value.Value;
		}
	}
	[DontSerialize]
	public ILanguageController controller;

	IDisposable sub;
	void Start(){
		if (ui_texts.Count == 0)
			ui_texts.AddRange(GetComponents<Text>());
		if (mesh_texts.Count == 0)
			mesh_texts.AddRange(GetComponents<TextMesh>());
		//Stopgap solution until I can get dependency injection working.
		if (controller == null){
			controller = LanguageController.controller;
		}
		rx_value = controller.rx_get_language_label(rx_language);
		sub = rx_value.Subscribe((t)=>{
			foreach(Text ui in ui_texts){
				if (ui != null){
					ui.text = t;
				}
			}
			foreach(TextMesh mesh in mesh_texts){
				if (mesh != null){
					mesh.text = t;
				}
			}
		});
	}
	
	void OnDestroy(){
		if (sub != null)
			sub.Dispose();
	}
}