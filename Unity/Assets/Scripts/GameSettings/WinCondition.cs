using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
namespace GameSettings{
public class WinCondition
{

  #region Attributes

		public FloatReactiveProperty rx_min_ripeness = new FloatReactiveProperty();
		public float min_ripeness{
			get{ return rx_min_ripeness.Value;}
			set{ rx_min_ripeness.Value = value;}
		}
		public FloatReactiveProperty rx_max_ripeness = new FloatReactiveProperty();
		public float max_ripeness{
			get{ return rx_max_ripeness.Value;}
			set{ rx_max_ripeness.Value = value;}
		}
		public FloatReactiveProperty rx_min_size = new FloatReactiveProperty();
		public float min_size{
			get{ return rx_min_size.Value;}
			set{ rx_min_size.Value = value;}
		}
		public FloatReactiveProperty rx_min_basket_weight = new FloatReactiveProperty();
		public float min_basket_weight{
			get{ return rx_min_basket_weight.Value;}
			set{ rx_min_basket_weight.Value = value;}
		}
		public FloatReactiveProperty rx_max_basket_weight = new FloatReactiveProperty();
		public float max_basket_weight{
			get{ return rx_max_basket_weight.Value;}
			set{ rx_max_basket_weight.Value = value;}
		}
		public Dictionary<StrawberryComponent.BerryPenalty, FloatReactiveProperty> rx_penalties 
			= new Dictionary<StrawberryComponent.BerryPenalty, FloatReactiveProperty> ();
		public FloatReactiveProperty rx_penalty(StrawberryComponent.BerryPenalty penalty){
			if (!rx_penalties.ContainsKey(penalty)){
				rx_penalties[penalty] = new FloatReactiveProperty(0.0f);
			}
			return rx_penalties[penalty];
		}
		public float penalty(StrawberryComponent.BerryPenalty penalty){
			return rx_penalty (penalty).Value;
		}
		public WinCondition penalty(StrawberryComponent.BerryPenalty penalty, float value){
			rx_penalty(penalty).Value = value;
			return this;
		}
		public Dictionary<StrawberryComponent.BerryPenalty, float> penalties{
			get{
				Dictionary<StrawberryComponent.BerryPenalty, float> output = new Dictionary<StrawberryComponent.BerryPenalty, float>();
				foreach(KeyValuePair<StrawberryComponent.BerryPenalty, FloatReactiveProperty> kvp in rx_penalties){
					output[kvp.Key] = kvp.Value.Value;
				}
				return output;
			}
			set{
				//Clear existing penalties first.
				foreach(FloatReactiveProperty prop in rx_penalties.Values){
					prop.Value = 0.0f;
				}
				foreach(KeyValuePair<StrawberryComponent.BerryPenalty, float> kvp in value){
					penalty(kvp.Key,kvp.Value);
				}
			}
		}

		public ReadOnlyReactiveProperty<float[]> rx_ripeness_range;

		#endregion
		public WinCondition(){
			rx_min_ripeness.Subscribe ((float value) => {
				if (value > max_ripeness)
					max_ripeness = value;
			});
			rx_max_ripeness.Subscribe ((float value) => {
				if (value < min_ripeness)
					min_ripeness = value;
			});
			rx_ripeness_range = rx_min_ripeness.DistinctUntilChanged().CombineLatest (
				rx_max_ripeness.DistinctUntilChanged(),
				(float min, float max) => {
				return new float[]{min,max};
			}
			).ToReadOnlyReactiveProperty<float[]>();
			initialize();
		}

		public WinCondition initialize(){
			min_ripeness = 0.5f;
			max_ripeness = 1.5f;
			min_size = 0.0f;
			min_basket_weight = 15.0f;
			max_basket_weight = 17.0f;
			return this;
		}
		public WinCondition copy_from(WinCondition that){
			min_ripeness = that.min_ripeness;
			max_ripeness = that.max_ripeness;
			min_size = that.min_size;
			min_basket_weight = that.min_basket_weight;
			max_basket_weight = that.max_basket_weight;
			penalties = that.penalties;
			return this;
		}
		
	}
}
