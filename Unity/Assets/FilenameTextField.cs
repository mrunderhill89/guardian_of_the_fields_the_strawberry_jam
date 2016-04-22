using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class FilenameTextField : BetterBehaviour {
	public InputField input;
	public IFileLoader loader;

	// Use this for initialization

	void Start () {
		if (loader != null){
			loader.rx_filename.Subscribe(text=>{
				input.text = text;
			});
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
