using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartGame : MonoBehaviour {
	public void trigger(){
		SceneManager.LoadScene("main", LoadSceneMode.Single);
	}
}
