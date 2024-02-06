using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class UIManager : MonoBehaviour
{
	private GameManager m_gameManager;

	private VisibilityMeter m_visibilityMeter;
	private InteractIcon m_interactIcon;
	private HealthBar m_healthBar;
	private MissionScore m_missionScore;
	private HintPopup m_hintPopup;
	private ToolMenu m_toolMenu;
	private DeathScreen m_deathScreen;
	private MissionSummary m_missionSummary;
	private PauseMenu m_pauseMenu;

	public void Init(GameManager gm)
	{
		m_gameManager = gm;

		m_visibilityMeter = transform.Find("Visibility Meter").GetComponent<VisibilityMeter>();
		m_interactIcon = transform.Find("Interact Icon").GetComponent<InteractIcon>();
		m_healthBar = transform.Find("Health Bar").GetComponent<HealthBar>();
		m_missionScore = transform.Find("Mission Score").GetComponent<MissionScore>();
		m_hintPopup = transform.Find("Hint Popup").GetComponent<HintPopup>();
		m_toolMenu = transform.Find("Tool Menu").GetComponent<ToolMenu>();
		m_deathScreen = transform.Find("Death Screen").GetComponent<DeathScreen>();
		m_missionSummary = transform.Find("Mission End Screen").GetComponent<MissionSummary>();
		m_pauseMenu = transform.Find("Pause Menu").GetComponent<PauseMenu>();

		SetupHUD();

	}

	void SetupHUD()
	{
		m_healthBar.SetText(100);
		m_missionScore.SetText(0);
		m_interactIcon.ToggleInteract(false);
		m_hintPopup.DisplayHint(null);
		m_pauseMenu.Init(this);


		//m_gameManager.GetController().GetInteract().IsPresent += m_interactIcon.ToggleInteract;

		m_gameManager.GetController().GetInteract().UpdateIcon += m_interactIcon.SetIcon;
		m_gameManager.GetLootManager().ScoreIncrease += m_missionScore.SetText;
		m_gameManager.GetController().GetHealthComponent().healthChange += m_healthBar.SetText;

		m_gameManager.GetController().GetToolset().changingTools += m_toolMenu.HighlightImage;
		m_gameManager.GetController().GetToolset().openMenu += m_toolMenu.OpenMenu;
		m_gameManager.GetController().GetToolset().closeMenu += m_toolMenu.CloseMenu;

		m_gameManager.GetMissionManager().missionComplete += MissionComplete;
	}

	public void ResetUI()
	{
		m_interactIcon.ToggleInteract(false);
		m_hintPopup.DisplayHint(null);
	}

	public void PlayerDead(bool isAlive)
	{
		m_deathScreen.gameObject.SetActive(!isAlive);
	}

	public void MissionComplete()
	{
		m_missionSummary.gameObject.SetActive(true);
	}

	public void GamePaused(InputAction.CallbackContext ctx)
	{
		m_pauseMenu.gameObject.SetActive(true);
		m_gameManager.EnableInputEvents(false);
	}

	public void GameUnpaused()
	{
		m_pauseMenu.gameObject.SetActive(false);
		m_gameManager.EnableInputEvents(true);
	}

	public PauseMenu GetPauseMenu()
	{
		return m_pauseMenu;
	}

	public DeathScreen GetDeathScreen()
	{
		return m_deathScreen;
	}

	public VisibilityMeter GetVisibilityMeter()
	{
		return m_visibilityMeter;
	}

	public MissionScore GetMissionScore()
	{
		return m_missionScore;
	}

	public HealthBar GetHealthBar()
	{
		return m_healthBar;
	}

	public InteractIcon GetInteractIcon()
	{
		return m_interactIcon;
	}

	public HintPopup GetHintPopup()
	{
		return m_hintPopup;
	}

	public GameManager GetManager()
	{
		return m_gameManager;
	}
}
