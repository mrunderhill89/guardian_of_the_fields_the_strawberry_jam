using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class SettingsFileBar : BetterBehaviour {
	public InputField file_input;
	public Button load;
	public Button save;
	public ObjectVisibility appear_when_different;
	public Button apply;
	public Button revert;
	public GameSettingsComponent data_component;
	
	public Dropdown quick_import;
	class QuickImportOption{
		internal FileInfo file;
		internal string filename;
		internal Dropdown.OptionData dropdown_option;
		internal ReactiveProperty<string> text;
		public QuickImportOption(FileInfo _file){
			file = _file;
			dropdown_option = new Dropdown.OptionData();
			filename = Path.GetFileNameWithoutExtension(file.Name);
			text = LanguageTable.dictionary.current_language.Select(lang=>{
				if (LanguageTable.has("filename_"+filename))
					return LanguageTable.get("filename_"+filename);
				return textInfo.ToTitleCase(filename);
			}).ToReactiveProperty();
			text.Subscribe(t=>{
				dropdown_option.text = t;
			});
		}
		static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
	}

	ReactiveCollection<QuickImportOption> quick_import_options = new ReactiveCollection<QuickImportOption>();
	IntReactiveProperty default_option = new IntReactiveProperty(0);
	void Start () {
		quick_import.options.Clear();
		DirectoryInfo d = new DirectoryInfo(GameSettings.Model.default_folder);
		//Automatically set the dropdown to the default file if one is present.
		default_option.CombineLatest(
			quick_import_options.ObserveCountChanged(),
			(index, count)=>{
				return Math.Min(index,count);
			}
		).Subscribe(value=>{
			quick_import.value = value;
			if (value < quick_import_options.Count)
				quick_import_options[value].text.Subscribe(text=>{
					if (quick_import.value == value)
						quick_import.captionText.text = text;
				});
		});

		quick_import
		.SelectFromCollection(quick_import_options, (opt, index)=>{
			if (default_option.Value == 0 
				&& string.Equals(opt.filename, "default", StringComparison.CurrentCultureIgnoreCase)){
				default_option.Value = index;
			}
			return opt.dropdown_option;
		}).Subscribe(option=>{
			data_component.import(option.file.FullName);
			file_input.text = option.file.FullName;
		});
		
		quick_import_options.SetRange(d.GetFiles("*.yaml").Select(file=>new QuickImportOption(file)));
		
		load.onClick.AddListener(()=>{
			data_component.import(file_input.text);
		});
		save.onClick.AddListener(()=>{
			data_component.export(file_input.text);
		});
		apply.onClick.AddListener(()=>{
			data_component.apply();
		});
		revert.onClick.AddListener(()=>{
			data_component.revert();
		});
		data_component.rx_is_working_rules.Subscribe((value)=>{
			appear_when_different.visible = !value;
		});
		file_input.text = GameSettings.Model.default_filename;
	}
}
