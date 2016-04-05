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

public interface IMultiLoader<T>: ILoader<T>{
	string[] get_options();
	bool has_option(string opt);
	string directory{get; set;}
	StringReactiveProperty rx_directory {get;}
}