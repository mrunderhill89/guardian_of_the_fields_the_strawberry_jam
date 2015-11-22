using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.RepresentationModel;

public class LanguageTable : BetterBehaviour {
	public List<Text> text_objects = new List<Text>();
	public List<TextMesh> text_meshes = new List<TextMesh>();
	[Serialize][Hide]
	protected string _key = "";
	[Show]
	public string key {
		get{return _key;}
		set{ 
			_key = value;
			update();
		}
	}
	[Show]
	public string value {
		get{
			return get(key);
		}
		set{
			languages[current_language][key] = value;
			update();
		}
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
	
	//Static Elements Go Here
	[Show]
	static Dictionary<string, Dictionary<string,string>> languages = new Dictionary<string, Dictionary<string,string>>();
	[Serialize][Show]
	public static string current_language = "English";
	[Serialize][Hide]
	static string default_language = "English";
	static LanguageTable(){
		Import("Assets/Languages.yaml");
	}
	public static Dictionary<string,string> get_language(string lang){
		if (!languages.ContainsKey(lang)){
			languages[lang] = new Dictionary<string,string>();
		}
		return languages[lang];
	}
	
	public static string get(string key, bool read_only = false){
		return get(key, current_language, read_only);
	}
	public static string get(string key, string lang, bool read_only = false){
		if (key == "") return get("no_key",lang,false);
		if (!get_language(lang).ContainsKey(key)){
			if (read_only){
				return "<No Entry>";
			} else {
				if(get_language(default_language).ContainsKey(key)){
					get_language(lang)[key] = "??"+get_language(default_language)[key];
				} else {
					get_language(lang)[key] = "!!"+key;
				}
			}
		}
		return get_language(lang)[key];
	}
	
	[Show]
	public static void Export(string filename = "Assets/Languages.yaml"){
		StreamWriter fout = new StreamWriter(filename);
			var serializer = new Serializer();
			serializer.Serialize(fout, languages);
		fout.Close();
	}
	[Show]
	public static void Import(string filename = "Assets/Languages.yaml"){
		string Document = File.ReadAllLines(filename).Aggregate("", (string b, string n)=>{
			if (b == "") return n;
			return b+"\n"+n;
		});
		var input = new StringReader(Document);
		var deserializer = new Deserializer(namingConvention: new UnderscoredNamingConvention());
		languages = deserializer.Deserialize<Dictionary<string, Dictionary<string,string>>>(input);
	}
	[Show]
	public static void Refresh(){
		foreach (LanguageTable comp in GameObject.FindObjectsOfType<LanguageTable>()){
			comp.update();
		}
	}
}
