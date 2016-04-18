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

public interface ILanguageController{
	string load_text(string key);
	string load_text(string key, string lang);
	string get_language_label(string lang);
	ReadOnlyReactiveProperty<string> rx_load_text(string key);
	ReadOnlyReactiveProperty<string> rx_load_text(StringReactiveProperty rx_key);
	ReadOnlyReactiveProperty<string> rx_get_language_label(string target_language);
	ReadOnlyReactiveProperty<string> rx_get_language_label(StringReactiveProperty rx_lang);
	ReadOnlyReactiveProperty<string> rx_current_language_key {get;}
}

public class LanguageControllerStatic : ILanguageController {
	protected Dictionary<string, ReactiveProperty<LanguageModel>> _rx_languages = new Dictionary<string,ReactiveProperty<LanguageModel>>();
	public ReactiveProperty<LanguageModel> get_language_property(string key){
		if (!_rx_languages.ContainsKey(key)){
			_rx_languages[key] = new ReactiveProperty<LanguageModel>();
		}
		return _rx_languages[key];
	}
	public LanguageModel get_language(string key){
		return get_language_property(key).Value;
	}
	public void set_language(string key, LanguageModel value){
		get_language_property(key).Value = value;
	}

	protected const string default_language_key = "English";
	public LanguageModel default_language{
		get{
			return get_language(default_language_key);
		}
	}
	protected StringReactiveProperty _rx_current_language_key
		= new StringReactiveProperty("");
	public ReadOnlyReactiveProperty<string> rx_current_language_key{
		get{ return _rx_current_language_key.ToReadOnlyReactiveProperty<string>(); }
	}
	
	[Show]
	public string current_language_key{
		get{ return _rx_current_language_key.Value;}
		set{ _rx_current_language_key.Value = value; }
	}

	public bool has_language(string lang){
		return _rx_languages.ContainsKey(lang);
	}
	public bool has_text(string key){
		return has_text(key, current_language_key);
	}
	public bool has_text(string key, string lang){
		return has_language(lang) && get_language(lang).entries.ContainsKey(key);
	}
	
	public string load_text(string key){
		return load_text(key, current_language_key);
	}
	
	public string load_text(string key, string lang){
		if (lang == "")
			lang = default_language_key;
		return prioritize_languages(key, get_language(lang), default_language);
	}

	private string prioritize_languages(string key, LanguageModel first, LanguageModel second){
		if (first != null && first.entries.ContainsKey(key))
			return first.entries[key];
		if (second != null && second.entries.ContainsKey(key))
			return "?"+second.entries[key];
		return "!"+key;
	}

	protected Dictionary<string, ReadOnlyReactiveProperty<string>> properties
		= new Dictionary<string, ReadOnlyReactiveProperty<string>>();
	public ReadOnlyReactiveProperty<string> rx_load_text(string key){
		if (!properties.ContainsKey(key)){
			properties[key] = _rx_current_language_key.SelectMany(lang_key=>{
				if (lang_key == "")
					lang_key = default_language_key;
				return get_language_property(lang_key)
					.Select(lang_model => prioritize_languages(key, lang_model, default_language));
			}).ToReadOnlyReactiveProperty<string>();
		}
		return properties[key];
	}
	
	public ReadOnlyReactiveProperty<string> rx_load_text(StringReactiveProperty rx_key){
		return rx_key.SelectMany(key => rx_load_text(key)).ToReadOnlyReactiveProperty<string>();
	}
	
	public string get_language_label(string target_language){
		return get_language_label(target_language, current_language_key);
	}
	
	public string get_language_label(string target_language, string viewing_language){
		if (!has_language(target_language))
			return target_language;
		return get_language(target_language).get_language_label(viewing_language);
	}
	
	public ReadOnlyReactiveProperty<string> rx_get_language_label(StringReactiveProperty rx_key){
		return rx_key.SelectMany(key=>rx_get_language_label(key)).ToReadOnlyReactiveProperty<string>();
	}

	public ReadOnlyReactiveProperty<string> rx_get_language_label(string target_language){
		return get_language_property(target_language).CombineLatest(
			rx_current_language_key,
			(target,viewing)=>target.get_language_label(viewing)
		).ToReadOnlyReactiveProperty<string>();
	}
	
	public void load_all(IMultiLoader<LanguageModel> loader){
		foreach(string opt in loader.get_options()){
			set_language(opt, loader.load(opt));
		}
	}
}

public class LanguageController: BetterBehaviour, ILanguageController {
	[DontSerialize][Show]
	public static LanguageControllerStatic controller = new LanguageControllerStatic();	
	public string load_text(string key){ return controller.load_text(key); }
	public string load_text(string key, string lang){ return controller.load_text(key, lang); }
	public string get_language_label(string lang){ return controller.get_language_label(lang); }
	public ReadOnlyReactiveProperty<string> rx_load_text(string key)
		{ return controller.rx_load_text(key);}
	public ReadOnlyReactiveProperty<string> rx_load_text(StringReactiveProperty rx_key)
		{ return controller.rx_load_text(rx_key);}

	public ReadOnlyReactiveProperty<string> rx_get_language_label(string key)
		{ return controller.rx_get_language_label(key);}
	public ReadOnlyReactiveProperty<string> rx_get_language_label(StringReactiveProperty rx_key)
		{ return controller.rx_get_language_label(rx_key);}

	public ReadOnlyReactiveProperty<string> rx_current_language_key{
		get{ return controller.rx_current_language_key; }
	}
	public string current_language_key{
		get{ return controller.current_language_key; }
		set{ controller.current_language_key = value; }
	}
	
	void Start(){
		LocalFileLoader<LanguageModel> local = new LocalFileLoader<LanguageModel>(Application.streamingAssetsPath + "/Data/Languages");
		controller.load_all(local);
	}
}