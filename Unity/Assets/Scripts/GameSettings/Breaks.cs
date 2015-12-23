using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace GameSettings{
public class Breaks : IEquatable<Breaks>
{

  #region Attributes

		public IntReactiveProperty rx_length = new IntReactiveProperty();
		public int length{
			get{ return rx_length.Value;}
			set{rx_length.Value = value;}
		}
		public IntReactiveProperty rx_distance = new IntReactiveProperty();
		public int distance{
			get{ return rx_distance.Value;}
			set{rx_distance.Value = value;}
		}

  #endregion
		public Breaks(){
			initialize ();
		}

		public Breaks initialize(){
			length = 5;
			distance = 10;
			return this;
		}

		public Breaks copy_from(Breaks that){
			length = that.length;
			distance = that.distance;
			return this;
		}
		
		public bool Equals(Breaks that){
			return System.Object.ReferenceEquals(this,that) || 
			(length == that.length
			&& distance == that.distance);
		}
}
}
