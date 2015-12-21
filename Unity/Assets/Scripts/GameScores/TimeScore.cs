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
		public DateTime date_recorded_local{
			get{
				return date_recorded.ToLocalTime();
			}
		}
		public float played_for {get; set;}
		public TimeScore(){
			date_recorded = DateTime.UtcNow;
			played_for = 0.0f;
		}
	}
}

