using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Vexe.Runtime.Types;
public class LoadingProgress : BetterBehaviour {
	public Slider progress_slider;
	public ObjectVisibility show_loaded;
	public ObjectVisibility show_unloaded;
	
	// Update is called once per frame
	void Update () {
		progress_slider.value = LeafStateSystem.instance.loading_progress;
		show_loaded.visible = LeafStateSystem.instance.finished_loading;
		show_unloaded.visible = !LeafStateSystem.instance.finished_loading;
	}
}
