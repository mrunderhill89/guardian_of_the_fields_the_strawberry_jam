using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Vexe.Runtime.Types;

public class RxUIAdapter: IDisposable {
	public List<IDisposable> subscriptions = new List<IDisposable>();
	public RxUIAdapter register_toggle(Toggle toggle, IObservable<bool> rx_get, Action<bool> rx_set){
		subscriptions.Add(
			rx_get
				.DistinctUntilChanged()
				.Subscribe ((value) => {
					toggle.isOn = value;
		}));
		subscriptions.Add(toggle.OnValueChangedAsObservable().Subscribe(rx_set));
		return this;
	}

	public RxUIAdapter register_slider(Slider slider, IObservable<float> rx_get, Action<float> rx_set){
		subscriptions.Add(
			rx_get
				.DistinctUntilChanged()
				.Subscribe ((value) => {
					slider.value = value;
		}));
		slider.OnValueChangedAsObservable().Subscribe(rx_set);
		return this;
	}

	public RxUIAdapter register_input_float(InputField field, IObservable<float> rx_get, Action<float> rx_set){
		subscriptions.Add(
			rx_get
				.DistinctUntilChanged()
				.Subscribe ((value) => {
					field.text = value.ToString();
		}));
		subscriptions.Add(
			field.OnValueChangeAsObservable()
				.Where(text=>{
					float sink; //No pun intended.
					return float.TryParse(text, out sink);
				}).Select(text=>float.Parse(text))
				.Subscribe(rx_set));
		return this;
	}
	
	public RxUIAdapter register_input_int(InputField field, IObservable<int> rx_get, Action<int> rx_set){
		subscriptions.Add(
			rx_get
				.DistinctUntilChanged()
				.Subscribe ((value) => {
					field.text = value.ToString();
			}));
		subscriptions.Add(
			field.OnValueChangeAsObservable()
				.Where(text=>{
					int sink;
					return int.TryParse(text, out sink);
				})
				.Select(text=>int.Parse(text))
				.Subscribe(rx_set));
		return this;
	}
	
	public void Dispose(){
		foreach(IDisposable sub in subscriptions){
			sub.Dispose();
		}
		subscriptions.Clear();
	}
}