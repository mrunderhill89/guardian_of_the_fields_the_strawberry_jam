﻿using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.RepresentationModel;

public class GameStartData : BetterBehaviour {
	[DontSerialize]
	protected static StartData _instance = null;
	public static StartData instance{
		get{
			if (_instance == null) {
				_instance = load_settings();
				if (_instance.randomize) {
					_instance.rng_seed = UnityEngine.Random.seed;
				}
			}
			return _instance;
		}
		private set{
			_instance = value;
		}
	}
	
	
	public StartData current;
	void Awake(){
		if (current == null){
			current = new StartData();
		}
	}

	[Serializable]
	public class StartData{
		[Serialize][Hide]
		protected int _rng_seed;
		[Show]
		public int rng_seed{
			get{ return _rng_seed;}
			set{ _rng_seed = value;}
		}
		[Serialize][Hide]
		protected bool _randomize = false;
		[Show]
		public bool randomize{
			get{ return _randomize;}
			set{ _randomize = value;}
		}

		//Tutorial Settings
		protected bool _tutorial = true;
		public bool tutorial {
			get{ return _tutorial; }
			set{ _tutorial = value; }
		}

		protected bool _debug = false;
		public bool debug {
			get{ return _debug; }
			set{ _debug = value; }
		}

		//Strawberry Settings
		[Serialize][Hide]
		protected int _max_berries_in_field = 40;
		[Show]
		public int max_berries_in_field{
			get{ return _max_berries_in_field;}
			set{ _max_berries_in_field = value;}
		}

		[Serialize][Hide]
		protected float _min_ripeness = 0.0f;
		[Show]
		public float min_ripeness{
			get{ return _min_ripeness;}
			set{ 
				_min_ripeness = value;
				if (max_ripeness < _min_ripeness){
					max_ripeness = _min_ripeness;
				}
			}
		}

		[Serialize][Hide]
		protected float _max_ripeness = 2.0f;
		[Show]
		public float max_ripeness{
			get{ return _max_ripeness;}
			set{ 
				_max_ripeness = value;
				if (min_ripeness > _max_ripeness){
					min_ripeness = _max_ripeness;
				}
			}
		}

		[Serialize][Hide]
		protected float _min_size = 0.06f;
		[Show]
		public float min_size{
			get{ return _min_size;}
			set{ _min_size = value;}
		}

		[Serialize][Hide]
		protected float _max_size = 0.06f;
		[Show]
		public float max_size{
			get{ return _max_size;}
			set{ _max_size = value;}
		}

		[Serialize][Hide]
		protected float _berry_density = 2400.00f;
		[Show]
		public float berry_density{
			get{ return _berry_density;}
			set{ _berry_density = value;}
		}
		
		//Rows and Breaks
		[Serialize][Hide]
		protected int _break_distance = 100;
		[Show]
		public int break_distance{
			get{ return _break_distance;}
			set{ _break_distance = value;}
		}

		[Serialize][Hide]
		protected int _break_length = 10;
		[Show]
		public int break_length{
			get{ return _break_length;}
			set{ _break_length = value;}
		}
		//Break data goes here
		
		//Time of Day & Game Length
		[Serialize][Hide]
		protected float _game_length = 2400.0f; //In Seconds
		[Show]
		public float game_length{
			get{ return _game_length;}
			set{ _game_length = value;}
		}

		[Serialize][Hide]
		protected float _start_hour = 6.0f;//In Hours
		[Show]
		public float start_hour{
			get{ return _start_hour;}
			set{ _start_hour = value;}
		}

		[Serialize][Hide]
		protected float _end_hour = 18.0f; //In Hours
		[Show]
		public float end_hour{
			get{ return _end_hour;}
			set{ _end_hour = value;}
		}
		//Win Condition Settings
		[Serialize][Hide]
		protected float _min_accepted_ripeness = 0.5f;
		[Show]
		public float min_accepted_ripeness{
			get{ return _min_accepted_ripeness;}
			set{ _min_accepted_ripeness = Mathf.Clamp(value, _min_ripeness, _max_ripeness);
				if (max_accepted_ripeness < _min_accepted_ripeness){
					max_accepted_ripeness = _min_accepted_ripeness;
				}
			}
		}

		[Serialize][Hide]
		protected float _max_accepted_ripeness = 1.25f;
		[Show]
		public float max_accepted_ripeness{
			get{ return _max_accepted_ripeness;}
			set{ _max_accepted_ripeness = Mathf.Clamp(value, _min_ripeness, _max_ripeness);
				if (min_accepted_ripeness > _max_accepted_ripeness){
					min_accepted_ripeness = _max_accepted_ripeness;
				}
			}
		}

		[Serialize][Hide]
		protected float _min_berry_weight = 0.00f;
		[Show]
		public float min_berry_weight{
			get{ return _min_berry_weight;}
			set{ _min_berry_weight = value;}
		}

		[Serialize][Hide]
		protected float _min_basket_weight = 15.00f;
		[Show]
		public float min_basket_weight{
			get{ return _min_basket_weight;}
			set{ _min_basket_weight = value;}
		}

		[Serialize][Hide]
		protected float _max_basket_weight = 17.00f;
		[Show]
		public float max_basket_weight{
			get{ return _max_basket_weight;}
			set{ _max_basket_weight = value;}
		}

		[Serialize][Hide]
		protected Dictionary<StrawberryComponent.BerryPenalty, float> _penalty_values
			= new Dictionary<StrawberryComponent.BerryPenalty, float>();
		[Show]	
		public Dictionary<StrawberryComponent.BerryPenalty, float> penalty_values{
			get{ return _penalty_values; }
			set{ _penalty_values = value; }
		}
		//Hazard Data goes here
	}

	public static string default_filepath{
		get{ return Application.dataPath + "/Data/Settings/default.yaml"; }
	}
	public static StartData load_settings(){
		return load_settings(default_filepath);
	}
	[Show]
	public static StartData load_settings(string filename){
		string Document = File.ReadAllLines(filename).Aggregate("", (string b, string n)=>{
			if (b == "") return n;
			return b+"\n"+n;
		});
		var input = new StringReader(Document);
		var deserializer = new Deserializer(namingConvention: new UnderscoredNamingConvention());
		StartData data = deserializer.Deserialize<GameStartData.StartData>(input);
		return data;
	}
	[Show]

	public static void save_settings(StartData data){
		save_settings(data, default_filepath);
	}
	public static void save_settings(StartData data, string filename){
		StreamWriter fout = new StreamWriter(filename);
		var serializer = new Serializer();
		serializer.Serialize(fout, data);
		fout.Close();
	}
}
