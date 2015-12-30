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

public class LanguageDictionary{
	public Dictionary<string,Dictionary<string,string>> languages;
	public Dictionary<string,ReadOnlyReactiveProperty<string>> observables;
	public StringReactiveProperty current_language;
	string default_language = "English";
	public LanguageDictionary(){
		languages = new Dictionary<string, Dictionary<string, string>> ();
		observables = new Dictionary<string, ReadOnlyReactiveProperty<string>> ();
		current_language = new StringReactiveProperty("");
	}
	public string get(string key, bool read_only = false){
		return get (key, current_language.Value, read_only);
	}
	public bool has(string key){
		return has(key, current_language.Value);
	}
	public bool has(string key, string lang){
		if (!languages.ContainsKey(lang))
			return false;
		return languages[lang].ContainsKey(key);
	}
	public bool has_any(string key){
		foreach(Dictionary<string, string> dictionary in languages.Values){
			if (dictionary.ContainsKey(key)) return true;
		}
		return false;
	}
	public string get(string key, string lang, bool read_only = false, string wrong_language = "??", string no_entry = "!!"){
		if (lang == "") {
			if (current_language.Value == ""){
				lang = default_language;
			} else {
				lang = current_language.Value;
			}
		}
		if (key == "") 
			key = "no_key";
		if (!read_only){
			if (!languages.ContainsKey(lang)){
				languages[lang] = new Dictionary<string, string>();
			}
			if (!languages[lang].ContainsKey(key)){
				if (languages[default_language].ContainsKey(key)){
					languages[lang][key] = wrong_language+languages[default_language][key];
				} else {
					languages[lang][key] = no_entry+key;
				}
			}
		}
		if (!languages [lang].ContainsKey (key))
			return "";
		return languages [lang] [key];
	}

	public ReadOnlyReactiveProperty<string> get_property(string key, bool read_only = false){
		if (!observables.ContainsKey (key)) {
			observables[key] = current_language.Select ((string lang) => {
				return get (key, lang, read_only);
			}).ToReadOnlyReactiveProperty<string>();
		}
		return observables [key];
	}
	[Serialize][Hide]
	static string _default_filename = "";
	public string default_filename {
		get {
			try{
				_default_filename = Application.streamingAssetsPath + "/Data/Languages.yaml";
			}
			catch(ArgumentException){
				if (_default_filename == ""){
					_default_filename = "./StreamingAssets/Data/Languages.yaml";
				}
			}
			//I know, I know! get_streamingAssetsPath can only be run from the main thread.
			//But it's a stupid restriction, so just use best path you can get.
			return _default_filename;
		}
	}
	public LanguageDictionary import(){
		return import (default_filename);
	}
	[Show]
	public LanguageDictionary import(string filename){
		if (filename == null || filename == "")
			filename = default_filename;
		string Document = File.ReadAllLines(filename).Aggregate("", (string b, string n)=>{
			if (b == "") return n;
			return b+"\n"+n;
		});
		var input = new StringReader(Document);
		var deserializer = new Deserializer(namingConvention: new UnderscoredNamingConvention());
		languages = deserializer.Deserialize<Dictionary<string, Dictionary<string,string>>>(input);
		return this;
	}
	public LanguageDictionary export(){
		return export (default_filename);
	}
	[Show]
	public LanguageDictionary export(string filename){
		if (filename == null || filename == "")
			filename = default_filename;
		StreamWriter fout = new StreamWriter(filename);
		var serializer = new Serializer();
		serializer.Serialize(fout, languages);
		fout.Close();
		return this;
	}
}

