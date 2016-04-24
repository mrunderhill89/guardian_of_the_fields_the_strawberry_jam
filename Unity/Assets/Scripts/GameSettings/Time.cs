using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace GameSettings{
public class Time : IEquatable<GameSettings.Time>
{

  #region Attributes
		public BoolReactiveProperty rx_infinite_length = new BoolReactiveProperty();
		public bool infinite_length{
			get{ return rx_infinite_length.Value;}
			set{ rx_infinite_length.Value = value;}
		}

		public BoolReactiveProperty rx_end_early = new BoolReactiveProperty();
		public bool end_early{
			get{ return rx_end_early.Value;}
			set{ rx_end_early.Value = value;}
		}

		public FloatReactiveProperty rx_game_length = new FloatReactiveProperty();
		public float game_length{
			get{ return rx_game_length.Value;}
			set{ rx_game_length.Value = value;}
		}

		public FloatReactiveProperty rx_start_hour = new FloatReactiveProperty();
		public float start_hour{
			get{ return rx_start_hour.Value;}
			set{ rx_start_hour.Value = value;}
		}

		public FloatReactiveProperty rx_end_hour = new FloatReactiveProperty();
		public float end_hour{
			get{ return rx_end_hour.Value;}
			set{ rx_end_hour.Value = value;}
		}

		public FloatReactiveProperty rx_movement_speed = new FloatReactiveProperty();
		public float movement_speed{
			get{ return rx_movement_speed.Value; }
			set{ rx_movement_speed.Value = value;}
		}

		public FloatReactiveProperty rx_slide_speed = new FloatReactiveProperty();
		public float slide_speed{
			get{ return rx_slide_speed.Value; }
			set{ rx_slide_speed.Value = value;}
		}

		#endregion
		public Time(){
			rx_infinite_length.Subscribe ((value) => {
				if (value){
					end_early = true;
				}
			});
			initialize ();
		}

		public Time initialize(){
			infinite_length = false;
			end_early = true;
			game_length = 2400.0f;
			start_hour = 6.0f;
			end_hour = 18.0f;
			movement_speed = 1.5f;
			slide_speed = 0.1f;
			return this;
		}

		public Time copy_from(Time that){
			infinite_length = that.infinite_length;
			end_early = that.end_early;
			game_length = that.game_length;
			start_hour = that.start_hour;
			end_hour = that.end_hour;
			return this;
		}
		public bool Equals(GameSettings.Time that){
			return System.Object.ReferenceEquals(this,that) || 
			(infinite_length == that.infinite_length
			&& end_early == that.end_early
			&& game_length == that.game_length
			&& start_hour == that.start_hour 
			&& end_hour == that.end_hour);
		}
	}
}
