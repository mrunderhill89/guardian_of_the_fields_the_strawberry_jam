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
	public InputField directory;
	public InputField filename;
	public IDirectoryLoader directory_loader;
	public IFileLoader file_loader;
	void Start () {
		if (file_loader != null && filename != null){
			file_loader.rx_filename.Subscribe(text=>{
				if (filename != null && filename.text != null)
					filename.text = text;
			});
			filename.OnValueChangeAsObservable().Subscribe(f=>{
				file_loader.filename = f;
			});
		}
		if (directory_loader != null && directory != null){
			directory_loader.rx_directory.Subscribe(text=>{
				if (directory != null && directory.text != null)
					directory.text = text;
			});
			directory.OnValueChangeAsObservable().Subscribe(d=>{
				directory_loader.directory = d;
			});
		}
	}
}
