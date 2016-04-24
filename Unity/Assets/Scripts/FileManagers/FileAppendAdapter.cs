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

public class FileAppendAdapter<T>: ISaver<T[]>, ISaver<T>{
	protected ILoader<T[]> _loader;
	public ILoader<T[]> loader{
		get{ return _loader; }
		private set{ 
			_loader = value;
		}
	}

	protected ISaver<T[]> _saver;
	public  ISaver<T[]> saver{
		get{ return _saver; }
		private set{ 
			_saver = value;
		}
	}
	
	public void save(T[] new_values){
		T[] previous = loader.load();
		T[] values = concat_array(previous, new_values);
		saver.save(values);
	}

	public T[] concat_array(T[] x, T[] y){
		T[] values = new T[x.Length + y.Length];
		int idx = 0;
		for (int i = 0; i < x.Length; i++)
			values[idx++] = x[i];
		for (int j = 0; j < y.Length; j++)
			values[idx++] = y[j];
		return values;
	}

	public void save(T single){
		save( new[]{single} );
	}
	
	public void rx_save(T[] new_values){
		loader.rx_load().Take(1).Select(
			previous=>concat_array(previous, new_values)
		).Subscribe(values=>{
			saver.rx_save(values);
		});
	}
	
	public void rx_save(T single){
		rx_save( new[]{single} );
	}
	
	public FileAppendAdapter(ILoader<T[]> _l, ISaver<T[]> _s){
		loader = _l; 
		saver = _s;
	}
}