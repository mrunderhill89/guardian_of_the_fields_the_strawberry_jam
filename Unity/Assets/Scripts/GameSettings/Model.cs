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

namespace GameSettings{
	[Serializable]
	public class Model
	{
		#region Properties
		public Randomness randomness { get; set;}
		public Flags flags {get; set;}
		public StrawberryGeneration strawberry {get; set;}
		public Breaks breaks {get; set;}
		public WinCondition win_condition {get; set;}
		public GameSettings.Time time {get; set;}

		private static string _default_filename = "";
		public static string default_filename{
			get{
				try{
					_default_filename = Application.streamingAssetsPath + "/Data/Settings/default.yaml";
				}
				catch(ArgumentException){
					if (_default_filename == ""){
						_default_filename = "./StreamingAssets/Data/Settings/default.yaml";
					}
				}
				return _default_filename;
			}
		}
		public const float precision = 100.0f;
		#endregion

		#region Public methods
		public Model(){
			randomness = new Randomness ();
			flags = new Flags ();
			strawberry = new StrawberryGeneration ();
			breaks = new Breaks ();
			win_condition = new WinCondition ();
			time = new Time ();
			/*
			 * These sanity checks are meant to prevent a situation
			 * where none of the player's strawberries are accepted.
			 * Basically, there should always be some overlap between
			 * the possible range and the accepted range.
			 */
			strawberry.rx_min_ripeness
				.Subscribe ((value) => {
					if (win_condition.max_ripeness < value){
						win_condition.max_ripeness = value;
					}
					if (win_condition.min_ripeness < value){
						win_condition.min_ripeness = value;
					}
				});
			strawberry.rx_max_ripeness
				.Subscribe ((value) => {
					if (win_condition.min_ripeness > value){
						win_condition.min_ripeness = value;
					}
					if (win_condition.max_ripeness > value){
						win_condition.max_ripeness = value;
					}
				});
			win_condition.rx_min_ripeness
				.Subscribe ((value) => {
					if (strawberry.max_ripeness < value){
						strawberry.max_ripeness = value;
					}
					if (strawberry.min_ripeness > value){
						strawberry.min_ripeness = value;
					}
				});
			win_condition.rx_max_ripeness
				.Subscribe ((value) => {
					if (strawberry.min_ripeness > value){
						strawberry.min_ripeness = value;
					}
					if (strawberry.max_ripeness < value){
						strawberry.max_ripeness = value;
					}
				});
			initialize ();
		}

		public static Model import(string filename = "")
		{
			if (filename == "")
				filename = default_filename;
			string Document = File.ReadAllLines(filename).Aggregate("", (string b, string n)=>{
				if (b == "") return n;
				return b+"\n"+n;
			});
			var input = new StringReader(Document);
			var deserializer = new Deserializer(namingConvention: new UnderscoredNamingConvention());
			Model data = deserializer.Deserialize<Model>(input);
			return data;
		}

		public Model export(string filename = "")
		{
			if (filename == "")
				filename = default_filename;
			StreamWriter fout = new StreamWriter(filename);
			var serializer = new Serializer();
			serializer.Serialize(fout, this);
			fout.Close();
			return this;
		}

		public Model initialize(){
			flags.initialize ();
			randomness.initialize ();
			strawberry.initialize ();
			time.initialize ();
			breaks.initialize ();
			win_condition.initialize ();
			return this;
		}

		public Model copy_from(Model that)
		{
			flags.copy_from (that.flags);
			randomness.copy_from (that.randomness);
			strawberry.copy_from (that.strawberry);
			time.copy_from (that.time);
			breaks.copy_from (that.breaks);
			win_condition.copy_from (that.win_condition);
			return this;
		}

		public Model copy_of(){
			return new Model().copy_from(this);
		}

		#endregion


	}
}