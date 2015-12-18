using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace GameSettings{
public class Randomness
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
			rx_randomize.ObserveOnMainThread().Subscribe((value) => {
				if (value){
					seed = unity_seed;
				}
			});
			initialize ();
		}

		public static int unity_seed{
			get{
				try{
					return UnityEngine.Random.seed;
				} catch (ArgumentException){
					return 0;
				}
			}
		}

	public Randomness initialize(){
		seed = unity_seed;
		randomize = true;
		return this;
	}

	public Randomness copy_from(Randomness that){
		if (!randomize){
			seed = that.seed;
		}
		randomize = that.randomize;
		return this;
	}


  #endregion


}
}