public class LanguageTable : BetterBehaviour {
	protected static LanguageDictionary _dictionary = null;
	[Show]
	public static LanguageDictionary dictionary{
		get{
			if (_dictionary == null){
				dictionary = new LanguageDictionary().import();
			}
			return _dictionary;
		}
		private set {
			_dictionary = value;
		}
	}
	public Dictionary<string, List<Text>> text_objects = new Dictionary<string, List<Text>>();
	public Dictionary<string, List<TextMesh>> text_meshes = new Dictionary<string, List<TextMesh>>();
	[DontSerialize]
	public List<IDisposable> disposables = new List<IDisposable>();
	public int Count{
		get{
			return text_objects.Count + text_meshes.Count;
		}
	}
	[Serialize][Hide]
	protected string _language = "";
	[Show]
	public string language{
		get{ 
			return _language;
		}
		set{ _language = value;}
	}

	[Serialize][Hide]
	protected string _key = "";
	[Show]
	public string key{
		get{ 
			if (_key == "") _key = name;
			return _key;
		}
		set{ _key = value; }
	}
	[Show]
	public string value{
		get{
			return get(key,language,true);
		}
	}

	[Serialize][Hide]
	protected bool _auto_assign = true;
	[Show]
	public bool auto_assign{
		get{ return _auto_assign;}
		set{ _auto_assign = value;}
	}

	[Serialize][Hide]
	protected string _prefix = "";
	[Show]
	public string prefix{
		get{ return _prefix;}
		set{ _prefix = value;}
	}

	[Serialize][Hide]
	protected string _suffix = "";
	[Show]
	public string suffix{
		get{ return _suffix;}
		set{ _suffix = value;}
	}

	void Start(){
		if (auto_assign && Count == 0) {
			if (GetComponent<Text>() != null){
				SubscribeText(GetComponent<Text>());
			} else if(GetComponent<TextMesh>() != null){
				SubscribeMesh(GetComponent<TextMesh>());
			}
		}
		if (text_objects != null) {
			foreach (KeyValuePair<string,List<Text>> text_list in text_objects) {
				foreach (Text text in text_list.Value) {
					SubscribeText (text_list.Key, text);
				}
			}
		}
		if (text_meshes != null) {
			foreach (KeyValuePair<string,List<TextMesh>> mesh_list in text_meshes) {
				foreach (TextMesh mesh in mesh_list.Value) {
					SubscribeMesh (mesh_list.Key, mesh);
				}
			}
		}
	}

	void OnDestroy(){
		foreach (IDisposable disposable in disposables) {
			disposable.Dispose();
		}
	}

	public LanguageTable SubscribeText (Text target){
		return SubscribeText (key, target);
	}
	public LanguageTable SubscribeText (string key, Text target){
		if (key == "")
			key = this.key;
		disposables.Add(dictionary.get_property (key).Subscribe ((string value) => {
			target.text = build_text(key,value);
		}));
		return this;
	}

	public LanguageTable SubscribeMesh (TextMesh target){
		return SubscribeMesh(key, target);
	}

	public LanguageTable SubscribeMesh (string key, TextMesh target){
		if (key == "")
			key = this.key;
		disposables.Add(dictionary.get_property (key).Subscribe ((string value) => {
			target.text = build_text(key,value);
		}));
		return this;
	}

	string build_text(string key, string value){
		if (language != "" && language != dictionary.current_language.Value) {
			string native_value = get(key,language);
			if (native_value != value)
				return prefix + native_value + " (" + value + ")" + suffix;
		}
		return prefix + value + suffix;
	}

	public void set_global_language(string lang){
		dictionary.current_language.Value = lang;
	}

	public static string get(string key, string language = "", bool read_only = false){
		if (!dictionary.languages.ContainsKey(language))
			language = dictionary.current_language.Value;
		return dictionary.get(key, language, read_only);
	}

	public static ReadOnlyReactiveProperty<string> get_property(string key, bool read_only = false){
		return dictionary.get_property(key,read_only);
	}
	
	public static bool has(string key, string language= ""){
		if (!dictionary.languages.ContainsKey(language))
			language = dictionary.current_language.Value;
		return dictionary.has(key, language);
	}
	
	public static bool has_any(string key){
		return dictionary.has_any(key);
	}
}
