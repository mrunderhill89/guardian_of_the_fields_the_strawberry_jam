using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class LanguageTable : BetterBehaviour {
	public enum Language
	{
		English,
		Spanish
	}
	protected static Language default_lang = Language.English;
	protected static Language _current_lang = Language.English;
	[Serialize][Hide]
	protected static Dictionary<Language,Dictionary<string,string>> languages
		= new Dictionary<Language, Dictionary<string, string>>();
	[Show]
	public static Dictionary<string,string> entries{
		get{ return languages[current_lang];}
	}
	static LanguageTable(){
		foreach (Language lang in Language.GetValues(typeof(Language))) {
			if (!languages.ContainsKey(lang)){
				languages[lang] = new Dictionary<string, string> ();
			}
		}
	}
	public static string get(string key){
		return get(key, current_lang);
	}

	public static string get(string key, Language lang){
		if (key == "")
			return "<No entry>";
		if (!languages [_current_lang].ContainsKey (key)) {
			if (!languages [default_lang].ContainsKey (key)) {
				languages[default_lang][key] = "!!"+key;
			}
			if (_current_lang != default_lang){
				languages[_current_lang][key] = "??"+languages[default_lang][key];
			}
		}
		return languages [_current_lang] [key];
	}


	public List<Text> text_objects = new List<Text>();
	public List<TextMesh> text_meshes = new List<TextMesh>();
	[Show]
	public static Language current_lang{
		get{ return _current_lang;}
		set{ 
			_current_lang = value;
			foreach (LanguageTable component in GameObject.FindObjectsOfType<LanguageTable>()){
				component.update();
			}
		}
	}
	public string key = "";
	[Show]
	public string value{
		get { return get(key);}
		set { languages[current_lang][key] = value;}
	}
	[Show]
	public static void delete_entry(string key, Language lang){
		languages[lang].Remove (key);
	}
	[Show]
	public static void fix_entry_name(string old_key, string new_key){
		foreach (LanguageTable component in GameObject.FindObjectsOfType<LanguageTable>()) {
			if (component.key == old_key)
				component.key = new_key;
		}
		string hold;
		foreach (Language lang in Language.GetValues(typeof(Language))) {
			hold = languages[lang][old_key];
			delete_entry(old_key,lang);
			languages[lang][new_key] = hold;
		}
	}
	[Show]
	public static void sync(){
		foreach (Language x in Language.GetValues(typeof(Language))) {
			foreach(string key in languages[x].Keys){
				foreach (Language y in Language.GetValues(typeof(Language))) {
					get (key,y);
				}
			}
		}
	}
	public static string folder_name = "/Text";
	[Show]
	public static void Export(){

	}
	[Show]
	public static void Import(){

	}
	public void Start(){
		update ();
	}
	public void update(){
		foreach (Text tex in text_objects) {
			tex.text = value;
		}
		foreach (TextMesh mesh in text_meshes) {
			mesh.text = value;
		}
	}
}
