using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
	private GameManager m_gameManager;

	private VisibilityMeter m_visibilityMeter;
	private InteractIcon m_interactIcon;
	private HealthBar m_healthBar;
	private MissionScore m_missionScore;

	public void Init(GameManager gm)
	{
		m_gameManager = gm;

		m_visibilityMeter = transform.Find("Visibility Meter").GetComponent<VisibilityMeter>();
		m_interactIcon = transform.Find("Interact Icon").GetComponent<InteractIcon>();
		m_healthBar = transform.Find("Health Bar").GetComponent<HealthBar>();
		m_missionScore = transform.Find("Mission Score").GetComponent<MissionScore>();

		SetupHUD();

	}

	void SetupHUD()
	{
		m_healthBar.SetText(100);
		m_missionScore.SetText(0);
		m_interactIcon.ToggleInteract(false);


		m_gameManager.GetController().GetInteract().IsPresent += m_interactIcon.ToggleInteract;
		m_gameManager.GetLootManager().ScoreIncrease += m_missionScore.SetText;
		m_gameManager.GetController().GetHealthComponent().healthChange += m_healthBar.SetText;
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
	public GameManager GetManager()
	{
		return m_gameManager;
	}
}
