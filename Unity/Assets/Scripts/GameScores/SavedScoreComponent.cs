using UnityEngine;
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
using GameScores;

public class SavedScores{
	public ReactiveCollection<Score> rx_scores = new ReactiveCollection<Score> ();
	[Show]
	public List<Score> scores{
		get{
			return rx_scores.ToList();
		}
		set{
			rx_scores.Clear();
			foreach (Score s in value){
				rx_scores.Add(s);
			}
		}
	}

	public static string default_filename {
		get{ return Application.streamingAssetsPath + "/Data/scores.yaml";}
	}

	public SavedScores import(string filename = ""){
		if (filename == "")
			filename = default_filename;
		string Document = File.ReadAllLines(filename).Aggregate("", (string b, string n)=>{
			if (b == "") return n;
			return b+"\n"+n;
		});
		var input = new StringReader(Document);
		var deserializer = new Deserializer(namingConvention: new UnderscoredNamingConvention());
		scores = deserializer.Deserialize<List<Score>>(input);
		return this;
	}

	public static SavedScores import_static(string filename = "")
	{
		return new SavedScores().import(filename);
	}
	
	public SavedScores export(string filename = "")
	{
		if (filename == "")
			filename = default_filename;
		StreamWriter fout = new StreamWriter(filename);
		var serializer = new Serializer();
		serializer.Serialize(fout, this);
		fout.Close();
		return this;
	}

	public SavedScores copy_from(SavedScores that){
		scores = that.scores;
		return this;
	}
}

public class SavedScoreComponent : BetterBehaviour {
	public static SavedScores saved_scores;
	void Awake () {
		saved_scores = SavedScores.import_static ();
	}
}
