using UnityEngine;
using System;
using System.Collections.Generic;
using Vexe.Runtime.Types;

public class LanguageModel {
	protected Dictionary<string,string> _names = new Dictionary<string,string>();
	[Show]
	public Dictionary<string,string> names{
		get{ return _names; }
		set{ 
			_names = value;
		}
	}
	
	public string get_language_label(string other_language){
		string native = names["Native"];
		if (names.ContainsKey(other_language) && names[other_language] != native){
			return names[other_language]+" ("+native+")";
		}
		return native;
	}

	public int Count(){ return _entries.Count; }
	
	protected Dictionary<string,string> _entries = new Dictionary<string,string>();
	[Show]
	public Dictionary<string,string> entries{
		get{ return _entries; }
		set{ 
			_entries = value;
		}
	}
	
	public string image_name {get; set;}
}