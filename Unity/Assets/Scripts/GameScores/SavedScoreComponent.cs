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
/*
public class SavedScores{
	public ReactiveCollection<Score> rx_scores = new ReactiveCollection<Score> ();
	[Show]
	public List<Score> scores{
		get{
			return rx_scores.ToList();
		}
		set{
			rx_scores.SetRange(value);
		}
	}
	public int Count{
		get{ return scores.Count;}
	}


	public static string default_filename {
		get{ return Application.streamingAssetsPath + "/Data/Scores.yaml";}
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
		serializer.Serialize(fout, scores);
		fout.Close();
		return this;
	}

	[Show]
	public SavedScores clear()
	{ rx_scores.Clear(); return this; }

	public SavedScores copy_from(SavedScores that){
		scores = that.scores;
		return this;
	}
}
*/

public class SavedScoreController: IDirectoryLoader<Score[]>, ISaver<Score> {
	protected IDirectoryLoader<Score[]> _loader;
	public IDirectoryLoader<Score[]> loader{
		get{ return _loader; }
		private set{ 
			_loader = value;
		}
	}

	protected ISaver<Score[]> _save_all;
	public  ISaver<Score[]> save_all{
		get{ return _save_all; }
		private set{ 
			_save_all = value;
		}
	}

	protected ISaver<Score> _save_one;
	public  ISaver<Score> save_one{
		get{ return _save_one; }
		private set{ 
			_save_one = value;
		}
	}

	public SavedScoreController(){
		var local = new LocalFileLoader<Score[]>();
		loader = local;
		save_all = local;
		save_one = new FileAppendAdapter<Score>(local, local);
	}

	public void load_current(){
		scores = loader.load();
	}
	
	public void save_current(){
		save_all.save(scores);
	}

	public void clear_current(){
		rx_scores.Clear();
	}

	//ILoader
		public Score[] load(){
			return loader.load();
		}
		public ReadOnlyReactiveProperty<Score[]> rx_load(){
			return loader.rx_load();
		}
	//IFileLoader
		public Score[] load(string _file){
			return loader.load(_file);
		}
		public ReadOnlyReactiveProperty<Score[]> rx_load(string file){
			return loader.rx_load(file);
		}
		public string[] available_files(){
			return loader.available_files();
		}
		public bool is_file_available(string file){
			return loader.is_file_available(file);
		}
		
		public string filename{get{ return loader.filename; } set{ loader.filename = value; }}
		
	public ReadOnlyReactiveProperty<string> rx_filename{get{ return loader.rx_filename; }}
	//IDirectoryLoader
		public Score[] load(string _file, string _directory){
			return loader.load(_file, _directory);
		}
		public ReadOnlyReactiveProperty<Score[]> rx_load(string _file, string _directory){
			return loader.rx_load(_file, directory);
		}
		
		public string directory{get {return loader.directory; } set {loader.directory = value;}}
		public ReadOnlyReactiveProperty<string> rx_directory{get{ return loader.rx_directory; }}
	//ISaver
	//Save a single score
	public void save(Score score){
		save_one.save(score);
	}
	public void rx_save(Score score){
		save_one.rx_save(score);
	}

	public ReactiveCollection<Score> rx_scores = new ReactiveCollection<Score> ();
	[Show]
	public Score[] scores{
		get{
			return rx_scores.ToArray();
		}
		set{
			rx_scores.SetRange(value);
		}
	}
	
	public int Count{
		get{ return rx_scores.Count;}
	}
}

public class SavedScoreComponent : BetterBehaviour, IDirectoryLoader<Score[]> {
	protected static SavedScoreController controller = new SavedScoreController();

	//ILoader
	public Score[] load(){
		return controller.load();
	}
	public ReadOnlyReactiveProperty<Score[]> rx_load(){
		return controller.rx_load();
	}

	//IFileLoader
	public Score[] load(string _file){
		return controller.load(_file);
	}
	public ReadOnlyReactiveProperty<Score[]> rx_load(string file){
		return controller.rx_load(file);
	}

	public string filename{ get{ return controller.filename; } set{ controller.filename = value; }}

	public ReadOnlyReactiveProperty<string> rx_filename{get{ return controller.rx_filename; }}

	//IDirectoryLoader
	public Score[] load(string _file, string _directory){
		return controller.load(_file, _directory);
	}

	public ReadOnlyReactiveProperty<Score[]> rx_load(string _file, string _directory){
		return controller.rx_load(_file, _directory);
	}
	public string[] available_files(){
		return controller.available_files();
	}
	public bool is_file_available(string file){
		return controller.is_file_available(file);
	}

	public string directory{get {return controller.directory; } set {controller.directory = value;}}
	public ReadOnlyReactiveProperty<string> rx_directory{get{ return controller.rx_directory; }}
	
	//Internal
	public static void save_static(Score score){
		controller.rx_save(score);
	}
	public static ReactiveCollection<Score> rx_scores{
		get{ return controller.rx_scores; }
	}
	
	public void load_current(){
		controller.load_current();
	}

	public void save_current(){
		controller.save_current();
	}
	
	public void clear_current(){
		controller.clear_current();
	}
	
	void Start(){
		controller.directory = Application.streamingAssetsPath + "/Data";
		controller.filename = "Scores";
		controller.load_current();
	}
}


/*
	[DontSerialize][Show]
	public static SavedScores saved_scores;
	void Awake () {
		saved_scores = SavedScores.import_static ();
	}
	[Show]
	public SavedScoreComponent export(string filename="")
		{export_static (filename); return this;}
	public static void export_static(string filename = ""){
		saved_scores.export(filename);
	}
	[Show]
	public SavedScoreComponent import(string filename="")
		{import_static (filename); return this;}
	public static void import_static(string filename = ""){
		saved_scores.import(filename);
	}
	[Show]
	public SavedScoreComponent clear()
	{ saved_scores.clear(); return this; }
	
	public static void record_score(Score score){
		saved_scores.rx_scores.Add (score);
	}
	public static int Count{
		get{ 
			if (saved_scores == null) return 0;
			return saved_scores.Count;
		}
	}
*/