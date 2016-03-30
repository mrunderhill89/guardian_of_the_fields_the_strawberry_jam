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

public class SaveLoadSelector{
	protected static LocalFileLoader<GameSettings.Model> _local_settings;
	protected static LocalFileLoader<GameSettings.Model> local_settings {
		get{ 
			if (_local_settings == null){
				local_settings = new LocalFileLoader<GameSettings.Model>(Application.streamingAssetsPath + "/Data/Settings/");
			}
			return _local_settings; 
		}
		private set{ _local_settings = value; }
	}
	public static IMultiLoader<GameSettings.Model> get_settings_loader(){
		#if UNITY_WEBGL
			Debug.LogWarning("Web File Loader Not Implemented Yet.");
		#endif
		return local_settings;
	}
	public static ISaver<GameSettings.Model> get_settings_saver(){
		#if UNITY_WEBGL
			Debug.LogWarning("Web File Saver Not Implemented Yet.");
		#endif
		return local_settings;
	}

}