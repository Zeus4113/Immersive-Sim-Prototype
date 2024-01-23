using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
	public void RestartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}

	public void LoadMenuScene()
	{
		SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
	}
}
