﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class DirectoryTextField : BetterBehaviour {
	public InputField input;
	public IDirectoryLoader loader;

	void Start () {
		if (loader != null){
			loader.rx_directory.Subscribe(text=>{
				if (input != null && input.text != null)
					input.text = text;
			});
			input.text = loader.directory;
		}
	}
}
