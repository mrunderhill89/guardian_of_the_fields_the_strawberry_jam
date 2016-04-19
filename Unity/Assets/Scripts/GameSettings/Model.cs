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
	public class Model : IEquatable<Model>
	{
		#region Properties
		public Randomness randomness { get; set;}
		public Flags flags {get; set;}
		public StrawberryGeneration strawberry {get; set;}
		public Breaks breaks {get; set;}
		public WinCondition win_condition {get; set;}
		public GameSettings.Time time {get; set;}
		public static string default_folder {
			get{ return Application.streamingAssetsPath + "/Data/Settings/";}
		}
		public static string default_filename {
			get{ return "default";}
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
			initialize ();
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

		public bool Equals(Model that){
			return System.Object.ReferenceEquals(this,that) || 
			(flags.Equals(that.flags)
			&& randomness.Equals(that.randomness)
			&& strawberry.Equals(that.strawberry)
			&& time.Equals(that.time)
			&& breaks.Equals(that.breaks)
			&& win_condition.Equals(that.win_condition));
		}
		#endregion


	}
}
