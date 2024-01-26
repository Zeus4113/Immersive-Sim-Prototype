using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
	[SerializeField] private UIManager m_UIManager;
	[SerializeField] private Player.Controller m_controller;
	[SerializeField] private LootManager m_lootManager;
	[SerializeField] private CheckpointManager m_checkpointManager;
	[SerializeField] private EnemyManager m_enemyManager;
	[SerializeField] private MissionManager m_missionManager;

	private PlayerInput m_input;

	private void Start()
	{
		m_input = GetComponent<PlayerInput>();

		SetupGame();
	}

	void SetupGame()
	{
		if(m_controller != null) m_controller.Init(m_input, this);

		if (m_lootManager != null) m_lootManager.Init(this);

		if (m_checkpointManager != null) m_checkpointManager.Init(this);

		if (m_enemyManager != null) m_enemyManager.Init(this);

		if (m_UIManager != null) m_UIManager.Init(this);

		if(m_missionManager != null ) m_missionManager.Init(this, m_enemyManager, m_lootManager);
	}
	public void EnableInputEvents(bool isTrue)
	{
		if (isTrue)
		{
			Cursor.lockState = CursorLockMode.Locked;
			m_input.actions.FindActionMap("Player Movement").Enable();
			m_input.actions.FindActionMap("UI").Disable();
		}
		else if (!isTrue)
		{
			Cursor.lockState = CursorLockMode.Confined;
			m_input.actions.FindActionMap("Player Movement").Disable();
			m_input.actions.FindActionMap("UI").Enable();
		}

	}

	public UIManager GetUIManager()
	{
		return m_UIManager;
	}

	public Player.Controller GetController()
	{
		return m_controller;
	}

	public LootManager GetLootManager()
	{
		return m_lootManager;
	}

	public CheckpointManager GetCheckpointManager()
	{
		return m_checkpointManager;
	}

	public EnemyManager GetEnemyManager()
	{
		return m_enemyManager;
	}

	public MissionManager GetMissionManager()
	{
		return m_missionManager;
	}
}
