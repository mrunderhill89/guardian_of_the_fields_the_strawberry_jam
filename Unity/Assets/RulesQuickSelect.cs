using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class RulesQuickSelect : BetterBehaviour {
	public GameSettingsComponent data_component;
	public Dropdown quick_import;
	protected IFileLoader<GameSettings.Model> loader;

	class QuickImportOption{
		internal string option_name;
		internal ReadOnlyReactiveProperty<string> rx_display_text;
		internal Dropdown.OptionData dropdown_option;
		public QuickImportOption(string _name){
			option_name = _name;
			dropdown_option = new Dropdown.OptionData();
			
			rx_display_text = LanguageController.controller.rx_get_filename_label(_name);
			
			rx_display_text.Subscribe(t=>{
				dropdown_option.text = t;
			});
		}
	}
	
	ReactiveCollection<QuickImportOption> quick_import_options = new ReactiveCollection<QuickImportOption>();
	IntReactiveProperty default_option = new IntReactiveProperty(0);

	void Start () {
		loader = GameSettingsComponent.loader;
		quick_import.options.Clear();
		
		//Automatically set the dropdown to the default file if one is present.
		default_option.CombineLatest(
			quick_import_options.ObserveCountChanged(),
			(index, count)=>{
				return Math.Min(index,count);
			}
		).Subscribe(value=>{
			quick_import.value = value;
			if (value < quick_import_options.Count)
				quick_import_options[value].rx_display_text.Subscribe(text=>{
					if (quick_import != null && quick_import.value == value)
						quick_import.captionText.text = text;
				});
		});
		
		//Keep the box synced with the loader when possible.
		loader.rx_filename.Subscribe((file)=>{
			foreach (QuickImportOption opt in quick_import_options){
				if (quick_import != null && opt.option_name == file){
					quick_import.captionText.text = opt.rx_display_text.Value;
				}
			}
		});
		
		quick_import
		.SelectFromCollection(quick_import_options, (opt, index)=>{
			if (default_option.Value == 0 
				&& string.Equals(opt.option_name, "default", StringComparison.CurrentCultureIgnoreCase)){
				//If this option calls itself "default," set our default index to it.
				default_option.Value = index;
			}
			return opt.dropdown_option;
		}).Subscribe(option=>{
			data_component.current_rules = loader.load(option.option_name);
		});
		
		quick_import_options.SetRange(loader.available_files().Select(name=>new QuickImportOption(name)));
	}
}
