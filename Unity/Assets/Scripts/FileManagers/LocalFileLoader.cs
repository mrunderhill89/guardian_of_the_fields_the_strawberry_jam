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
	public ReadOnlyReactiveProperty<T> rx_load(){
		return rx_load(filename, directory);
	}

	//ISaver
	public void save(T value){
		save(value, filename, directory);
	}
	protected Subject<T> _rx_save = new Subject<T>();
	public IObservable<T> rx_save{
		get{ return _rx_save; }
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
			return deserializer.Deserialize<T>(input);
		} catch(Exception e){
			Debug.LogError("LocalFileLoader: Problem attempting to load file:'"+filesystem_name+"':"+e.ToString());
			return default(T);
		}
	}

	public ReadOnlyReactiveProperty<T> rx_load(string _file, string _directory){
		return Observable.Return<T>(load(_file, _directory)).ToReadOnlyReactiveProperty<T>();
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

	public LocalFileLoader(string _directory = "", string _file = ""){
		filename = _file;
		directory = _directory;
		to_filesystem = file => directory+"/"+file+".yaml";
		from_filesystem = fn => Path.GetFileNameWithoutExtension(fn);
	}
}

/*
	public LocalFileLoader(Func<string,string> n_fm, string dir = ""):this(dir){
		name_to_filename = n_fm;
	}
	
	public LocalFileLoader(string dir = ""){
		rx_directory = new StringReactiveProperty(dir);
		name_to_filename = (name)=> directory+"/"+name+".yaml";
	}
	
	public string[] available_files(){
		DirectoryInfo d = new DirectoryInfo(directory);
		return d.GetFiles("*.yaml").Select(file=>Path.GetFileNameWithoutExtension(file.Name)).ToArray();
	}
	
	public bool is_file_available(string name){
		return File.Exists(name_to_filename(name));
	}
	
	protected Func<string,string> name_to_filename;
	
	//Load
	public T load(){
		return load(filename);
	}
	
	public T load(string file){
		return load(file,directory);
	}
	
	public T load(string file, string directory){
		string filename = name_to_filename(file);
		try{
			string Document = File.ReadAllLines(filename).Aggregate("", (string b, string n)=>{
				if (b == "") return n; //Ignore empty lines
				return b+"\n"+n;
			});
			var input = new StringReader(Document);
			var deserializer = new Deserializer(namingConvention: new UnderscoredNamingConvention());
			return deserializer.Deserialize<T>(input);
		} catch(Exception e){
			Debug.LogError("LocalFileLoader: Problem attempting to load file:'"+filename+"':"+e.ToString());
			return default(T);
		}
	}
	
	// Rx Load
	public IObservable rx_load(){
		return rx_load(filename);
	}
	public ReactiveProperty<T> rx_load(){
		return rx_name.Select<string,T>(this.load).ToReactiveProperty<T>();
	}
	
	public void save(string name, T value){
		string filename = name_to_filename(name);
		try {
			StreamWriter fout = new StreamWriter(filename);
			var serializer = new Serializer();
			serializer.Serialize(fout, value);
			fout.Close();
		} catch (Exception e){
			Debug.LogError("LocalFileLoader: Problem attempting to save to file:'"+filename+"':"+e.ToString());
		}
	}

*/