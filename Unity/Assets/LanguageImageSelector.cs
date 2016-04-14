using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Vexe.Runtime.Types;
using UniRx;

public class LanguageImageSelector : BetterBehaviour {
	public Image target_image;
	public Sprite default_sprite;
	public Dictionary<string,Sprite> images = new Dictionary<string, Sprite>();
	public ILanguageController controller;

	// Use this for initialization
	void Start () {
		if (target_image == null)
			target_image = GetComponent<Image> ();
		if (default_sprite == null)
			default_sprite = target_image.sprite;
		if (controller == null)
			controller = LanguageController.controller;
		controller.rx_current_language_key.Subscribe ((string lang) => {
			if (images.ContainsKey(lang)){
				target_image.sprite = images[lang];
			} else {
				target_image.sprite = default_sprite;
			}
		});
	}
}
