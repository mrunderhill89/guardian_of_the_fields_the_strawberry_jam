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

public class LocalFileLoader<T>: IMultiLoader<T>, ISaver<T> {
	protected StringReactiveProperty _rx_directory;
	public StringReactiveProperty rx_directory{
		get{ return _rx_directory;}
		private set{ _rx_directory = value;}
	}
	public String directory{
		get{ return rx_directory.Value; }
		set{ rx_directory.Value = value;}
	}

	public LocalFileLoader(Func<string,string> n_fm, string dir = ""):this(dir){
		name_to_filename = n_fm;
	}
	
	public LocalFileLoader(string dir = ""){
		rx_directory = new StringReactiveProperty(dir);
		name_to_filename = (name)=> directory+"/"+name+".yaml";
	}
	
	public string[] get_options(){
		DirectoryInfo d = new DirectoryInfo(directory);
		return d.GetFiles("*.yaml").Select(file=>file.Name).ToArray();
	}
	
	public bool has_option(string name){
		return File.Exists(name_to_filename(name));
	}
	
	protected Func<string,string> name_to_filename;
	
	public T load(string name){
		string filename = name_to_filename(name);
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
	
	public ReactiveProperty<T> rx_load(StringReactiveProperty rx_name){
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
}

/*

		public Model import(string filename = ""){
			Model that = import_static(filename);
			if (that == null) return this;
			copy_from(that);
			return this;
		}
		
		public static Model import_static(string filename = "")
		{
			if (filename == "")
				filename = default_filename;
			string Document = File.ReadAllLines(filename).Aggregate("", (string b, string n)=>{
				if (b == "") return n;
				return b+"\n"+n;
			});
			var input = new StringReader(Document);
			var deserializer = new Deserializer(namingConvention: new UnderscoredNamingConvention());
			Model data = deserializer.Deserialize<Model>(input);
			return data;
		}

		public Model export(string filename = "")
		{
			if (filename == "")
				filename = default_filename;
			StreamWriter fout = new StreamWriter(filename);
			var serializer = new Serializer();
			serializer.Serialize(fout, this);
			fout.Close();
			return this;
		}
*/