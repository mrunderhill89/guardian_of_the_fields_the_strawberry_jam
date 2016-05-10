using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.RepresentationModel;
using UniRx;

public class LocalFileLoader<T>: IDirectoryLoader<T>, IDirectorySaver<T> {
	//ILoader
	public T load(){
		return load(filename, directory);
	}
	protected ReactiveProperty<T> rx_last_loaded = new ReactiveProperty<T>();
	public ReadOnlyReactiveProperty<T> rx_load(){
		return rx_last_loaded.ToReadOnlyReactiveProperty<T>();
	}

	//ISaver
	public void save(T value){
		save(value, filename, directory);
	}
	
	public void rx_save(T value){
		rx_scheduled_save.Value = value;
	}
	
	protected Subject<T> _rx_on_save = new Subject<T>();
	public IObservable<T> rx_on_save{
		get{ return _rx_on_save; }
	}
	
	//IFileLoader
	protected StringReactiveProperty _rx_filename = new StringReactiveProperty();
	public ReadOnlyReactiveProperty<string> rx_filename{
		get{ return _rx_filename.ToReadOnlyReactiveProperty<string>(); }
	}
	[Show]
	public string filename{
		get{ return _rx_filename.Value; }
		set{ _rx_filename.Value = value; }
	}
	
	public T load(string _file){
		return load(_file,directory);
	}
	
	public ReadOnlyReactiveProperty<T> rx_load(string _file){
		return rx_load(_file, directory);
	}
	
	public string[] available_files(){
		DirectoryInfo d = new DirectoryInfo(directory);
		return d.GetFiles("*.yaml").Select(file=>from_filesystem(file.Name)).ToArray();
	}
	
	public bool is_file_available(string file){
		return File.Exists(to_filesystem(file));
	}
	
	//IFileSaver
	public void save(T value, string file){
		save(value, file, directory);
	}
	
	protected ReactiveProperty<T> rx_scheduled_save = new ReactiveProperty<T>();
	public void rx_save(T value, string _file){
		filename = _file;
		rx_scheduled_save.Value = value;
	}
	
	//IDirectoryLoader
	protected StringReactiveProperty _rx_directory = new StringReactiveProperty();
	public ReadOnlyReactiveProperty<string> rx_directory{
		get{ return _rx_directory.ToReadOnlyReactiveProperty<string>(); }
	}
	[Show]
	public string directory{
		get{ return _rx_directory.Value; }
		set{ _rx_directory.Value = value; }
	}
	
	public T load(string _file, string _directory){
		filename = _file;
		directory = _directory;
		try{
			string Document = File.ReadAllLines(filesystem_name).Aggregate("", (string b, string n)=>{
				if (b == "") return n; //Ignore empty lines
				return b+"\n"+n;
			});
			var input = new StringReader(Document);
			var deserializer = new Deserializer(namingConvention: new UnderscoredNamingConvention());
			var output = deserializer.Deserialize<T>(input);
			rx_last_loaded.Value = output;
			return output;
		} catch(Exception e){
			Debug.LogError("LocalFileLoader: Problem attempting to load file:'"+filesystem_name+"':"+e.ToString());
			return default(T);
		}
	}

	public ReadOnlyReactiveProperty<T> rx_load(string _file, string _directory){
		return Observable.Return<T>(load(_file, _directory)).ToReadOnlyReactiveProperty<T>();;
	}

	//IDirectorySaver
	public void save(T value, string _file, string _directory){
		filename = _file;
		directory = _directory;
		try {
			StreamWriter fout = new StreamWriter(filesystem_name);
			var serializer = new Serializer();
			serializer.Serialize(fout, value);
			fout.Close();
		} catch (Exception e){
			Debug.LogError("LocalFileLoader: Problem attempting to save to file:'"+filesystem_name+"':"+e.ToString());
		}
	}

	public void rx_save(T value, string _file, string _directory){
		filename = _file;
		directory = _directory;
		rx_scheduled_save.Value = value;
	}

	
	//Internal
	public Func<string,string> to_filesystem;
	public Func<string,string> from_filesystem;
	public string filesystem_name{
		get{
			if (to_filesystem == null)
				return filename;
			return to_filesystem(filename);
		}
	}
	protected ReadOnlyReactiveProperty<UniRx.Tuple<string,string>> rx_file_location;

	public LocalFileLoader(string _directory = "", string _file = ""){
		filename = _file;
		directory = _directory;
		to_filesystem = file => directory+"/"+file+".yaml";
		from_filesystem = fn => Path.GetFileNameWithoutExtension(fn);
		
		rx_file_location = rx_directory.Where(dir=>dir!="").CombineLatest(
			rx_filename.Where(file=>file!=""), 
			(dir,file)=>{
				return UniRx.Tuple.Create(dir, file);
			}
		).ToReadOnlyReactiveProperty<UniRx.Tuple<string,string>>();
		
		rx_scheduled_save.WithLatestFrom(rx_file_location, (T value, UniRx.Tuple<string,string> location)=>{
			return new {directory = location.Item1, filename = location.Item2, value = value};
		}).Subscribe(data=>{
			save(data.value, data.filename, data.directory);
		});
	}
}