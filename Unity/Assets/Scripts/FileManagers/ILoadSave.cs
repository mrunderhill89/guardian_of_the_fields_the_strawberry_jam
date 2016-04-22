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

public interface ILoader<T>{
	T load();
	ReadOnlyReactiveProperty<T> rx_load();
}

public interface ISaver<T>{
	void save(T value);
	IObservable<T> rx_save{get;}
}