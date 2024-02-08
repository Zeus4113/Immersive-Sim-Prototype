using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

	private UIManager manager;


	public void Init(UIManager man)
	{
		manager = man;
	}

	private void OnEnable()
	{
		Time.timeScale = 0f;
	}

	public void ResumeGame()
	{
		manager.GameUnpaused();
	}

	public void RestartLevel()
	{
		SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}

	public void LoadMenuScene()
	{
		SceneManager.LoadScene("MainMenuScene");
	}

	private void OnDisable()
	{
		Time.timeScale = 1f;
	}

	private void OnDestroy()
	{
		Time.timeScale = 1f;
	}
}
