using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using GameSettings;
using UnityEngine;
using UniRx;
using Vexe.Runtime.Types;

public class GameSettingsComponent : BetterBehaviour, IDirectoryLoader<Model>, IDirectorySaver<Model>
{
  #region Attributes
	protected static LocalFileLoader<Model> _local = new LocalFileLoader<Model>();
	[Show]
	public static LocalFileLoader<Model> local{
		get{
			return _local;
		}
	}
	public static IDirectoryLoader<Model> loader{
		get{ return local; }
	}
	public static IDirectorySaver<Model> saver{
		get{ return local; }
	}
	
	/*
	* The working rules are static, so they should be initialized no matter whether
	* the component is used or not.
	*/
	[DontSerialize]
	public static ReactiveProperty<Model> rx_working_rules = new ReactiveProperty<Model>();
	
	[Show]
	public static Model working_rules{
		get{
			return rx_working_rules.Value;
		}
		private set {rx_working_rules.Value = value;}
	}

	/*
	* The current rules are per-component. They should never touch the working rules except on startup or apply.
	*/
	[DontSerialize]
	public ReactiveProperty<Model> rx_current_rules = new ReactiveProperty<Model>();

	[Show]
	public Model current_rules{
		get{
			if (rx_current_rules.Value == null && working_rules != null){
				rx_current_rules.Value = working_rules.copy_of();
			}
			return rx_current_rules.Value;
		}
		set {
			rx_current_rules.Value = value;
		}
	}

	#endregion
		public void load_current(){
			current_rules = load();
		}
		public void save_current(){
			save(current_rules);
		}
	#region Inherit from ILoader
		public Model load(){
			return loader.load();
		}
		public ReadOnlyReactiveProperty<Model> rx_load(){
			return loader.rx_load();
		}
	#endregion
	#region Inherit from ISaver
		public void save(Model value){
			saver.save(value);
		}
		public void rx_save(Model value){
			saver.rx_save(value);
		}
	#endregion
	#region Inherit from IFileLoader
		public Model load(string file){
			return load(file);
		}
		public ReadOnlyReactiveProperty<Model> rx_load(string file){
			return loader.rx_load(file);
		}
		public string filename{
			get{
				return loader.filename;
			}
			set{
				loader.filename = value;
				saver.filename = value;
			}
		}
		public ReadOnlyReactiveProperty<string> rx_filename{
			get{ return loader.rx_filename;	}
		}
		public string[] available_files(){
			return loader.available_files();
		}
		public bool is_file_available(string file){
			return loader.is_file_available(file);
		}
	#endregion
	#region Inherit from IFileSaver
		public void save(Model value, string file){
			saver.save(value,file);
		}
		public void rx_save(Model value, string file){
			saver.rx_save(value, file);
		}
	#endregion
	#region Inherit from IDirectoryLoader
		public Model load(string file, string directory){
			return loader.load(file, directory);
		}
		public ReadOnlyReactiveProperty<Model> rx_load(string file, string directory){
			return loader.rx_load(file, directory);
		}
		public string directory{
			get{ return loader.directory; }
			set{
				loader.directory = value;
				saver.directory = value;
			}
		}
		public ReadOnlyReactiveProperty<string> rx_directory{
			get{ return loader.rx_directory; }
		}
	#endregion
	#region Inherit from IDirectorySaver
		public void save(Model value, string file, string directory){
			saver.save(value,file);
		}
		public void rx_save(Model value, string file, string directory){
			saver.rx_save(value, file, directory);
		}
	#endregion
	void Start(){
		local.directory = Application.streamingAssetsPath + "/Data/Settings/";
		working_rules = loader.load("default");
	}
	
	public GameSettingsComponent parent = null;

	public void apply(){
		if (parent != null){
			parent.current_rules.copy_from(current_rules);
		} else {
			working_rules.copy_from(current_rules);
		}
	}
	public void revert(){
		current_rules.copy_from(working_rules);
		if (parent != null){
			current_rules.copy_from(parent.current_rules);
		} else {
			current_rules.copy_from(working_rules);
		}
	}
}

