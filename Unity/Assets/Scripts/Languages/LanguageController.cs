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
}

public class LanguageControllerStatic : ILanguageController {
	public const string default_language_key = "English";
	public LanguageModel default_language{
		get{ return load_language(default_language_key); }
	}

	[DontSerialize]
	protected StringReactiveProperty _rx_current_language_key;
	public ReadOnlyReactiveProperty<string> rx_current_language_key;
	[Show]
	public string current_language_key{
		get{ return rx_current_language_key.Value; }
		set{ _rx_current_language_key.Value = value; }
	}
	
	public IMultiLoader<LanguageModel> loader;
	[Show]
	protected Dictionary<string, LanguageModel> loaded_languages
		= new Dictionary<string, LanguageModel>();
	
	[Show]
	public LanguageModel load_language(string lang){
		if (!loaded_languages.ContainsKey(lang)) {
			loaded_languages[lang] = loader.load(lang);
		}
		return loaded_languages[lang];
	}
	
	public string load_text(string key){
		return load_text(key, current_language_key);
	}
	
	public string load_text(string key, string lang){
		LanguageModel given_language = load_language(lang);
		if (given_language.entries.ContainsKey(key))
			return given_language.entries[key];
		if (default_language.entries.ContainsKey(key))
			return "?"+default_language.entries[key];
		return "!"+key;
	}

	[Show]
	protected Dictionary<string, ReadOnlyReactiveProperty<string>> rx_entries
		= new Dictionary<string,ReadOnlyReactiveProperty<string>>();
	public ReadOnlyReactiveProperty<string> rx_load_text(string key){
		if (!rx_entries.ContainsKey(key)){
			rx_entries[key] = rx_current_language_key.Select((lang)=>{
				return load_text(key,lang);
			}).ToReadOnlyReactiveProperty();
		}
		return rx_entries[key];
	}
	
	public ReadOnlyReactiveProperty<string> rx_load_text(StringReactiveProperty rx_key){
		return rx_key.SelectMany(key=>rx_load_text(key)).ToReadOnlyReactiveProperty<string>();
	}
	
	protected string load_language_name(string native_key, string other_key){
		LanguageModel native = load_language(native_key);
		if (native.names.ContainsKey(other_key)){
			return native.names[other_key];
		} else if (native.names.ContainsKey(native_key)){
			return native.names[native_key];
		}
		return "!!"+native_key;
	} 
	
	public string get_language_label(string target_language){
		string native = load_language_name(target_language, target_language);
		string current = load_language_name(target_language, current_language_key);
		if (native != current){
			return current + "(" + native + ")";
		}
		return native;
	}
	
	public LanguageControllerStatic(){
		loader = new LocalFileLoader<LanguageModel>(Application.streamingAssetsPath + "/Data/Languages");
		_rx_current_language_key = new StringReactiveProperty(default_language_key);
		rx_current_language_key = _rx_current_language_key
			.Where(lang=>loader.has_option(lang))
			.ToReadOnlyReactiveProperty<string>();
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
	public string current_language_key{
		get{ return controller.current_language_key; }
		set{ controller.current_language_key = value; }
	}
}