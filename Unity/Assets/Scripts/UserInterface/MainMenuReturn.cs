using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuReturn : MonoBehaviour {
	public void trigger(){
		SceneManager.LoadScene("title_screen", LoadSceneMode.Single);
	}
}
