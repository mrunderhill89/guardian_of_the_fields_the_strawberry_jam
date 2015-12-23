using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace GameSettings{
public class Randomness : IEquatable<Randomness>
{
  #region Attributes
	public IntReactiveProperty rx_seed = new IntReactiveProperty();
	public int seed{
		get{ return rx_seed.Value;}
		set{ rx_seed.Value = value;}
	}
	public BoolReactiveProperty rx_randomize = new BoolReactiveProperty();
	public bool randomize{
		get{ return rx_randomize.Value;}
		set{ rx_randomize.Value = value;}
	}

	public Randomness(){
		initialize();
	}

	public Randomness initialize(){
		seed = 0;
		randomize = true;
		return this;
	}

	public Randomness copy_from(Randomness that){
		seed = that.seed;
		randomize = that.randomize;
		return this;
	}
	
	public bool Equals(Randomness that){
		return System.Object.ReferenceEquals(this,that) || 
		(seed == that.seed
		|| (randomize == true && that.randomize == true));
	}
  #endregion
}
}
