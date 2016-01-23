// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRx
{
    public static partial class UnityUIComponentExtensions
    {
        public static IDisposable SubscribeToText(this IObservable<string> source, Text text)
        {
            return source.Subscribe(x => text.text = x);
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, Text text)
        {
            return source.Subscribe(x => text.text = x.ToString());
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, Text text, Func<T, string> selector)
        {
            return source.Subscribe(x => text.text = selector(x));
        }

        public static IDisposable SubscribeToInteractable(this IObservable<bool> source, Selectable selectable)
        {
            return source.Subscribe(x => selectable.interactable = x);
        }

        /// <summary>Observe onClick event.</summary>
        public static IObservable<Unit> OnClickAsObservable(this Button button)
        {
            return button.onClick.AsObservable();
        }

        /// <summary>Observe onValueChanged with current `isOn` value on subscribe.</summary>
        public static IObservable<bool> OnValueChangedAsObservable(this Toggle toggle)
        {
            // Optimized Defer + StartWith
            return Observable.Create<bool>(observer =>
            {
                observer.OnNext(toggle.isOn);
                return toggle.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static IObservable<float> OnValueChangedAsObservable(this Scrollbar scrollbar)
        {
            return Observable.Create<float>(observer =>
            {
                observer.OnNext(scrollbar.value);
                return scrollbar.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `normalizedPosition` value on subscribe.</summary>
        public static IObservable<Vector2> OnValueChangedAsObservable(this ScrollRect scrollRect)
        {
            return Observable.Create<Vector2>(observer =>
            {
                observer.OnNext(scrollRect.normalizedPosition);
                return scrollRect.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static IObservable<float> OnValueChangedAsObservable(this Slider slider)
        {
            return Observable.Create<float>(observer =>
            {
                observer.OnNext(slider.value);
                return slider.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onEndEdit(Submit) event.</summary>
        public static IObservable<string> OnEndEditAsObservable(this InputField inputField)
        {
            return inputField.onEndEdit.AsObservable();
        }

        /// <summary>Observe onValueChange with current `text` value on subscribe.</summary>
        public static IObservable<string> OnValueChangeAsObservable(this InputField inputField)
        {
            return Observable.Create<string>(observer =>
            {
                observer.OnNext(inputField.text);
                return inputField.onValueChanged.AsObservable().Subscribe(observer);
            });
        }
		
		/// <summary>Observe onValueChange with current `text` value on subscribe.</summary>
        public static IObservable<int> OnValueChangedAsObservable(this Dropdown dropdown)
        {
            return Observable.Create<int>(observer =>
            {
                observer.OnNext(dropdown.value);
                return dropdown.onValueChanged.AsObservable().Subscribe(observer);
            });
        }
		
		/// <summary>Hooks up a Dropdown to select a value from a reactive collection. </summary>
		public static IObservable<T> SelectFromCollection<T>(
			this Dropdown dropdown, 
			ReactiveCollection<T> collection
		){
			return dropdown.SelectFromCollection<T>(
				collection,
				(T value, int i)=>value.ToString()
			);
		}
		public static IObservable<T> SelectFromCollection<T>(
			this Dropdown dropdown, 
			ReactiveCollection<T> collection,
			System.Func<T, int, string> get_text
		){
			return dropdown.SelectFromCollection<T>(
				collection,
				(T value, int index)=>{
					var option = new Dropdown.OptionData();
					option.text = get_text(value, index);
					return option;
				}
			);
		}
		public static IObservable<T> SelectFromCollection<T>(
			this Dropdown dropdown, 
			ReactiveCollection<T> collection,
			System.Func<T, int, IObservable<string>> get_text
		){
			return dropdown.SelectFromCollection<T>(
				collection,
				(T value, int index)=>{
					var option = new Dropdown.OptionData();
					get_text(value, index).Subscribe(text=>{
						if (dropdown.value == index)
							dropdown.captionText.text = text;
						option.text = text;
					});
					return option;
				}
			);
		}
		public static IObservable<T> SelectFromCollection<T>(
			this Dropdown dropdown, 
			ReactiveCollection<T> collection, 
			System.Func<T, int, Dropdown.OptionData> get_option
		){
			dropdown.options.Clear();
			for (int i  = 0; i < collection.Count; i++){
				dropdown.options.Insert(i, get_option(collection[i],i));
			}
			collection.ObserveAdd().Subscribe(evn=>{
				var option = get_option(evn.Value,evn.Index);
				if (dropdown.options.Count == 0)
					dropdown.captionText.text = option.text;
				dropdown.options.Insert(evn.Index, option);
			});
			collection.ObserveRemove().Subscribe(evn=>{
				dropdown.options.RemoveAt(evn.Index);
			});
			return collection.ObserveContents()
			.CombineLatest(
				dropdown.OnValueChangedAsObservable().Where(index=>index < collection.Count),
				(evn, index)=>{
					if (index >= evn.Contents.Length)
						return default(T);
					return evn.Contents[index];
				}
			);
		}
    }
}

#endif