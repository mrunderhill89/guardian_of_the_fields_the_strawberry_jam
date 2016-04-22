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

public interface IDirectoryLoader{
	string directory{get;set;}
	ReadOnlyReactiveProperty<string> rx_directory{get;}
}

public interface IDirectoryLoader<T>: IFileLoader<T>, IDirectoryLoader{
	T load(string file, string directory);
	ReadOnlyReactiveProperty<T> rx_load(string file, string directory);
}

public interface IDirectorySaver<T>: IFileSaver<T>, IDirectoryLoader{
	void save(T value, string file, string directory);
	void rx_save(T value, string file, string directory);
}