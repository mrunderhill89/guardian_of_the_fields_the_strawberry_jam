using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace GameSettings{
public class StrawberryGeneration : IEquatable<StrawberryGeneration>
{

  #region Attributes

		public IntReactiveProperty rx_max_berries_in_field = new IntReactiveProperty();
		public FloatReactiveProperty rx_min_size = new FloatReactiveProperty();
		public FloatReactiveProperty rx_max_size = new FloatReactiveProperty();
		public FloatReactiveProperty rx_min_ripeness = new FloatReactiveProperty();
		public FloatReactiveProperty rx_max_ripeness = new FloatReactiveProperty();
		public FloatReactiveProperty rx_density = new FloatReactiveProperty();

		public int max_berries_in_field{
			get{ return rx_max_berries_in_field.Value;}
			set{ rx_max_berries_in_field.Value = value; }
		}
		public float min_size{
			get{ return rx_min_size.Value;}
			set{ rx_min_size.Value = value; }
		}
		public float max_size{
			get{ return rx_max_size.Value;}
			set{ rx_max_size.Value = value; }
		}
		public float min_ripeness{
			get{ return rx_min_ripeness.Value;}
			set{ rx_min_ripeness.Value = value; }
		}
		public float max_ripeness{
			get{ return rx_max_ripeness.Value;}
			set{ rx_max_ripeness.Value = value; }
		}
		public float density{
			get{ return rx_density.Value;}
			set{ rx_density.Value = value; }
		}

		public ReadOnlyReactiveProperty<float[]> rx_ripeness_range;
		
		#endregion
		public StrawberryGeneration(){
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
			initialize ();
		}
		public StrawberryGeneration initialize(){
			max_berries_in_field = 40;
			min_size = 0.06f;
			max_size = 0.06f;
			min_ripeness = 0.0f;
			max_ripeness = 2.0f;
			density = 2400.0f;
			return this;
		}
		public StrawberryGeneration copy_from(StrawberryGeneration that){
			max_berries_in_field = that.max_berries_in_field;
			min_size = that.min_size;
			max_size = that.max_size;
			min_ripeness = that.min_ripeness;
			max_ripeness = that.max_ripeness;
			density = that.density;
			return this;
		}
		
		public bool Equals(StrawberryGeneration that){
			return System.Object.ReferenceEquals(this,that) || 
			(max_berries_in_field == that.max_berries_in_field
			&& min_size == that.min_size
			&& max_size == that.max_size
			&& min_ripeness == that.min_ripeness
			&& max_ripeness == that.max_ripeness
			&& density == that.density);
		}

}
}