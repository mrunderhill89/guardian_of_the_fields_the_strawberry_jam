using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace GameSettings{
public class Flags
{

  #region Attributes
		public BoolReactiveProperty rx_tutorial = new BoolReactiveProperty();
		public bool tutorial{
			get{ return rx_tutorial.Value;}
			set{ rx_tutorial.Value = value;}
		}
		public BoolReactiveProperty rx_cheats = new BoolReactiveProperty();
		public bool cheats{
			get{ return rx_cheats.Value;}
			set{ rx_cheats.Value = value;}
		}

		#endregion
		public Flags initialize(){
			tutorial = true;
			cheats = false;
			return this;
		}

		public Flags copy_from(Flags that){
			tutorial = that.tutorial;
			cheats = that.cheats;
			return this;
		}
	}

}