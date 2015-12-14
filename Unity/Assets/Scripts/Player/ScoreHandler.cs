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

public class ScoreHandler : BetterBehaviour {
	public StrawberryStateMachine berry_state;
	[Serializable]
	public class TotalScore{
		protected Dictionary<string, StrawberryScore> _strawberries = 
			new Dictionary<string, StrawberryScore>();
		[Show]
		public Dictionary<string, StrawberryScore> strawberries{
			get{ return _strawberries;}
			private set{ _strawberries = value;}
		}
		protected BasketScore _baskets;
		[Show]
		public BasketScore baskets{
			get{ return _baskets;}
			private set{ _baskets = value;}
		}

		protected DateTime _date_recorded;
		public DateTime date_recorded{
			get{ return _date_recorded; }
			set{ _date_recorded = value; }
		}

		protected float _play_length = 0.0f;
		public float play_length{
			get{ return _play_length; }
			set{ _play_length = value;}
		}

		public GameSettings.Model settings {get; set;}

		public TotalScore(StrawberryScore picked, StrawberryScore dropped, BasketScore bs){
			strawberries["fall"] = picked;
			strawberries["basket"] = dropped;
			baskets = bs;
		}

		public TotalScore clone(TotalScore that){
			settings.copy_from (that.settings);
			date_recorded = that.date_recorded;
			play_length = that.play_length;
			foreach (KeyValuePair<string,StrawberryScore> kvp in strawberries){
				if (that.strawberries.ContainsKey(kvp.Key)){
					kvp.Value.clone(that.strawberries[kvp.Key]);
				}
			}
			baskets.clone(that.baskets);
			return this;
		}
		public TotalScore() : this(new StrawberryScore(), new StrawberryScore(), new BasketScore()) {}
	}
	[Serializable]
	public class StrawberryScore{
		protected int _ripe = 0;
		[Show]
		public int ripe {
			get{ return _ripe;}
			private set{ _ripe = value;}
		}
		protected int _overripe = 0;
		[Show]
		public int overripe {
			get{ return _overripe;}
			private set{ _overripe = value;}
		}
		protected int _underripe = 0;
		[Show]
		public int underripe {
			get{ return _underripe;}
			private set{ _underripe = value;}
		}
		protected int _undersize = 0;
		[Show]
		public int undersize {
			get{ return _undersize;}
			private set{ _undersize = value;}
		}
		public void reset(){
			ripe = 0;
			overripe = 0;
			underripe = 0;
			undersize = 0;
		}
		public StrawberryScore clone(StrawberryScore that){
			ripe = that.ripe;
			overripe = that.overripe;
			underripe = that.underripe;
			undersize = that.undersize;
			return this;
		}
		public StrawberryScore get_from_state(StrawberryStateMachine berry_state, string state_name){
			reset();
			foreach (StrawberryComponent berry in berry_state.get_strawberries(state_name)){
				if (berry.is_under_size()){undersize++;}
				if (berry.is_over_ripe()){
					overripe++;
				} else if (berry.is_under_ripe()){
					underripe++;
				} else if (!berry.is_under_size()){
					ripe++;
				}
			}
			return this;
		}
	}
	[Serializable]
	public class BasketScore{
		protected int _accepted = 0;
		[Show]
		public int accepted{
			get{ return _accepted; }
			set{ _accepted = value; }
		}

		protected int _overweight = 0;
		[Show]
		public int overweight{
			get{ return _overweight; }
			set{ _overweight = value; }
		}

		protected int _underweight = 0;
		[Show]
		public int underweight{
			get{ return _underweight; }
			set{ _underweight = value; }
		}

		protected int _overflow = 0;
		[Show]
		public int overflow{
			get{ return _overflow; }
			set{ _overflow = value; }
		}

		public void reset(){
			accepted = 0;
			overweight = 0;
			underweight = 0;
			overflow = 0;
		}
		public BasketScore clone(BasketScore that){
			accepted = that.accepted;
			overweight = that.overweight;
			underweight = that.underweight;
			overflow = that.overflow;
			return this;
		}
		public BasketScore get_from_current(){
			reset();
			foreach(BasketComponent basket in BasketComponent.baskets){
				if (basket.is_overflow()){
					overflow++;
				}
				if (basket.is_overweight()){overweight++;} 
				else if (basket.is_underweight()){underweight++;}
				else if (!basket.is_overflow()){accepted++;}
			}
			return this;
		}
	}
	[DontSerialize][Show]
	protected TotalScore _current_score;
	public TotalScore current_score{
		get{
			if (_current_score == null){
				current_score = new TotalScore();
			}
			return _current_score;
		}
		set{ _current_score = value;}
	}

	[DontSerialize][Show]
	public List<TotalScore> saved_scores = new List<TotalScore>();
	[Show]
	public void record_score(){
		current_score.date_recorded = DateTime.Now;
		saved_scores.Add(new TotalScore().clone(current_score));
	}

	public static string default_filepath{
		get{ return Application.streamingAssetsPath + "/Data/Scores.yaml"; }
	}
	[Show]
	public void load_scores(){ load_scores (default_filepath);}
	[Show]
	public void load_scores(string filename){
		if (saved_scores == null){
			saved_scores = new List<TotalScore>();
		}
		string Document = File.ReadAllLines(filename).Aggregate("", (string b, string n)=>{
			if (b == "") return n;
			return b+"\n"+n;
		});
		var input = new StringReader(Document);
		var deserializer = new Deserializer(namingConvention: new UnderscoredNamingConvention());
		saved_scores = deserializer.Deserialize<List<TotalScore>>(input);
	}
	[Show]
	public void save_scores(){ save_scores (default_filepath);}
	[Show]
	public void save_scores(string filename){
		StreamWriter fout = new StreamWriter(filename);
			var serializer = new Serializer();
			serializer.Serialize(fout, saved_scores);
		fout.Close();
	}

	public bool lock_strawberries = false;
	public bool lock_baskets = false;
	
	void Awake(){
		current_score = new TotalScore();
		current_score.settings = GameSettingsComponent.working_rules;
		load_scores();
	}
	
	void Update(){
		if (!lock_strawberries){
			current_score.strawberries["fall"].get_from_state(berry_state, "fall");
			current_score.strawberries["basket"].get_from_state(berry_state, "basket");
		}
		if (!lock_baskets){
			current_score.baskets.get_from_current();
		}
	}
}
