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

public interface IFileLoader{
	string filename{get;set;}
	ReadOnlyReactiveProperty<string> rx_filename{get;}
}

public interface IFileLoader<T>: ILoader<T>, IFileLoader{
	T load(string file);
	ReadOnlyReactiveProperty<T> rx_load(string file);
	string[] available_files();
	bool is_file_available(string file);
}

public interface IFileSaver<T>: ISaver<T>, IFileLoader{
	void save(T value, string file);
}