using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;

namespace GameScores
{
	public class TimeScore
	{
		public DateTime date_recorded { get; set; }
		[Show]
		public DateTime date_recorded_local(){
			return date_recorded.ToLocalTime();
		}
		public float played_for {get; set;}
		public float distance_covered {get; set;}

		public TimeScore(){
			date_recorded = DateTime.UtcNow;
			played_for = 0.0f;
			distance_covered = 0.0f;
		}

		public TimeScore copy_from(TimeScore that){
			date_recorded = that.date_recorded;
			played_for = that.played_for;
			distance_covered = that.distance_covered;
			return this;
		}
		public TimeScore copy_of(){
			return new TimeScore().copy_from(this);
		}

	}
}

