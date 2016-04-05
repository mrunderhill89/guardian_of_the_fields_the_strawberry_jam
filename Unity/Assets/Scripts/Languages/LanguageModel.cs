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